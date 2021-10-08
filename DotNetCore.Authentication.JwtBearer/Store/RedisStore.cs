using CSRedis;
using DotNetCore.Authentication.JwtBearer.Entities;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DotNetCore.Authentication.JwtBearer
{
    public class RedisStore : ITokenStore
    {
        private readonly JwtOptions _options;
        public RedisStore(IOptions<JwtOptions> options)
        {
            _options = options.Value;
        }

        public async Task<bool> AddAccessTokenAsync(AccessToken token)
        {
            return await RedisHelper.SetAsync(GetAccessTokenCacheKey(token.UserId), token, _options.ExpiresIn);
        }

        public async Task<AccessToken> GetAccessTokenAsync(string userId)
        {
            return await RedisHelper.GetAsync<AccessToken>(GetAccessTokenCacheKey(userId));
        }

        public async Task<bool> RemoveAccessTokenAsync(string userId)
        {
            return await RedisHelper.DelAsync(GetAccessTokenCacheKey(userId)) > 0;
        }

        public async Task<bool> AddRefreshTokenAsync(RefreshToken token)
        {
            return await RedisHelper.SetAsync(GetRefreshTokenCacheKey(token.Token), token, _options.RefreshExpiresIn); ;
        }


        public async Task<RefreshToken> GetRefreshTokenAsync(string refreshToken)
        {
            return await RedisHelper.GetAsync<RefreshToken>(GetRefreshTokenCacheKey(refreshToken));
        }


        public async Task<bool> RemoveRefreshTokenAsync(string refreshToken)
        {
            return await RedisHelper.DelAsync(GetRefreshTokenCacheKey(refreshToken)) > 0;
        }

        private string GetRefreshTokenCacheKey(string refreshToken)
        {
            return $"{_options.CachePrefix}:Login:RefreshToken:{refreshToken}";
        }

        private string GetAccessTokenCacheKey(string userId)
        {
            return $"{_options.CachePrefix}:Login:AccessToken:{userId}";
        }
    }
}
