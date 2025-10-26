using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MTA.Application.DTOs;
using MTA.Application.Services;
using MTA.Domain.Constants;
using MTA.Web.Attributes;

namespace MTA.Web.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class LookupController : ControllerBase
    {
        private readonly ILookupService _lookupService;

        public LookupController(ILookupService lookupService)
        {
            _lookupService = lookupService;
        }

        [HttpGet]
        [AllowAnonymous]
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
        [AllowAnonymous]
        public async Task<ActionResult<LookupDto>> GetById(int id)
        {
            var lookup = await _lookupService.GetByIdAsync(id);
            if (lookup == null)
                return NotFound($"Lookup with ID {id} not found.");

            return Ok(lookup);
        }

        [HttpGet("category/{category}")]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<LookupDto>>> GetByCategory(string category)
        {
            var result = await _lookupService.GetByCategoryAsync(category);
            return Ok(result);
        }

        [HttpPost]
        [AuthorizeRole(RoleNames.Admin)]
        public async Task<ActionResult<LookupDto>> Create([FromBody] CreateLookupDto dto)
        {
            if (dto == null)
                return BadRequest("Invalid request.");

            var created = await _lookupService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        [HttpPut("{id:int}")]
        [AuthorizeRole(RoleNames.Admin)]
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
        [AuthorizeRole(RoleNames.Admin)]
        public async Task<ActionResult> Delete(int id)
        {
            var success = await _lookupService.DeleteAsync(id);
            if (!success)
                return NotFound($"Lookup with ID {id} not found.");

            return NoContent();
        }

        [HttpGet("categories")]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<string>>> GetCategories()
        {
            var result = await _lookupService.GetAllCategoriesAsync();
            return Ok(result);
        }
    }
}
