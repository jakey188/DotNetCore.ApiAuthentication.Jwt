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
        /// <summary>
        /// 退出
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public static async Task<bool> LogoutAsync(this HttpContext context)
        {
            var tokenService = context.Request.HttpContext.RequestServices.GetRequiredService<ITokenService>();

            return await tokenService.RemoveTokenAsync();
        }

        /// <summary>
        /// 获取用户Id
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public static async Task<string> GetUserIdAsync(this HttpContext context)
        {
            return await Task.FromResult(context.GetUserId());
        }

        /// <summary>
        /// 获取用户Id
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public static string GetUserId(this HttpContext context)
        {
            if (!context.User.Identity.IsAuthenticated)
                return string.Empty;

            var userId = context.User.Claims.FirstOrDefault(x => x.Type == AppConst.ClaimUserId)?.Value;

            return userId;
        }

        /// <summary>
        /// 获取Claim子项
        /// </summary>
        /// <param name="context"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static async Task<string> GetClaimValueAsync(this HttpContext context, string type)
        {
            return await Task.FromResult(context.GetClaimValue(type));
        }

        /// <summary>
        /// 获取Claim子项
        /// </summary>
        /// <param name="context"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static string GetClaimValue(this HttpContext context, string type)
        {
            if (!context.User.Identity.IsAuthenticated)
                return string.Empty;

            var value = context.User.Claims.FirstOrDefault(x => x.Type == type)?.Value;

            return value;
        }

        /// <summary>
        /// 获取AccessToken
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public static async Task<string> GetAccessTokenAsync(this HttpContext context)
        {
            return await Task.FromResult(context.GetAccessToken());
        }

        /// <summary>
        /// 获取AccessToken
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public static string GetAccessToken(this HttpContext context)
        {
            var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Replace("Bearer ", "");

            if (string.IsNullOrWhiteSpace(token))
                return string.Empty;

            return token;
        }

        /// <summary>
        /// 获取RefreshToken
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public static async Task<string> GetRefreshTokenAsync(this HttpContext context)
        {
            var refreshToken = context.User.Claims.FirstOrDefault(x => x.Type == AppConst.ClaimRefreshToken)?.Value;

            return await Task.FromResult(refreshToken);
        }
    }
}
