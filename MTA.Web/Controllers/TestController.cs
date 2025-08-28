using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using MTA.Web.Extensions;

namespace MTA.Web.Controllers;

/// <summary>
/// Test controller for basic functionality verification
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class TestController : ControllerBase
{
    /// <summary>
    /// Simple test endpoint
    /// </summary>
    /// <returns>Test message</returns>
    [HttpGet]
    public ActionResult<string> Get()
    {
        return Ok("MTA Tennis Academy API is working!");
    }

    /// <summary>
    /// Health check endpoint
    /// </summary>
    /// <returns>Health status</returns>
    [HttpGet("health")]
    public ActionResult<object> Health()
    {
        return Ok(new
        {
            Status = "Healthy",
            Timestamp = DateTime.UtcNow,
            Version = "1.0.0"
        });
    }

    /// <summary>
    /// Endpoint that requires authentication (any authenticated user)
    /// </summary>
    /// <returns>User information</returns>
    [HttpGet("user-info")]
    [Authorize]
    public ActionResult<object> GetUserInfo()
    {
        var userInfo = new
        {
            UserId = HttpContext.GetCurrentUserId(),
            Email = HttpContext.GetCurrentUserEmail(),
            Role = HttpContext.GetCurrentUserRole(),
            FullName = HttpContext.GetCurrentUserFullName(),
            SkillLevel = HttpContext.GetCurrentUserSkillLevel(),
            Experience = HttpContext.GetCurrentUserExperience(),
            ImageUrl = HttpContext.GetCurrentUserImageUrl(),
            AccountStatus = HttpContext.GetCurrentUserAccountStatus(),
            IsAuthenticated = HttpContext.IsAuthenticated()
        };

        return Ok(userInfo);
    }

    /// <summary>
    /// Endpoint that requires Admin role
    /// </summary>
    /// <returns>Admin only message</returns>
    [HttpGet("admin-only")]
    [Authorize(Policy = "RoleAdmin")]
    public ActionResult<string> AdminOnly()
    {
        return Ok($"Hello Admin! Your ID is: {HttpContext.GetCurrentUserId()}");
    }

    /// <summary>
    /// Endpoint that requires Student role
    /// </summary>
    /// <returns>Student only message</returns>
    [HttpGet("student-only")]
    [Authorize(Policy = "RoleStudent")]
    public ActionResult<string> StudentOnly()
    {
        return Ok($"Hello Student! Your skill level is: {HttpContext.GetCurrentUserSkillLevel()}");
    }

    /// <summary>
    /// Endpoint that requires either Admin or Moderator role
    /// </summary>
    /// <returns>Admin/Moderator message</returns>
    [HttpGet("admin-moderator")]
    [Authorize(Policy = "RolesAdminModerator")]
    public ActionResult<string> AdminOrModerator()
    {
        var userRole = HttpContext.GetCurrentUserRole();
        return Ok($"Hello {userRole}! You have elevated privileges.");
    }

    /// <summary>
    /// Endpoint that requires Coach role
    /// </summary>
    /// <returns>Coach only message</returns>
    [HttpGet("coach-only")]
    [Authorize(Policy = "RoleCoach")]
    public ActionResult<string> CoachOnly()
    {
        return Ok($"Hello Coach! You can help students with skill level: {HttpContext.GetCurrentUserSkillLevel()}");
    }

    /// <summary>
    /// Endpoint that checks user role programmatically
    /// </summary>
    /// <returns>Role-based response</returns>
    [HttpGet("role-check")]
    [Authorize]
    public ActionResult<object> RoleCheck()
    {
        var response = new
        {
            UserId = HttpContext.GetCurrentUserId(),
            Role = HttpContext.GetCurrentUserRole(),
            IsAdmin = HttpContext.HasRole("Admin"),
            IsStudent = HttpContext.HasRole("Student"),
            IsCoach = HttpContext.HasRole("Coach"),
            HasElevatedPrivileges = HttpContext.HasAnyRole("Admin", "Moderator", "Coach"),
            AllClaims = HttpContext.GetAllUserClaims()
        };

        return Ok(response);
    }
}
