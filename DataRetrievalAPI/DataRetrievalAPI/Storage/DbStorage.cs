using DataRetrievalAPI.Persistence.Entities;
using DataRetrievalAPI.Repositories;

namespace DataRetrievalAPI.Storage
{
    /// <summary>
    /// Storage implementation that persists <see cref="DataItem"/> entities in the database.
    /// </summary>
    public class DbStorage : IStorage
    {
        private readonly IRepository<DataItem> _repo;

        /// <summary>
        /// Initializes a new instance of <see cref="DbStorage"/> with the specified repository.
        /// </summary>
        /// <param name="repo">Repository for managing <see cref="DataItem"/> entities.</param>
        public DbStorage(IRepository<DataItem> repo) => _repo = repo;

        /// <summary>
        /// Saves a data item in the database. Updates the item if it exists, otherwise creates a new one.
        /// </summary>
        /// <param name="id">The unique identifier of the data item.</param>
        /// <param name="payload">The data payload to save.</param>
        public async Task SaveAsync(Guid id, string payload)
        {
            var existing = await _repo.GetByIdAsync(id);
            if (existing != null)
            {
                existing.Payload = payload;
                existing.UpdatedAt = DateTime.UtcNow;
                await _repo.UpdateAsync(existing);
            }
            else
            {
                var item = new DataItem { Id = id, Payload = payload, CreatedAt = DateTime.UtcNow };
                await _repo.AddAsync(item);
            }
            await _repo.SaveChangesAsync();
        }

        /// <summary>
        /// Reads a data item payload from the database by its unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the data item.</param>
        /// <returns>The data payload if found; otherwise, <c>null</c>.</returns>
        public async Task<string?> ReadAsync(Guid id)
        {
            var item = await _repo.GetByIdAsync(id);
            return item?.Payload;
        }
    }
}
