using CSRedis;
using DotNetCore.Authentication.JwtBearer.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DotNetCore.Authentication.JwtBearer
{
    public class RedisStore : StoreBase, ITokenStore
    {
        private readonly JwtOptions _options;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public RedisStore(IOptions<JwtOptions> options,
            IHttpContextAccessor httpContextAccessor)
            : base(options, httpContextAccessor)
        {
            _options = options.Value;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<bool> AddTokenAsync(AccessToken accessToken, RefreshToken refreshToken)
        {
            var key = await GetAccessTokenCacheKeyAsync(refreshToken.UserClaimIdentitys);

            await RedisHelper.SetAsync(key, accessToken, _options.ExpiresIn);

            return await RedisHelper.SetAsync(await GetRefreshTokenCacheKeyAsync(refreshToken.Token), refreshToken, _options.RefreshExpiresIn); ;
        }

        public async Task<AccessToken> GetAccessTokenAsync()
        {
            var key = await GetAccessTokenCacheKeyAsync();

            return await RedisHelper.GetAsync<AccessToken>(key);
        }

        public async Task<bool> RemoveAccessTokenAsync()
        {
            var key = await GetAccessTokenCacheKeyAsync();

            return await RedisHelper.DelAsync(key) > 0;
        }

        public async Task<bool> AddRefreshTokenAsync(RefreshToken token)
        {
            return await RedisHelper.SetAsync(await GetRefreshTokenCacheKeyAsync(token.Token), token, _options.RefreshExpiresIn); ;
        }

        public async Task<RefreshToken> GetRefreshTokenAsync(string refreshToken = null)
        {
            return await RedisHelper.GetAsync<RefreshToken>(await GetRefreshTokenCacheKeyAsync(refreshToken));
        }

        public async Task<bool> RemoveRefreshTokenAsync(string refreshToken = null)
        {
            return await RedisHelper.DelAsync(await GetRefreshTokenCacheKeyAsync(refreshToken)) > 0;
        }
    }
}
