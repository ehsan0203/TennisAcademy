using Microsoft.AspNetCore.Mvc;
using MTA.Application.DTOs.User;
using MTA.Application.Services;

namespace MTA.Web.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class ProfileController : ControllerBase
    {
        private readonly IAccountService _accountService;

        public ProfileController(IAccountService accountService)
        {
            _accountService = accountService;
        }

        [HttpGet]
        public async Task<ActionResult<CurrentUserDto>> GetCurrentUser()
        {
            int userId = int.Parse(User.FindFirst("UserId")?.Value ?? "0");
            if (userId == 0) return Unauthorized();

            var user = await _accountService.GetCurrentUserAsync(userId);
            if (user == null) return NotFound();

            return Ok(user);
        }

        [HttpPut]
        public async Task<ActionResult<CurrentUserDto>> UpdateCurrentUser([FromForm] UpdateCurrentUserDto updateDto)
        {
            int userId = int.Parse(User.FindFirst("UserId")?.Value ?? "0");
            if (userId == 0) return Unauthorized();

            var updatedUser = await _accountService.UpdateCurrentUserAsync(userId, updateDto);
            if (updatedUser == null) return NotFound();

            return Ok(updatedUser);
        }
    }

}
