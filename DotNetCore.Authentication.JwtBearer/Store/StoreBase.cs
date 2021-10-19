using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotNetCore.Authentication.JwtBearer
{
    public abstract class StoreBase
    {
        private readonly JwtOptions _options;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public StoreBase(IOptions<JwtOptions> options,
            IHttpContextAccessor httpContextAccessor)
        {
            _options = options.Value;
            _httpContextAccessor = httpContextAccessor;
        }

        /// <summary>
        /// 获取RefreshToken缓存Key
        /// </summary>
        /// <param name="refreshToken"></param>
        /// <returns></returns>
        protected async Task<string> GetRefreshTokenCacheKeyAsync(string refreshToken=null)
        {
            if(string.IsNullOrWhiteSpace(refreshToken))
                refreshToken = await _httpContextAccessor.HttpContext.GetRefreshTokenAsync();
            return $"{_options.CachePrefix}:Login:RefreshToken:{refreshToken}";
        }

        /// <summary>
        /// 获取AccessToken缓存Key
        /// </summary>
        /// <param name="claimList"></param>
        /// <returns></returns>
        protected async Task<string> GetAccessTokenCacheKeyAsync(List<UserClaimIdentity> claimList=null)
        {
            var cacheKey = string.Empty;

            var uid = string.Empty;

            if (claimList == null)
            {
                cacheKey = await _httpContextAccessor.HttpContext.GetClaimValueAsync(AppConst.ClaimAccessTokenCacheKey);

                uid = await _httpContextAccessor.HttpContext.GetUserIdAsync();
            }
            else
            {
                cacheKey = claimList.GetClaimCacheKey();

                uid = claimList.FirstOrDefault(c => c.IsPrimaryKey).Value; 
            }

            return $"{_options.CachePrefix}:Login:AccessToken:{uid}:{cacheKey}";
        }
    }
}
