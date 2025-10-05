namespace DataRetrievalAPI.DTOs
{
    /// <summary>
    /// DTO used for creating or updating a data entry.
    /// </summary>
    public class CreateUpdateDataDto
    {
        /// <summary>
        /// Optional unique identifier for the data.
        /// If not provided during creation, a new GUID will be generated.
        /// </summary>
        public Guid? Id { get; set; }

        /// <summary>
        /// The actual data payload.
        /// </summary>
        public string Payload { get; set; } = string.Empty;
    }
}
