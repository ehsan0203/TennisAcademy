using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MTA.Application.DTOs;
using MTA.Application.Services;
using MTA.Domain.Constants;
using MTA.Web.Attributes;

namespace MTA.Web.Controllers;

/// <summary>
/// Controller for managing FAQ categories and questions
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
[Authorize]
public class FAQController : ControllerBase
{
    private readonly IFAQService _faqService;
    private readonly IMapper _mapper;

    public FAQController(IFAQService faqService, IMapper mapper)
    {
        _faqService = faqService;
        _mapper = mapper;
    }

    #region Categories

    /// <summary>
    /// Gets all FAQ categories with optional filtering and pagination
    /// </summary>
    /// <param name="page">Page number (default: 1)</param>
    /// <param name="pageSize">Page size (default: 10)</param>
    /// <param name="searchTerm">Search term for title or description</param>
    /// <param name="isActive">Filter by active status</param>
    /// <returns>Paginated list of FAQ categories</returns>
    /// <response code="200">Returns the paginated list of FAQ categories</response>
    /// <response code="400">If the request parameters are invalid</response>
    /// <response code="401">If the user is not authenticated</response>
    /// <response code="403">If the user doesn't have required role</response>
    /// <response code="500">If there was an internal server error</response>
    [HttpGet("categories")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<PaginatedResult<FAQCategoryDto>>> GetCategories(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? searchTerm = null,
        [FromQuery] bool? isActive = null)
    {
        try
        {
            if (page < 1 || pageSize < 1 || pageSize > 100)
                return BadRequest("Invalid pagination parameters");

            var result = await _faqService.GetCategoriesAsync(page, pageSize, searchTerm, isActive);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    /// <summary>
    /// Gets a specific FAQ category by ID
    /// </summary>
    /// <param name="id">The ID of the FAQ category</param>
    /// <returns>The requested FAQ category</returns>
    /// <response code="200">Returns the requested FAQ category</response>
    /// <response code="404">If the FAQ category was not found</response>
    /// <response code="500">If there was an internal server error</response>
    [HttpGet("category/{id}")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<FAQCategoryDto>> GetCategory(int id)
    {
        try
        {
            var category = await _faqService.GetCategoryByIdAsync(id);
            if (category == null)
                return NotFound($"FAQ category with ID {id} not found");

            return Ok(category);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    /// <summary>
    /// Creates a new FAQ category
    /// </summary>
    /// <param name="categoryDto">The FAQ category data</param>
    /// <returns>The created FAQ category</returns>
    /// <response code="201">Returns the newly created FAQ category</response>
    /// <response code="400">If the FAQ category data is invalid</response>
    /// <response code="401">If the user is not authenticated</response>
    /// <response code="403">If the user doesn't have required role</response>
    /// <response code="500">If there was an internal server error</response>
    [HttpPost("CreateCategory")]
    [AuthorizeRole(RoleNames.Admin, RoleNames.Coach)]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<FAQCategoryDto>> CreateCategory([FromBody] CreateFAQCategoryDto categoryDto)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var createdCategory = await _faqService.CreateCategoryAsync(categoryDto);
            return CreatedAtAction(nameof(GetCategory), new { id = createdCategory.Id }, createdCategory);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    /// <summary>
    /// Updates an existing FAQ category
    /// </summary>
    /// <param name="id">The ID of the FAQ category to update</param>
    /// <param name="categoryDto">The updated FAQ category data</param>
    /// <returns>The updated FAQ category</returns>
    /// <response code="200">Returns the updated FAQ category</response>
    /// <response code="400">If the FAQ category data is invalid</response>
    /// <response code="404">If the FAQ category was not found</response>
    /// <response code="401">If the user is not authenticated</response>
    /// <response code="403">If the user doesn't have required role</response>
    /// <response code="500">If there was an internal server error</response>
    [HttpPut("UpdateCategory/{id}")]
    [AuthorizeRole(RoleNames.Admin, RoleNames.Coach)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<FAQCategoryDto>> UpdateCategory(int id, [FromBody] FAQCategoryDto categoryDto)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var updatedCategory = await _faqService.UpdateCategoryAsync(id, categoryDto);
            return Ok(updatedCategory);
        }
        catch (ArgumentException ex)
        {
            return NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    /// <summary>
    /// Deletes an FAQ category
    /// </summary>
    /// <param name="id">The ID of the FAQ category to delete</param>
    /// <returns>No content on successful deletion</returns>
    /// <response code="204">If the FAQ category was successfully deleted</response>
    /// <response code="400">If the FAQ category cannot be deleted (has questions)</response>
    /// <response code="404">If the FAQ category was not found</response>
    /// <response code="401">If the user is not authenticated</response>
    /// <response code="403">If the user doesn't have required role</response>
    /// <response code="500">If there was an internal server error</response>
    [HttpDelete("DeleteCategory/{id}")]
    [AuthorizeRole(RoleNames.Admin, RoleNames.Coach)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> DeleteCategory(int id)
    {
        try
        {
            var deleted = await _faqService.DeleteCategoryAsync(id);
            if (!deleted)
                return NotFound($"FAQ category with ID {id} not found");

            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    /// <summary>
    /// Toggles the active status of an FAQ category
    /// </summary>
    /// <param name="id">The ID of the FAQ category</param>
    /// <returns>The updated FAQ category</returns>
    /// <response code="200">Returns the updated FAQ category</response>
    /// <response code="404">If the FAQ category was not found</response>
    /// <response code="500">If there was an internal server error</response>
    [HttpPatch("categories/{id}/toggle-status")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<FAQCategoryDto>> ToggleCategoryStatus(int id)
    {
        try
        {
            var updatedCategory = await _faqService.ToggleCategoryStatusAsync(id);
            return Ok(updatedCategory);
        }
        catch (ArgumentException ex)
        {
            return NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    #endregion

    #region Questions

    /// <summary>
    /// Gets all questions in a specific category with pagination
    /// </summary>
    /// <param name="categoryId">The ID of the category</param>
    /// <param name="page">Page number (default: 1)</param>
    /// <param name="pageSize">Page size (default: 10)</param>
    /// <param name="isActive">Filter by active status</param>
    /// <returns>Paginated list of questions</returns>
    /// <response code="200">Returns the paginated list of questions</response>
    /// <response code="400">If the request parameters are invalid</response>
    /// <response code="500">If there was an internal server error</response>
    [HttpGet("GetQuestionsByCategory/{categoryId}/questions")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<PaginatedResult<QuestionDto>>> GetQuestionsByCategory(
        int categoryId,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] bool? isActive = null)
    {
        try
        {
            if (page < 1 || pageSize < 1 || pageSize > 100)
                return BadRequest("Invalid pagination parameters");

            var result = await _faqService.GetQuestionsByCategoryAsync(categoryId, page, pageSize, isActive);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    /// <summary>
    /// Gets a specific question by ID
    /// </summary>
    /// <param name="id">The ID of the question</param>
    /// <returns>The requested question</returns>
    /// <response code="200">Returns the requested question</response>
    /// <response code="404">If the question was not found</response>
    /// <response code="500">If there was an internal server error</response>
    [HttpGet("GetQuestion/{id}")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<QuestionDto>> GetQuestion(int id)
    {
        try
        {
            var question = await _faqService.GetQuestionByIdAsync(id);
            if (question == null)
                return NotFound($"Question with ID {id} not found");

            return Ok(question);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    /// <summary>
    /// Creates a new question
    /// </summary>
    /// <param name="questionDto">The question data</param>
    /// <returns>The created question</returns>
    /// <response code="201">Returns the newly created question</response>
    /// <response code="400">If the question data is invalid</response>
    /// <response code="401">If the user is not authenticated</response>
    /// <response code="403">If the user doesn't have required role</response>
    /// <response code="500">If there was an internal server error</response>
    [HttpPost("CreateQuestion")]
    [AuthorizeRole(RoleNames.Admin, RoleNames.Coach)]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<QuestionDto>> CreateQuestion([FromBody] CreateQuestionDto questionDto)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var createdQuestion = await _faqService.CreateQuestionAsync(questionDto);
            return CreatedAtAction(nameof(GetQuestion), new { id = createdQuestion.Id }, createdQuestion);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    /// <summary>
    /// Updates an existing question
    /// </summary>
    /// <param name="id">The ID of the question to update</param>
    /// <param name="questionDto">The updated question data</param>
    /// <returns>The updated question</returns>
    /// <response code="200">Returns the updated question</response>
    /// <response code="400">If the question data is invalid</response>
    /// <response code="404">If the question was not found</response>
    /// <response code="401">If the user is not authenticated</response>
    /// <response code="403">If the user doesn't have required role</response>
    /// <response code="500">If there was an internal server error</response>
    [HttpPut("UpdateQuestion/{id}")]
    [AuthorizeRole(RoleNames.Admin, RoleNames.Coach)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<QuestionDto>> UpdateQuestion(int id, [FromBody] UpdateQuestionDto questionDto)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var updatedQuestion = await _faqService.UpdateQuestionAsync(id, questionDto);
            return Ok(updatedQuestion);
        }
        catch (ArgumentException ex)
        {
            return NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    /// <summary>
    /// Deletes a question
    /// </summary>
    /// <param name="id">The ID of the question to delete</param>
    /// <returns>No content on successful deletion</returns>
    /// <response code="204">If the question was successfully deleted</response>
    /// <response code="404">If the question was not found</response>
    /// <response code="401">If the user is not authenticated</response>
    /// <response code="403">If the user doesn't have required role</response>
    /// <response code="500">If there was an internal server error</response>
    [HttpDelete("DeleteQuestion/{id}")]
    [AuthorizeRole(RoleNames.Admin, RoleNames.Coach)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> DeleteQuestion(int id)
    {
        try
        {
            var deleted = await _faqService.DeleteQuestionAsync(id);
            if (!deleted)
                return NotFound($"Question with ID {id} not found");

            return NoContent();
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    /// <summary>
    /// Toggles the active status of a question
    /// </summary>
    /// <param name="id">The ID of the question</param>
    /// <returns>The updated question</returns>
    /// <response code="200">Returns the updated question</response>
    /// <response code="404">If the question was not found</response>
    /// <response code="500">If there was an internal server error</response>
    [HttpPatch("questions/{id}/toggle-status")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<QuestionDto>> ToggleQuestionStatus(int id)
    {
        try
        {
            var updatedQuestion = await _faqService.ToggleQuestionStatusAsync(id);
            return Ok(updatedQuestion);
        }
        catch (ArgumentException ex)
        {
            return NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    #endregion

    #region Search and Utilities

    /// <summary>
    /// Searches questions across all categories
    /// </summary>
    /// <param name="searchTerm">The search term</param>
    /// <param name="page">Page number (default: 1)</param>
    /// <param name="pageSize">Page size (default: 10)</param>
    /// <returns>Paginated search results</returns>
    /// <response code="200">Returns the search results</response>
    /// <response code="400">If the request parameters are invalid</response>
    /// <response code="500">If there was an internal server error</response>
    [HttpGet("search")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<PaginatedResult<QuestionDto>>> SearchQuestions(
        [FromQuery] string searchTerm,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
                return BadRequest("Search term is required");

            if (page < 1 || pageSize < 1 || pageSize > 100)
                return BadRequest("Invalid pagination parameters");

            var result = await _faqService.SearchQuestionsAsync(searchTerm, page, pageSize);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    ///// <summary>
    ///// Gets frequently asked questions
    ///// </summary>
    ///// <param name="limit">Maximum number of questions to return (default: 10)</param>
    ///// <returns>List of frequently asked questions</returns>
    ///// <response code="200">Returns the list of frequently asked questions</response>
    ///// <response code="500">If there was an internal server error</response>
    //[HttpGet("frequently-asked")]
    //[AllowAnonymous]
    //[ProducesResponseType(StatusCodes.Status200OK)]
    //[ProducesResponseType(StatusCodes.Status500InternalServerError)]
    //public async Task<ActionResult<IEnumerable<QuestionDto>>> GetFrequentlyAskedQuestions([FromQuery] int limit = 10)
    //{
    //    try
    //    {
    //        if (limit < 1 || limit > 100)
    //            limit = 10;

    //        var questions = await _faqService.GetFrequentlyAskedQuestionsAsync(limit);
    //        return Ok(questions);
    //    }
    //    catch (Exception ex)
    //    {
    //        return StatusCode(500, $"Internal server error: {ex.Message}");
    //    }
    //}

    ///// <summary>
    ///// Gets FAQ statistics
    ///// </summary>
    ///// <returns>FAQ statistics</returns>
    ///// <response code="200">Returns the FAQ statistics</response>
    ///// <response code="500">If there was an internal server error</response>
    //[HttpGet("statistics")]
    //[ProducesResponseType(StatusCodes.Status200OK)]
    //[ProducesResponseType(StatusCodes.Status500InternalServerError)]
    //public async Task<ActionResult<FAQStatisticsDto>> GetStatistics()
    //{
    //    try
    //    {
    //        var statistics = await _faqService.GetStatisticsAsync();
    //        return Ok(statistics);
    //    }
    //    catch (Exception ex)
    //    {
    //        return StatusCode(500, $"Internal server error: {ex.Message}");
    //    }
    //}

    ///// <summary>
    ///// Gets categories with question count
    ///// </summary>
    ///// <returns>List of categories with question counts</returns>
    ///// <response code="200">Returns the list of categories with question counts</response>
    ///// <response code="500">If there was an internal server error</response>
    //[HttpGet("categories-with-counts")]
    //[AllowAnonymous]
    //[ProducesResponseType(StatusCodes.Status200OK)]
    //[ProducesResponseType(StatusCodes.Status500InternalServerError)]
    //public async Task<ActionResult<IEnumerable<FAQCategoryDto>>> GetCategoriesWithQuestionCount()
    //{
    //    try
    //    {
    //        var categories = await _faqService.GetCategoriesWithQuestionCountAsync();
    //        return Ok(categories);
    //    }
    //    catch (Exception ex)
    //    {
    //        return StatusCode(500, $"Internal server error: {ex.Message}");
    //    }
    //}

    #endregion
}

