using DotNetCore.Authentication.JwtBearer.Entities;
using DotNetCore.Authentication.JwtBearer.Responses;
using System.Security.Claims;
using System.Threading.Tasks;

namespace DotNetCore.Authentication.JwtBearer
{
    public interface ITokenService
    {
        Task<TokenResponse> CreateTokenAsync(Claim[] claims, string userId = null);

        Task<TokenResponse> CreateRefreshTokenAsync(string refreshToken);

        Task<AccessToken> GetAccessTokenAsync(string userId);

        Task<bool> RemoveTokenAsync(string userId, string refreshToken);
    }
}