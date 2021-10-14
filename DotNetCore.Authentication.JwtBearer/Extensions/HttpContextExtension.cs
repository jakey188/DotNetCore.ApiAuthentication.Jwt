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
        public static async Task<bool> SignOut(this HttpContext context)
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
            if (!context.User.Identity.IsAuthenticated)
                return await Task.FromResult(string.Empty);

            var userId = context.User.Claims.FirstOrDefault(x => x.Type == AppConst.ClaimCachePrefix + AppConst.ClaimUserId)?.Value;

            return await Task.FromResult(userId);
        }

        /// <summary>
        /// 获取Claim子项
        /// </summary>
        /// <param name="context"></param>
        /// <param name="type"></param>
        /// <param name="hasClaimCachePrefix"></param>
        /// <returns></returns>
        public static async Task<string> GetClaimsAsync(this HttpContext context, string type, bool hasClaimCachePrefix = false)
        {
            if (!context.User.Identity.IsAuthenticated)
                return await Task.FromResult(string.Empty);

            if (type.StartsWith(AppConst.ClaimCachePrefix)) hasClaimCachePrefix = false;

            var value = context.User.Claims.FirstOrDefault(x => x.Type == (hasClaimCachePrefix ? AppConst.ClaimCachePrefix + type : type))?.Value;

            if (string.IsNullOrWhiteSpace(value))
                value = context.User.Claims.FirstOrDefault(x => x.Type == AppConst.ClaimCachePrefix + type)?.Value;

            return await Task.FromResult(value);
        }

        /// <summary>
        /// 获取AccessToken
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public static async Task<string> GetAccessTokenAsync(this HttpContext context)
        {
            var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Replace("Bearer ", "");

            if (string.IsNullOrWhiteSpace(token))
                return await Task.FromResult(string.Empty);

            return await Task.FromResult(token);
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

        /// <summary>
        /// 获取用户缓存Key字段
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        internal static async Task<SortedDictionary<string, string>> GetClaimIdentityAsync(this HttpContext context)
        {
            var claimList = context.User.Claims.Where(x => x.Type.StartsWith(AppConst.ClaimCachePrefix)).ToList();

            var dic = new SortedDictionary<string, string>();
            foreach (var claim in claimList)
            {
                if (!dic.ContainsKey(claim.Type))
                    dic.Add(claim.Type.Replace(AppConst.ClaimCachePrefix, ""), claim.Value);
            }

            return await Task.FromResult(dic);
        }
    }
}
