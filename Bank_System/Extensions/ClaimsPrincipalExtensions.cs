using System.Security.Claims;
using Business_Logic.Exceptions;

namespace Bank_System.Extensions
{
    public static class ClaimsPrincipalExtensions
    {
        // Id of the logged-in user, read from the JWT ("sub" claim)
        public static long GetUserId(this ClaimsPrincipal user)
        {
            var value = user.FindFirst("sub")?.Value;

            return long.TryParse(value, out var id)
                ? id
                : throw new UnauthorizedException("The token does not contain a valid user id.");
        }
    }
}
