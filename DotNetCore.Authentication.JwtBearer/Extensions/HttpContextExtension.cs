using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace DotNetCore.Authentication.JwtBearer
{
    public static class HttpContextExtension
    {
        public static async Task<bool> SignOut(this HttpContext context)
        {
            var tokenService = context.Request.HttpContext.RequestServices.GetRequiredService<ITokenService>();

            var userId = await context.GetUserIdAsync();

            var refreshToken = await context.GetRefreshTokenAsync();

            return await tokenService.RemoveTokenAsync(userId, refreshToken);
        }

        internal static async Task<string> GetUserIdAsync(this HttpContext context)
        {
            if (!context.User.Identity.IsAuthenticated)
            {
                return await Task.FromResult(string.Empty);
            }

            var userId = context.User.Claims.FirstOrDefault(x => x.Type == AppConst.ClaimUserId)?.Value;

            return await Task.FromResult(userId);
        }


        internal static async Task<string> GetAccessTokenAsync(this HttpContext context)
        {
            var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Replace("Bearer ", "");

            if (string.IsNullOrWhiteSpace(token))
                return await Task.FromResult(string.Empty);

            return await Task.FromResult(token);
        }

        internal static async Task<string> GetRefreshTokenAsync(this HttpContext context)
        {
            var tokenStr = await GetAccessTokenAsync(context);

            if (string.IsNullOrWhiteSpace(tokenStr)) return tokenStr;

            var tokenHandler = new JwtSecurityTokenHandler();

            var token = tokenHandler.ReadJwtToken(tokenStr);

            var refreshToken = token.Claims.FirstOrDefault(x => x.Type == AppConst.ClaimRefreshToken)?.Value;

            return refreshToken;
        }
    }
}
