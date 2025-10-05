namespace DataRetrievalAPI.Services
{
    /// <summary>
    /// Service interface for managing data items, including retrieval, creation, and updates.
    /// </summary>
    public interface IDataService
    {
        /// <summary>
        /// Retrieves the payload of a data item by its unique identifier.
        /// Checks cache first, then file storage, then database.
        /// </summary>
        /// <param name="id">The unique identifier of the data item.</param>
        /// <returns>The data payload if found; otherwise, <c>null</c>.</returns>
        Task<string?> GetAsync(Guid id);

        /// <summary>
        /// Creates a new data item or updates an existing one in all storage layers and updates the cache.
        /// </summary>
        /// <param name="id">The unique identifier of the data item.</param>
        /// <param name="payload">The data payload to save.</param>
        Task CreateOrUpdateAsync(Guid id, string payload);
    }
}
