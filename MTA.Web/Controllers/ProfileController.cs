using System.Net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MTA.Application.DTOs.User;
using MTA.Application.Services;
using MTA.Web.Models;

namespace MTA.Web.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    [Authorize]
    public class ProfileController : ControllerBase
    {
        private readonly IAccountService _accountService;

        public ProfileController(IAccountService accountService)
        {
            _accountService = accountService;
        }

        [HttpGet("[action]")]
        [ProducesResponseType(typeof(CustomJsonResult<CurrentUserDto>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(CustomJsonResult<string>), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(CustomJsonResult<string>), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(CustomJsonResult<string>), (int)HttpStatusCode.Unauthorized)]
        public async Task<ActionResult<CustomJsonResult<CurrentUserDto>>> GetCurrentUser()
        {
            if (!int.TryParse(User.FindFirst("UserId")?.Value, out var userId) || userId == 0)
            {
                return Unauthorized(CustomJsonResult<string>.Failure("User is not authenticated."));
            }

            var user = await _accountService.GetCurrentUserAsync(userId);
            if (user == null)
            {
                return NotFound(CustomJsonResult<string>.Failure("User not found."));
            }

            //return Ok(CustomJsonResult<CurrentUserDto>.SuccessResult(user));
            return Ok(user);
        }

        [HttpPut("[action]")]
        [ProducesResponseType(typeof(CustomJsonResult<CurrentUserDto>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(CustomJsonResult<string>), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(CustomJsonResult<string>), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(CustomJsonResult<string>), (int)HttpStatusCode.Unauthorized)]
        public async Task<ActionResult<CustomJsonResult<CurrentUserDto>>> UpdateCurrentUser([FromForm] UpdateCurrentUserDto updateDto)
        {
            if (!int.TryParse(User.FindFirst("UserId")?.Value, out var userId) || userId == 0)
            {
                return Unauthorized(CustomJsonResult<string>.Failure("User is not authenticated."));
            }

            var updatedUser = await _accountService.UpdateCurrentUserAsync(userId, updateDto);
            if (updatedUser == null)
            {
                return NotFound(CustomJsonResult<string>.Failure("User not found."));
            }

            //return Ok(CustomJsonResult<CurrentUserDto>.SuccessResult(updatedUser));
            return Ok(updatedUser);
        }

        [HttpPost("[action]")]
        [ProducesResponseType(typeof(CustomJsonResult<string>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(CustomJsonResult<string>), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(CustomJsonResult<string>), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(CustomJsonResult<string>), (int)HttpStatusCode.Unauthorized)]
        public async Task<ActionResult<CustomJsonResult<string>>> UploadAvatar([FromForm] IFormFile profileImage)
        {
            if (profileImage == null || profileImage.Length == 0)
            {
                return BadRequest(CustomJsonResult<string>.Failure("Profile image file is required."));
            }

            if (!int.TryParse(User.FindFirst("UserId")?.Value, out var userId) || userId == 0)
            {
                return Unauthorized(CustomJsonResult<string>.Failure("User is not authenticated."));
            }

            var profileImagePath = await _accountService.UploadProfileImageAsync(userId, profileImage);
            if (profileImagePath == null)
            {
                return NotFound(CustomJsonResult<string>.Failure("User not found."));
            }

            //return Ok(CustomJsonResult<string>.SuccessResult(profileImagePath));
            return Ok(profileImagePath);
        }
    }
}
