using DataRetrievalAPI.Persistence.Entities;
using DataRetrievalAPI.Repositories;
using DataRetrievalAPI.Storage;
using Microsoft.Extensions.Caching.Distributed;
using Polly;

namespace DataRetrievalAPI.Services
{
    /// <summary>
    /// Service responsible for managing <see cref="DataItem"/> data operations,
    /// including caching, file storage, and database storage with retry policies.
    /// </summary>
    public class DataService : IDataService
    {
        private readonly IDistributedCache _cache;
        private readonly FileStorage _fileStorage;
        private readonly DbStorage _dbStorage;
        private readonly StorageFactory _factory;
        private readonly IRepository<DataItem> _repo;
        private readonly ILogger<DataService> _log;
        private readonly TimeSpan _cacheTtl;

        /// <summary>
        /// Initializes a new instance of <see cref="DataService"/>.
        /// </summary>
        /// <param name="cache">Distributed cache for fast access.</param>
        /// <param name="fileStorage">File storage service.</param>
        /// <param name="dbStorage">Database storage service.</param>
        /// <param name="factory">Factory for creating storage types.</param>
        /// <param name="repo">Repository for <see cref="DataItem"/> entities.</param>
        /// <param name="cfg">Application configuration for cache settings.</param>
        /// <param name="log">Logger instance for logging events.</param>
        public DataService(IDistributedCache cache, FileStorage fileStorage, DbStorage dbStorage, StorageFactory factory, IRepository<DataItem> repo, IConfiguration cfg, ILogger<DataService> log)
        {
            _cache = cache;
            _fileStorage = fileStorage;
            _dbStorage = dbStorage;
            _factory = factory;
            _repo = repo;
            _log = log;
            _cacheTtl = TimeSpan.FromMinutes(int.Parse(cfg["Storage:CacheMinutes"] ?? "10"));
        }

        /// <summary>
        /// Retrieves data for the given identifier, checking cache first, then file storage, then database.
        /// Implements retry policies for file and database reads.
        /// </summary>
        /// <param name="id">The unique identifier of the data item.</param>
        /// <returns>The data payload if found; otherwise, <c>null</c>.</returns>
        public async Task<string?> GetAsync(Guid id)
        {
            var cacheKey = $"data:{id}";

            // Check cache first
            var cached = await _cache.GetStringAsync(cacheKey);
            if (!string.IsNullOrEmpty(cached))
            {
                _log.LogInformation("Cache hit {id}", id);
                return cached;
            }

            // Try reading from file storage with retry
            var filePolicy = Policy<string?>.Handle<Exception>().RetryAsync(2);
            var fileResult = await filePolicy.ExecuteAsync(() => _fileStorage.ReadAsync(id));
            if (!string.IsNullOrEmpty(fileResult))
            {
                _log.LogInformation("File hit {id}", id);
                await _cache.SetStringAsync(cacheKey, fileResult, new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = _cacheTtl });
                return fileResult;
            }

            // Try reading from database with exponential backoff retry
            var dbPolicy = Policy<string?>.Handle<Exception>().WaitAndRetryAsync(3, i => TimeSpan.FromMilliseconds(100 * Math.Pow(2, i)));
            var dbResult = await dbPolicy.ExecuteAsync(async () => await _dbStorage.ReadAsync(id));
            if (!string.IsNullOrEmpty(dbResult))
            {
                _log.LogInformation("DB hit {id}", id);
                _ = _fileStorage.SaveAsync(id, dbResult); // Fire-and-forget save to file
                await _cache.SetStringAsync(cacheKey, dbResult, new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = _cacheTtl });
                return dbResult;
            }

            // Data not found
            return null;
        }

        /// <summary>
        /// Creates or updates a data item in all storage layers and updates the cache.
        /// </summary>
        /// <param name="id">The unique identifier of the data item.</param>
        /// <param name="payload">The data payload to save.</param>
        public async Task CreateOrUpdateAsync(Guid id, string payload)
        {
            // Save to database storage
            var storage = _factory.Create(StorageType.Db);
            await storage.SaveAsync(id, payload);

            // Save to file storage
            var file = _factory.Create(StorageType.File);
            await file.SaveAsync(id, payload);

            // Update cache
            var cacheKey = $"data:{id}";
            await _cache.SetStringAsync(cacheKey, payload, new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = _cacheTtl });
        }
    }
}
