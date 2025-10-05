namespace DataRetrievalAPI.Storage
{
    /// <summary>
    /// Specifies the type of storage to use.
    /// </summary>
    public enum StorageType
    {
        /// <summary>
        /// Use file-based storage.
        /// </summary>
        File,

        /// <summary>
        /// Use database storage.
        /// </summary>
        Db
    }

    /// <summary>
    /// Factory class for creating <see cref="IStorage"/> instances based on <see cref="StorageType"/>.
    /// </summary>
    public class StorageFactory
    {
        private readonly IServiceProvider _provider;

        /// <summary>
        /// Initializes a new instance of <see cref="StorageFactory"/> with the specified service provider.
        /// </summary>
        /// <param name="provider">The service provider used to resolve storage instances.</param>
        public StorageFactory(IServiceProvider provider) => _provider = provider;

        /// <summary>
        /// Creates an <see cref="IStorage"/> implementation based on the given <see cref="StorageType"/>.
        /// </summary>
        /// <param name="type">The type of storage to create.</param>
        /// <returns>An instance of <see cref="IStorage"/> corresponding to the requested type.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if an unsupported <see cref="StorageType"/> is provided.</exception>
        public IStorage Create(StorageType type) => type switch
        {
            StorageType.File => _provider.GetRequiredService<FileStorage>(),
            StorageType.Db => _provider.GetRequiredService<DbStorage>(),
            _ => throw new ArgumentOutOfRangeException(nameof(type))
        };
    }
}
