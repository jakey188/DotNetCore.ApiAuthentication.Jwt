using DotNetCore.Authentication.JwtBearer.Entities;
using System.Threading.Tasks;

namespace DotNetCore.Authentication.JwtBearer
{
    public interface ITokenStore
    {
        Task<bool> AddAccessTokenAsync(AccessToken token);
        Task<bool> AddRefreshTokenAsync(RefreshToken token);
        Task<AccessToken> GetAccessTokenAsync(string userId);
        Task<RefreshToken> GetRefreshTokenAsync(string refreshToken);
        Task<bool> RemoveAccessTokenAsync(string userId);
        Task<bool> RemoveRefreshTokenAsync(string refreshToken);
    }
}