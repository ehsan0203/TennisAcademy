using Microsoft.AspNetCore.Mvc;
using MTA.Application.DTOs;
using MTA.Application.Services;

namespace MTA.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LookupController : ControllerBase
    {
        private readonly ILookupService _lookupService;

        public LookupController(ILookupService lookupService)
        {
            _lookupService = lookupService;
        }

        [HttpGet]
        public async Task<ActionResult<PaginatedResult<LookupDto>>> GetAll(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? category = null,
            [FromQuery] string? searchTerm = null)
        {
            var result = await _lookupService.GetAllAsync(page, pageSize, category, searchTerm);
            return Ok(result);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<LookupDto>> GetById(int id)
        {
            var lookup = await _lookupService.GetByIdAsync(id);
            if (lookup == null)
                return NotFound($"Lookup with ID {id} not found.");

            return Ok(lookup);
        }

        [HttpGet("category/{category}")]
        public async Task<ActionResult<IEnumerable<LookupDto>>> GetByCategory(string category)
        {
            var result = await _lookupService.GetByCategoryAsync(category);
            return Ok(result);
        }


        [HttpPost]
        public async Task<ActionResult<LookupDto>> Create([FromBody] CreateLookupDto dto)
        {
            if (dto == null)
                return BadRequest("Invalid request.");

            var created = await _lookupService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult<LookupDto>> Update(int id, [FromBody] LookupDto dto)
        {
            if (dto == null)
                return BadRequest("Invalid request.");

            try
            {
                var updated = await _lookupService.UpdateAsync(id, dto);
                return Ok(updated);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult> Delete(int id)
        {
            var success = await _lookupService.DeleteAsync(id);
            if (!success)
                return NotFound($"Lookup with ID {id} not found.");

            return NoContent();
        }

        [HttpGet("categories")]
        public async Task<ActionResult<IEnumerable<string>>> GetCategories()
        {
            var result = await _lookupService.GetAllCategoriesAsync();
            return Ok(result);
        }

    }
}
