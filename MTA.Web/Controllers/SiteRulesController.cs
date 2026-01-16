using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MTA.Application.DTOs;
using MTA.Application.Services;
using MTA.Domain.Constants;
using MTA.Web.Attributes;

namespace MTA.Web.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
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
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<LookupDto>>> GetSiteRules([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var result = await _lookupService.GetAllAsync(page, pageSize, "SiteRule");
            return Ok(result);
        }

        [HttpGet("category")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<LookupDto>>> GetCategory([FromQuery] string CategoryName)
        {
            var result = await _lookupService.GetByCategoryAsync(CategoryName);
            return Ok(result);
        }

        [HttpGet("categories")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<string>>> Categories()
        {
            var result = await _lookupService.GetAllCategoriesAsync();
            return Ok(result);
        }

        // GET: api/siterules/5
        [HttpGet("{id}")]
        [AllowAnonymous]
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
        [AuthorizeRole(RoleNames.Admin)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public async Task<ActionResult<LookupDto>> Create([FromBody] SiteRuleUpsertDto dto)
        {
            var createDto = new CreateLookupDto
            {
                Category = "SiteRule",
                Key = dto.Subject,
                Value = dto.Text
            };
            var created = await _lookupService.CreateAsync(createDto);

            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        // PUT: api/siterules/5
        [HttpPut("{id}")]
        [AuthorizeRole(RoleNames.Admin)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update(int id, [FromBody] SiteRuleUpsertDto dto)
        {
            var existing = await _lookupService.GetByIdAsync(id);
            if (existing == null || existing.Category != "SiteRule")
                return NotFound();

            var updateDto = new LookupDto
            {
                Id = id,
                Category = "SiteRule",
                Key = dto.Subject,
                Value = dto.Text
            };
            await _lookupService.UpdateAsync(id, updateDto);

            return NoContent();
        }

        // DELETE: api/siterules/5
        [HttpDelete("{id}")]
        [AuthorizeRole(RoleNames.Admin)]
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
