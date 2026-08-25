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
public class UserCourseHistoryController : ControllerBase
{
    private readonly IUserCourseHistoryService _userCourseHistoryService;

    public UserCourseHistoryController(IUserCourseHistoryService userCourseHistoryService)
    {
        _userCourseHistoryService = userCourseHistoryService;
    }

    [HttpGet]
    [ProducesResponseType(typeof(CustomJsonResult<PaginatedResult<UserCourseHistoryDetailDto>>), StatusCodes.Status200OK)]
    public async Task<CustomJsonResult<PaginatedResult<UserCourseHistoryDetailDto>>> GetUserCourseHistories(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] int? accountId = null,
        [FromQuery] int? courseId = null,
        CancellationToken ct = default)
    {
        var result = await _userCourseHistoryService.GetAllAsync(page, pageSize, accountId, courseId, ct);
        return CustomJsonResult<PaginatedResult<UserCourseHistoryDetailDto>>.SuccessResult(result);
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(CustomJsonResult<UserCourseHistoryDetailDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(CustomJsonResult<string>), StatusCodes.Status404NotFound)]
    public async Task<CustomJsonResult<UserCourseHistoryDetailDto>> GetUserCourseHistory(int id, CancellationToken ct)
    {
        var result = await _userCourseHistoryService.GetByIdAsync(id, ct);
        return result is null
            ? CustomJsonResult<UserCourseHistoryDetailDto>.NotFound($"User course history with ID {id} not found")
            : CustomJsonResult<UserCourseHistoryDetailDto>.SuccessResult(result);
    }

    [HttpPost]
    [ProducesResponseType(typeof(CustomJsonResult<UpdateUserCourseHistoryDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(CustomJsonResult<string>), StatusCodes.Status400BadRequest)]
    public async Task<CustomJsonResult<UpdateUserCourseHistoryDto>> CreateUserCourseHistory([FromBody] CreateUserCourseHistoryDto dto, CancellationToken ct)
    {
        var created = await _userCourseHistoryService.CreateAsync(dto, ct);
        return CustomJsonResult<UpdateUserCourseHistoryDto>.Created(created);
    }

    [HttpGet("account/{accountId}")]
    [ProducesResponseType(typeof(CustomJsonResult<IEnumerable<UserCourseHistoryDetailDto>>), StatusCodes.Status200OK)]
    public async Task<CustomJsonResult<IEnumerable<UserCourseHistoryDetailDto>>> GetByAccount(int accountId, CancellationToken ct)
    {
        var result = await _userCourseHistoryService.GetByAccountAsync(accountId, ct);
        return CustomJsonResult<IEnumerable<UserCourseHistoryDetailDto>>.SuccessResult(result);
    }

    [HttpGet("course/{courseId}")]
    [ProducesResponseType(typeof(CustomJsonResult<IEnumerable<UserCourseHistoryDetailDto>>), StatusCodes.Status200OK)]
    public async Task<CustomJsonResult<IEnumerable<UserCourseHistoryDetailDto>>> GetByCourse(int courseId, CancellationToken ct)
    {
        var result = await _userCourseHistoryService.GetByCourseAsync(courseId, ct);
        return CustomJsonResult<IEnumerable<UserCourseHistoryDetailDto>>.SuccessResult(result);
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(typeof(CustomJsonResult<string>), StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(CustomJsonResult<string>), StatusCodes.Status404NotFound)]
    public async Task<CustomJsonResult<string>> DeleteUserCourseHistory(int id, CancellationToken ct)
    {
        var deleted = await _userCourseHistoryService.DeleteAsync(id, ct);
        return deleted
            ? CustomJsonResult<string>.NoContent()
            : CustomJsonResult<string>.NotFound($"User course history with ID {id} not found");
    }
}
