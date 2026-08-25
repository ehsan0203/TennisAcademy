using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MTA.Application.DTOs;
using MTA.Application.Services;
using MTA.Web.Models;

namespace MTA.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
[Authorize]
public class FAQController : ControllerBase
{
    private readonly IFAQService _faqService;

    public FAQController(IFAQService faqService)
    {
        _faqService = faqService;
    }

    #region Categories

    [HttpGet("categories")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(CustomJsonResult<PaginatedResult<FAQCategoryDto>>), StatusCodes.Status200OK)]
    public async Task<CustomJsonResult<PaginatedResult<FAQCategoryDto>>> GetCategories(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? searchTerm = null,
        [FromQuery] bool? isActive = null,
        CancellationToken ct = default)
    {
        var result = await _faqService.GetCategoriesAsync(page, pageSize, searchTerm, isActive, ct);
        return CustomJsonResult<PaginatedResult<FAQCategoryDto>>.SuccessResult(result);
    }

    [HttpGet("category/{id}")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(CustomJsonResult<FAQCategoryDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(CustomJsonResult<string>), StatusCodes.Status404NotFound)]
    public async Task<CustomJsonResult<FAQCategoryDto>> GetCategory(int id, CancellationToken ct)
    {
        var category = await _faqService.GetCategoryByIdAsync(id, ct);
        return category is null
            ? CustomJsonResult<FAQCategoryDto>.NotFound($"FAQ category with ID {id} not found")
            : CustomJsonResult<FAQCategoryDto>.SuccessResult(category);
    }

    [HttpPost("categories")]
    [ProducesResponseType(typeof(CustomJsonResult<FAQCategoryDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(CustomJsonResult<string>), StatusCodes.Status400BadRequest)]
    public async Task<CustomJsonResult<FAQCategoryDto>> CreateCategory([FromBody] CreateFAQCategoryDto categoryDto, CancellationToken ct)
    {
        var created = await _faqService.CreateCategoryAsync(categoryDto, ct);
        return CustomJsonResult<FAQCategoryDto>.Created(created);
    }

    [HttpPut("categories/{id}")]
    [ProducesResponseType(typeof(CustomJsonResult<FAQCategoryDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(CustomJsonResult<string>), StatusCodes.Status404NotFound)]
    public async Task<CustomJsonResult<FAQCategoryDto>> UpdateCategory(int id, [FromBody] FAQCategoryDto categoryDto, CancellationToken ct)
    {
        var updated = await _faqService.UpdateCategoryAsync(id, categoryDto, ct);
        return CustomJsonResult<FAQCategoryDto>.SuccessResult(updated);
    }

    [HttpDelete("categories/{id}")]
    [ProducesResponseType(typeof(CustomJsonResult<string>), StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(CustomJsonResult<string>), StatusCodes.Status404NotFound)]
    public async Task<CustomJsonResult<string>> DeleteCategory(int id, CancellationToken ct)
    {
        var deleted = await _faqService.DeleteCategoryAsync(id, ct);
        return deleted
            ? CustomJsonResult<string>.NoContent()
            : CustomJsonResult<string>.NotFound($"FAQ category with ID {id} not found");
    }

    [HttpPatch("categories/{id}/toggle-status")]
    [ProducesResponseType(typeof(CustomJsonResult<FAQCategoryDto>), StatusCodes.Status200OK)]
    public async Task<CustomJsonResult<FAQCategoryDto>> ToggleCategoryStatus(int id, CancellationToken ct)
    {
        var updated = await _faqService.ToggleCategoryStatusAsync(id, ct);
        return CustomJsonResult<FAQCategoryDto>.SuccessResult(updated);
    }

    #endregion

    #region Questions

    [HttpGet("categories/{categoryId}/questions")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(CustomJsonResult<PaginatedResult<QuestionDto>>), StatusCodes.Status200OK)]
    public async Task<CustomJsonResult<PaginatedResult<QuestionDto>>> GetQuestionsByCategory(
        int categoryId,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] bool? isActive = null,
        CancellationToken ct = default)
    {
        var result = await _faqService.GetQuestionsByCategoryAsync(categoryId, page, pageSize, isActive, ct);
        return CustomJsonResult<PaginatedResult<QuestionDto>>.SuccessResult(result);
    }

    [HttpGet("questions/{id}")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(CustomJsonResult<QuestionDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(CustomJsonResult<string>), StatusCodes.Status404NotFound)]
    public async Task<CustomJsonResult<QuestionDto>> GetQuestion(int id, CancellationToken ct)
    {
        var question = await _faqService.GetQuestionByIdAsync(id, ct);
        return question is null
            ? CustomJsonResult<QuestionDto>.NotFound($"Question with ID {id} not found")
            : CustomJsonResult<QuestionDto>.SuccessResult(question);
    }

    [HttpPost("questions")]
    [ProducesResponseType(typeof(CustomJsonResult<QuestionDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(CustomJsonResult<string>), StatusCodes.Status400BadRequest)]
    public async Task<CustomJsonResult<QuestionDto>> CreateQuestion([FromBody] CreateQuestionDto questionDto, CancellationToken ct)
    {
        var created = await _faqService.CreateQuestionAsync(questionDto, ct);
        return CustomJsonResult<QuestionDto>.Created(created);
    }

    [HttpPut("questions/{id}")]
    [ProducesResponseType(typeof(CustomJsonResult<QuestionDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(CustomJsonResult<string>), StatusCodes.Status404NotFound)]
    public async Task<CustomJsonResult<QuestionDto>> UpdateQuestion(int id, [FromBody] UpdateQuestionDto questionDto, CancellationToken ct)
    {
        var updated = await _faqService.UpdateQuestionAsync(id, questionDto, ct);
        return CustomJsonResult<QuestionDto>.SuccessResult(updated);
    }

    [HttpDelete("questions/{id}")]
    [ProducesResponseType(typeof(CustomJsonResult<string>), StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(CustomJsonResult<string>), StatusCodes.Status404NotFound)]
    public async Task<CustomJsonResult<string>> DeleteQuestion(int id, CancellationToken ct)
    {
        var deleted = await _faqService.DeleteQuestionAsync(id, ct);
        return deleted
            ? CustomJsonResult<string>.NoContent()
            : CustomJsonResult<string>.NotFound($"Question with ID {id} not found");
    }

    [HttpPatch("questions/{id}/toggle-status")]
    [ProducesResponseType(typeof(CustomJsonResult<QuestionDto>), StatusCodes.Status200OK)]
    public async Task<CustomJsonResult<QuestionDto>> ToggleQuestionStatus(int id, CancellationToken ct)
    {
        var updated = await _faqService.ToggleQuestionStatusAsync(id, ct);
        return CustomJsonResult<QuestionDto>.SuccessResult(updated);
    }

    [HttpGet("search")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(CustomJsonResult<PaginatedResult<QuestionDto>>), StatusCodes.Status200OK)]
    public async Task<CustomJsonResult<PaginatedResult<QuestionDto>>> SearchQuestions(
        [FromQuery] string searchTerm,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken ct = default)
    {
        var result = await _faqService.SearchQuestionsAsync(searchTerm, page, pageSize, ct);
        return CustomJsonResult<PaginatedResult<QuestionDto>>.SuccessResult(result);
    }

    #endregion
}
