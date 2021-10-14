using DotNetCore.Authentication.JwtBearer.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DotNetCore.Authentication.JwtBearer
{
    public interface ITokenStore
    {
        Task<bool> AddTokenAsync(AccessToken accessToken, RefreshToken refreshToken);
        Task<bool> AddRefreshTokenAsync(RefreshToken token);
        Task<AccessToken> GetAccessTokenAsync();
        Task<RefreshToken> GetRefreshTokenAsync(string refreshToken = null);
        Task<bool> RemoveAccessTokenAsync();
        Task<bool> RemoveRefreshTokenAsync(string refreshToken = null);
    }
}