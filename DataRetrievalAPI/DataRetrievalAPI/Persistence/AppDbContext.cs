using DataRetrievalAPI.Persistence.Entities;
using Microsoft.EntityFrameworkCore;

namespace DataRetrievalAPI.Persistence
{
    /// <summary>
    /// The application's database context, representing the connection to the database.
    /// </summary>
    public class AppDbContext : DbContext
    {
        /// <summary>
        /// Initializes a new instance of <see cref="AppDbContext"/> with the specified options.
        /// </summary>
        /// <param name="options">The options to configure the DbContext.</param>
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        /// <summary>
        /// Represents the collection of <see cref="DataItem"/> entities in the database.
        /// </summary>
        public DbSet<DataItem> DataItems { get; set; } = default!;

        /// <summary>
        /// Configures the entity model.
        /// </summary>
        /// <param name="modelBuilder">The builder used to configure entities.</param>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configure DataItem entity
            modelBuilder.Entity<DataItem>().HasKey(d => d.Id);

            base.OnModelCreating(modelBuilder);
        }
    }
}
