using DotNetCore.Authentication.JwtBearer.Responses;
using System.Security.Claims;
using System.Threading.Tasks;

namespace DotNetCore.Authentication.JwtBearer
{
    public interface ITokenService
    {
        Task<TokenResponse> CreateTokenAsync(Claim[] claims, string id = null);

        Task<TokenResponse> CreateRefreshTokenAsync(string refreshToken);
    }
}