using Polly;
using System.Text.Json;

namespace DataRetrievalAPI.Storage
{
    /// <summary>
    /// Storage implementation that persists data items as JSON files on the local filesystem.
    /// Supports file expiration and retry policies for I/O operations.
    /// </summary>
    public class FileStorage : IStorage
    {
        private readonly string _folder;
        private readonly TimeSpan _retention;
        private readonly ILogger<FileStorage> _log;

        /// <summary>
        /// Initializes a new instance of <see cref="FileStorage"/> with configuration settings.
        /// </summary>
        /// <param name="cfg">Application configuration containing file path and retention settings.</param>
        /// <param name="log">Logger instance for logging events.</param>
        public FileStorage(IConfiguration cfg, ILogger<FileStorage> log)
        {
            _log = log;
            _folder = cfg["Storage:FilePath"] ?? "./data/files";
            var minutes = int.Parse(cfg["Storage:FileRetentionMinutes"] ?? "30");
            _retention = TimeSpan.FromMinutes(minutes);
            Directory.CreateDirectory(_folder);
        }

        /// <summary>
        /// Generates a filename for a data item with expiration timestamp.
        /// </summary>
        /// <param name="id">The unique identifier of the data item.</param>
        /// <param name="expires">The expiration timestamp for the file.</param>
        /// <returns>The full path to the file.</returns>
        private string MakeFilename(Guid id, DateTime expires) => Path.Combine(_folder, $"{id}_expires_{expires:yyyyMMddHHmmss}.json");

        /// <summary>
        /// Saves a data item as a JSON file with an expiration time.
        /// Retries up to 3 times on I/O exceptions with exponential backoff.
        /// </summary>
        /// <param name="id">The unique identifier of the data item.</param>
        /// <param name="payload">The data payload to save.</param>
        public async Task SaveAsync(Guid id, string payload)
        {
            var expires = DateTime.UtcNow.Add(_retention);
            var filename = MakeFilename(id, expires);
            var policy = Policy.Handle<IOException>().WaitAndRetryAsync(3, attempt => TimeSpan.FromMilliseconds(100 * Math.Pow(2, attempt)));

            await policy.ExecuteAsync(async () =>
            {
                var wrapper = new { Id = id, Expires = expires, Payload = payload };
                var text = JsonSerializer.Serialize(wrapper);
                await File.WriteAllTextAsync(filename, text);
                _log.LogInformation("Wrote file {file}", filename);
            });
        }

        /// <summary>
        /// Reads the most recent non-expired JSON file for the given data item.
        /// Deletes expired files automatically.
        /// </summary>
        /// <param name="id">The unique identifier of the data item.</param>
        /// <returns>The data payload if a valid file exists; otherwise, <c>null</c>.</returns>
        public async Task<string?> ReadAsync(Guid id)
        {
            var files = Directory.EnumerateFiles(_folder, $"{id}_expires_*.json").ToList();
            if (!files.Any()) return null;

            foreach (var f in files.OrderByDescending(x => x))
            {
                try
                {
                    var text = await File.ReadAllTextAsync(f);
                    using var doc = JsonDocument.Parse(text);
                    var root = doc.RootElement;
                    var expires = root.GetProperty("Expires").GetDateTime();

                    if (expires < DateTime.UtcNow)
                    {
                        try { File.Delete(f); } catch { }
                        continue;
                    }

                    return root.GetProperty("Payload").GetString();
                }
                catch (Exception ex)
                {
                    _log.LogWarning(ex, "Failed read file {file}", f);
                    continue;
                }
            }

            return null;
        }
    }
}
