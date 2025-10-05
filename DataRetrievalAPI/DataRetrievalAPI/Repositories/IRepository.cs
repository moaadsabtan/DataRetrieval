namespace DataRetrievalAPI.Repositories
{
    /// <summary>
    /// Generic repository interface for managing entities of type <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">The type of entity.</typeparam>
    public interface IRepository<T> where T : class
    {
        /// <summary>
        /// Retrieves an entity by its unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the entity.</param>
        /// <returns>The entity if found; otherwise, null.</returns>
        Task<T?> GetByIdAsync(Guid id);

        /// <summary>
        /// Adds a new entity to the repository.
        /// </summary>
        /// <param name="entity">The entity to add.</param>
        Task AddAsync(T entity);

        /// <summary>
        /// Updates an existing entity in the repository.
        /// </summary>
        /// <param name="entity">The entity to update.</param>
        Task UpdateAsync(T entity);

        /// <summary>
        /// Persists changes made in the repository to the underlying database.
        /// </summary>
        Task SaveChangesAsync();
    }
}
