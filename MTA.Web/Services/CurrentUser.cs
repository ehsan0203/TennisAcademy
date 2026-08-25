using MTA.Application.Services.Interface;
using System.Security.Claims;

namespace MTA.Web.Services;

public class CurrentUser : ICurrentUser
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUser(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public int? Id
    {
        get
        {
            var value = _httpContextAccessor.HttpContext?.User?.FindFirst("UserId")?.Value;
            return int.TryParse(value, out var id) ? id : null;
        }
    }
}
