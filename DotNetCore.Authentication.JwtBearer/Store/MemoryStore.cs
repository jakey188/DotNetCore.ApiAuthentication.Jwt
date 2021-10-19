using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.Concurrent;
using DotNetCore.Authentication.JwtBearer.Entities;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using System.Linq;
using Microsoft.AspNetCore.Http;

namespace DotNetCore.Authentication.JwtBearer
{
    public class MemoryStore : StoreBase, ITokenStore
    {
        private readonly IMemoryCache _cache;
        private readonly JwtOptions _options;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public MemoryStore(IOptions<JwtOptions> options,
            IMemoryCache cache,
            IHttpContextAccessor httpContextAccessor)
            : base(options, httpContextAccessor)
        {
            _cache = cache;
            _options = options.Value;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<bool> AddTokenAsync(AccessToken accessToken, RefreshToken refreshToken)
        {
            var key = await GetAccessTokenCacheKeyAsync(refreshToken.UserClaimIdentitys);

            var accessTokenEntry = _cache.Set(key, accessToken, TimeSpan.FromSeconds(_options.ExpiresIn));

            var refreshTokenEntry = _cache.Set(await GetRefreshTokenCacheKeyAsync(refreshToken.Token), refreshToken, TimeSpan.FromSeconds(_options.RefreshExpiresIn)); ;

            return await Task.FromResult(accessTokenEntry != null && refreshTokenEntry!=null);
        }

        public async Task<AccessToken> GetAccessTokenAsync()
        {
            var key = await GetAccessTokenCacheKeyAsync();

            if (_cache.TryGetValue(key, out AccessToken token))
            {
                return await Task.FromResult(token);
            }
            return await Task.FromResult<AccessToken>(null);
        }

        public async Task<bool> RemoveAccessTokenAsync(List<UserClaimIdentity> claimList = null)
        {
            var key = await GetAccessTokenCacheKeyAsync(claimList);
            _cache.Remove(key);
            return await Task.FromResult(true);
        }

        public async Task<bool> AddRefreshTokenAsync(RefreshToken token)
        {
            var entry = _cache.Set(await GetRefreshTokenCacheKeyAsync(token.Token), token, TimeSpan.FromSeconds(_options.RefreshExpiresIn)); ;

            return await Task.FromResult(entry != null);
        }


        public async Task<RefreshToken> GetRefreshTokenAsync(string refreshToken=null)
        {
            if (_cache.TryGetValue(await GetRefreshTokenCacheKeyAsync(refreshToken), out RefreshToken token))
            {
                return await Task.FromResult(token);
            }
            return await Task.FromResult<RefreshToken>(null);
        }


        public async Task<bool> RemoveRefreshTokenAsync(string refreshToken = null)
        {
            _cache.Remove(await GetRefreshTokenCacheKeyAsync(refreshToken));
            return await Task.FromResult(true);
        }
    }
}
