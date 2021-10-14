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

        
        protected async Task<string> GetRefreshTokenCacheKeyAsync(string refreshToken=null)
        {
            if(string.IsNullOrWhiteSpace(refreshToken))
                refreshToken = await _httpContextAccessor.HttpContext.GetRefreshTokenAsync();
            return $"{_options.CachePrefix}:Login:RefreshToken:{refreshToken}";
        }

        protected async Task<string> GetAccessTokenCacheKeyAsync(List<UserClaimIdentity> claimList=null)
        {
            var claimIdentity = new SortedDictionary<string, string>();

            if (claimList == null)
                claimIdentity = await _httpContextAccessor.HttpContext.GetClaimIdentityAsync();
            else
                claimIdentity = GetSortedDictionary(claimList);

            var cacheKeys = string.Empty;

            foreach (var claim in claimIdentity)
            {
                cacheKeys += $"{claim.Key}:{claim.Value}:";
            }
            cacheKeys = cacheKeys.TrimEnd(':');

            return $"{_options.CachePrefix}:Login:AccessToken:{cacheKeys}";
        }

        private SortedDictionary<string, string> GetSortedDictionary(List<UserClaimIdentity> claimList)
        {
            var dic = new SortedDictionary<string, string>();
            foreach (var claim in claimList.Where(c=>c.IsCacheKey))
            {
                if (!dic.ContainsKey(claim.Type))
                    dic.Add(claim.Type, claim.Value);
            }
            return dic;
        }

    }
}
