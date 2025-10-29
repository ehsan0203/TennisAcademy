using Microsoft.AspNetCore.Mvc;
using MTA.Application.DTOs;
using MTA.Application.Services;

namespace MTA.Web.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SiteRulesController : ControllerBase
    {
        private readonly ILookupService _lookupService;
        private readonly ILogger<SiteRulesController> _logger;

        public SiteRulesController(ILookupService lookupService, ILogger<SiteRulesController> logger)
        {
            _lookupService = lookupService;
            _logger = logger;
        }

        // GET: api/siterules
        [HttpGet("SiteRules")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<LookupDto>>> GetSiteRules([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var result = await _lookupService.GetAllAsync(page, pageSize, "SiteRule");
            return Ok(result);
        }

        [HttpGet("category")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<LookupDto>>> GetCategory([FromQuery] string CategoryName)
        {
            var result = await _lookupService.GetByCategoryAsync(CategoryName);
            return Ok(result);
        }

        [HttpGet("categories")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<string>>> Categories()
        {
            var result = await _lookupService.GetAllCategoriesAsync();
            return Ok(result);
        }

        // GET: api/siterules/5
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<LookupDto>> GetById(int id)
        {
            var rule = await _lookupService.GetByIdAsync(id);
            if (rule == null || rule.Category != "SiteRule")
                return NotFound();

            return Ok(rule);
        }

        // POST: api/siterules
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public async Task<ActionResult<LookupDto>> Create([FromBody] CreateLookupDto dto)
        {
            dto.Category = "SiteRule"; // همیشه دسته‌بندی رو SiteRule می‌ذاریم
            var created = await _lookupService.CreateAsync(dto);

            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        // PUT: api/siterules/5
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update(int id, [FromBody] LookupDto dto)
        {
            var existing = await _lookupService.GetByIdAsync(id);
            if (existing == null || existing.Category != "SiteRule")
                return NotFound();

            dto.Category = "SiteRule"; // دسته‌بندی نباید تغییر کنه
            await _lookupService.UpdateAsync(id, dto);

            return NoContent();
        }

        // DELETE: api/siterules/5
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(int id)
        {
            var existing = await _lookupService.GetByIdAsync(id);
            if (existing == null || existing.Category != "SiteRule")
                return NotFound();

            var deleted = await _lookupService.DeleteAsync(id);
            if (!deleted)
                return StatusCode(500, new { message = "Could not delete the rule" });

            return NoContent();
        }


    }

}
