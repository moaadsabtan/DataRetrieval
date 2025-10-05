namespace DataRetrievalAPI.DTOs
{
    /// <summary>
    /// DTO representing the data returned by the API.
    /// </summary>
    public class DataDto
    {
        /// <summary>
        /// The unique identifier of the data.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// The actual data payload.
        /// </summary>
        public string Payload { get; set; } = string.Empty;
    }
}
