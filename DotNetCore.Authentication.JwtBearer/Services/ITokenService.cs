using DotNetCore.Authentication.JwtBearer.Entities;
using DotNetCore.Authentication.JwtBearer.Responses;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace DotNetCore.Authentication.JwtBearer
{
    public interface ITokenService
    {
        /// <summary>
        /// 创建TokenResponse
        /// </summary>
        /// <param name="claims">jwt的键值</param>
        /// <param name="payload">jwt额外的参数</param>
        /// <returns></returns>
        Task<TokenResponse> CreateTokenAsync(List<UserClaimIdentity> claims, Dictionary<string, object> payload = null);
        /// <summary>
        /// 根据refreshToken创建TokenResponse
        /// </summary>
        /// <param name="refreshToken">refreshToken</param>
        /// <param name="replaceClaim">需要替换的jwt键值,比如账号切换时使用</param>
        /// <param name="payload">jwt额外的参数</param>
        /// <returns></returns>
        Task<TokenResponse> CreateRefreshTokenAsync(string refreshToken = null, Dictionary<string, string> replaceClaim = null, Dictionary<string, object> payload = null);

        Task<AccessToken> GetAccessTokenAsync();

        Task<RefreshToken> GetRefreshTokenAsync(string refreshToken = null);

        Task<bool> RemoveTokenAsync();
    }
}