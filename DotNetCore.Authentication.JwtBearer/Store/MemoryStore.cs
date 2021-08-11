using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.Concurrent;
using DotNetCore.Authentication.JwtBearer.Entities;
using System.Threading.Tasks;

namespace DotNetCore.Authentication.JwtBearer.Store
{
    public class MemoryStore : ITokenStore
    {
        private ConcurrentDictionary<string, RefreshToken> tokens = new ConcurrentDictionary<string, RefreshToken>();

        public async Task<RefreshToken> GetTokenAsync(string refreshToken)
        {
            if (tokens.TryGetValue(refreshToken, out RefreshToken token))
            {
                return await Task.FromResult(token);
            }
            return null;
        }

        public async Task<bool> UpdateAsync(RefreshToken token)
        {
            if (tokens.TryGetValue(token.Token,out RefreshToken oldToken))
            {
                return tokens.TryUpdate(token.Token, token, oldToken);
            }

            return await Task.FromResult(false);
        }

        public async Task<bool> AddAsync(RefreshToken token)
        {
            if (tokens.TryAdd(token.Token, token))
                return await Task.FromResult(true);

            return await Task.FromResult(false);
        }
    }
}
