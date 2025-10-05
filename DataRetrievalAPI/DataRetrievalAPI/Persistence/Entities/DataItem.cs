using System.ComponentModel.DataAnnotations;

namespace DataRetrievalAPI.Persistence.Entities
{
    /// <summary>
    /// Entity representing a data item stored in the database.
    /// </summary>
    public class DataItem
    {
        /// <summary>
        /// The unique identifier of the data item.
        /// </summary>
        [Key]
        public Guid Id { get; set; }

        /// <summary>
        /// The actual data payload.
        /// </summary>
        public string Payload { get; set; } = string.Empty;

        /// <summary>
        /// The date and time when the data item was created (UTC).
        /// </summary>
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// The date and time when the data item was last updated (UTC), if applicable.
        /// </summary>
        public DateTime? UpdatedAt { get; set; }
    }
}
