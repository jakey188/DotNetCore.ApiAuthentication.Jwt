using DotNetCore.Authentication.JwtBearer.Entities;
using DotNetCore.Authentication.JwtBearer.Responses;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace DotNetCore.Authentication.JwtBearer
{
    public interface ITokenService
    {
        Task<TokenResponse> CreateTokenAsync(List<UserClaimIdentity> claims);

        Task<TokenResponse> CreateRefreshTokenAsync(string refreshToken = null);

        Task<AccessToken> GetAccessTokenAsync();

        Task<bool> RemoveTokenAsync();
    }
}