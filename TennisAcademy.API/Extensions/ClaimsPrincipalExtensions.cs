﻿using System.Security.Claims;

namespace TennisAcademy.API.Extensions
{
    public static class ClaimsPrincipalExtensions
    {
        public static Guid GetUserId(this ClaimsPrincipal user)
        {
            var id = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return Guid.Parse(id!);
        }
    }
}
