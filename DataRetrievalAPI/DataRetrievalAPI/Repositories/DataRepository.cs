using DataRetrievalAPI.Persistence;
using DataRetrievalAPI.Persistence.Entities;

namespace DataRetrievalAPI.Repositories
{
    /// <summary>
    /// Repository for managing <see cref="DataItem"/> entities in the database.
    /// </summary>
    public class DataRepository : IRepository<DataItem>
    {
        private readonly AppDbContext _db;

        /// <summary>
        /// Initializes a new instance of <see cref="DataRepository"/> with the specified database context.
        /// </summary>
        /// <param name="db">The application's database context.</param>
        public DataRepository(AppDbContext db) => _db = db;

        /// <summary>
        /// Adds a new <see cref="DataItem"/> to the database context.
        /// </summary>
        /// <param name="entity">The data item to add.</param>
        public async Task AddAsync(DataItem entity) => await _db.DataItems.AddAsync(entity);

        /// <summary>
        /// Retrieves a <see cref="DataItem"/> by its unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the data item.</param>
        /// <returns>The matching <see cref="DataItem"/> if found; otherwise, null.</returns>
        public async Task<DataItem?> GetByIdAsync(Guid id) => await _db.DataItems.FindAsync(id);

        /// <summary>
        /// Updates an existing <see cref="DataItem"/> in the database context.
        /// </summary>
        /// <param name="entity">The data item to update.</param>
        public Task UpdateAsync(DataItem entity)
        {
            _db.DataItems.Update(entity);
            return Task.CompletedTask;
        }

        /// <summary>
        /// Persists changes made in the database context to the database.
        /// </summary>
        public Task SaveChangesAsync() => _db.SaveChangesAsync();
    }
}
