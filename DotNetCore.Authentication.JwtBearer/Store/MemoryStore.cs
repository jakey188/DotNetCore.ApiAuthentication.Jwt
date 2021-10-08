using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.Concurrent;
using DotNetCore.Authentication.JwtBearer.Entities;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

namespace DotNetCore.Authentication.JwtBearer
{
    public class MemoryStore : ITokenStore
    {
        private readonly IMemoryCache _cache;
        private readonly JwtOptions _options;

        public MemoryStore(IOptions<JwtOptions> options,
            IMemoryCache cache)
        {
            _cache = cache;
            _options = options.Value;
        }

        public async Task<bool> AddAccessTokenAsync(AccessToken token)
        {
            var entry = _cache.Set(GetAccessTokenCacheKey(token.UserId), token, TimeSpan.FromSeconds(_options.ExpiresIn));

            return await Task.FromResult(entry != null);
        }

        public async Task<AccessToken> GetAccessTokenAsync(string userId)
        {
            if (_cache.TryGetValue(GetAccessTokenCacheKey(userId), out AccessToken token))
            {
                return await Task.FromResult(token);
            }
            return await Task.FromResult<AccessToken>(null);
        }

        public async Task<bool> RemoveAccessTokenAsync(string userId)
        {
            _cache.Remove(GetAccessTokenCacheKey(userId));
            return await Task.FromResult(true);
        }

        public async Task<bool> AddRefreshTokenAsync(RefreshToken token)
        {
            var entry = _cache.Set(GetRefreshTokenCacheKey(token.Token), token, TimeSpan.FromSeconds(_options.RefreshExpiresIn)); ;

            return await Task.FromResult(entry != null);
        }


        public async Task<RefreshToken> GetRefreshTokenAsync(string refreshToken)
        {
            if (_cache.TryGetValue(GetRefreshTokenCacheKey(refreshToken), out RefreshToken token))
            {
                return await Task.FromResult(token);
            }
            return await Task.FromResult<RefreshToken>(null);
        }


        public async Task<bool> RemoveRefreshTokenAsync(string refreshToken)
        {
            _cache.Remove(GetRefreshTokenCacheKey(refreshToken));
            return await Task.FromResult(true);
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
