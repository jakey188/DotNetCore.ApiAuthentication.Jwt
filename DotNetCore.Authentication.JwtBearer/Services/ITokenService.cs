using DotNetCore.Authentication.JwtBearer.Entities;
using DotNetCore.Authentication.JwtBearer.Responses;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace DotNetCore.Authentication.JwtBearer
{
    public interface ITokenService
    {
        Task<TokenResponse> CreateTokenAsync(List<UserClaimIdentity> claims, List<ClaimPayload> payload = null);

        Task<TokenResponse> CreateRefreshTokenAsync(string refreshToken = null, Dictionary<string, string> replaceClaim = null);

        Task<AccessToken> GetAccessTokenAsync();

        Task<RefreshToken> GetRefreshTokenAsync(string refreshToken = null);

        Task<bool> RemoveTokenAsync();
    }
}