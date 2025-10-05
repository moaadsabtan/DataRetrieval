using DataRetrievalAPI.DTOs;
using DataRetrievalAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace DataRetrievalAPI.Controllers
{
    [ApiController]
    [Route("data")]
    public class DataController : ControllerBase
    {
        private readonly IDataService _service;
        public DataController(IDataService service) => _service = service;

        /// <summary>
        /// Retrieves data by its unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the data (GUID format).</param>
        /// <returns>Returns the data if found; otherwise, returns 404 Not Found.</returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(string id)
        {
            if (!Guid.TryParse(id, out var guid)) return BadRequest("Invalid id format");
            var result = await _service.GetAsync(guid);
            if (result == null) return NotFound();
            return Ok(new DataDto { Id = guid, Payload = result });
        }

        /// <summary>
        /// Creates a new data entry.
        /// </summary>
        /// <param name="dto">The payload and optional Id for the data.</param>
        /// <returns>Returns 201 Created with the new data Id.</returns>
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateUpdateDataDto dto)
        {
            var id = dto.Id ?? Guid.NewGuid();
            await _service.CreateOrUpdateAsync(id, dto.Payload);
            return CreatedAtAction(nameof(Get), new { id = id.ToString() }, new { id });
        }

        /// <summary>
        /// Updates an existing data entry by its Id.
        /// </summary>
        /// <param name="id">The unique identifier of the data to update (GUID format).</param>
        /// <param name="dto">The updated payload.</param>
        /// <returns>Returns 204 No Content on success.</returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] CreateUpdateDataDto dto)
        {
            if (!Guid.TryParse(id, out var guid)) return BadRequest("Invalid id format");
            await _service.CreateOrUpdateAsync(guid, dto.Payload);
            return NoContent();
        }
    }
}
