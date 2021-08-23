using CSRedis;
using DotNetCore.Authentication.JwtBearer.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DotNetCore.Authentication.JwtBearer
{
    public class RedisStore : ITokenStore
    {

        public async Task<bool> AddAsync(RefreshToken token)
        {
            var cacheKey = GetCacheKey(token.Token);
            var expiration = (int)(token.Expiration - DateTime.UtcNow).TotalSeconds;
            await RedisHelper.SetAsync(cacheKey, token, expiration);
            return await Task.FromResult(true);
        }

        public async Task<RefreshToken> GetTokenAsync(string refreshToken)
        {
            var cacheKey = GetCacheKey(refreshToken);
            return await RedisHelper.GetAsync<RefreshToken>(cacheKey);
        }

        public async Task<bool> UpdateAsync(RefreshToken token)
        {
            var cacheKey = GetCacheKey(token.Token);
            await RedisHelper.DelAsync(cacheKey);
            return await AddAsync(token);
        }

        private string GetCacheKey(string refreshToken)
        {
            return $"DotNetCore:Jwt:RefreshToken:{refreshToken}";
        }
    }
}
