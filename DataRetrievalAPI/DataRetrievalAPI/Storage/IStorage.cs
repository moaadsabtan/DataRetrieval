namespace DataRetrievalAPI.Storage
{
    /// <summary>
    /// Defines a storage abstraction for persisting and retrieving data items.
    /// Implementations can use file system, database, or other storage mechanisms.
    /// </summary>
    public interface IStorage
    {
        /// <summary>
        /// Saves a data item to the storage.
        /// Implementations should handle creation or update as needed.
        /// </summary>
        /// <param name="id">The unique identifier of the data item.</param>
        /// <param name="payload">The data payload to save.</param>
        Task SaveAsync(Guid id, string payload);

        /// <summary>
        /// Reads a data item from the storage by its unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the data item.</param>
        /// <returns>The data payload if found; otherwise, <c>null</c>.</returns>
        Task<string?> ReadAsync(Guid id);
    }
}
