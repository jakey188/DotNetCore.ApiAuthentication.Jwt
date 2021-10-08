using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using DotNetCore.Authentication.JwtBearer.Entities;
using DotNetCore.Authentication.JwtBearer.Responses;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace DotNetCore.Authentication.JwtBearer
{
    public class TokenService : ITokenService
    {
        private readonly JwtOptions _options;
        private readonly ITokenStore _store;
        private readonly TokenValidationParameters _tokenValidation;
        private readonly JsonSerializerSettings setting = new JsonSerializerSettings
        {
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore
        };
        public TokenService(IOptions<JwtOptions> options,
            ITokenStore tokenStore,
            TokenValidationParameters tokenValidation)
        {
            _options = options.Value;
            _store = tokenStore;
            _tokenValidation = tokenValidation;
        }

        /// <summary>
        /// 获取AccessToken
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<AccessToken> GetAccessTokenAsync(string userId)
        {
            return await _store.GetAccessTokenAsync(userId);
        }

        /// <summary>
        /// 移除当前用户关联Token
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="refreshToken"></param>
        /// <returns></returns>
        public async Task<bool> RemoveTokenAsync(string userId,string refreshToken)
        {
            return await _store.RemoveAccessTokenAsync(userId) && await _store.RemoveRefreshTokenAsync(refreshToken);
        }

        /// <summary>
        /// 创建RefreshToken
        /// </summary>
        /// <param name="refreshToken"></param>
        /// <returns></returns>
        public async Task<TokenResponse> CreateRefreshTokenAsync(string refreshToken)
        {
            var token = await _store.GetRefreshTokenAsync(refreshToken);

            var result = CheckRefreshToken(token);

            if (result.IsError) return result;

            if (_options.RefreshTokenUseLimit)
            {
                await _store.RemoveRefreshTokenAsync(refreshToken);
            }
            else
            {
                token.IsUsed = true;

                await _store.AddRefreshTokenAsync(token);
            }

            var data = JsonConvert.DeserializeObject<Dictionary<string, string>>(token.Data, setting);

            var claims = data.Select(c => new Claim(c.Key, c.Value)).ToArray();

            return await CreateTokenAsync(claims, token.UserId);
        }

        /// <summary>
        /// 创建TokenResponse
        /// </summary>
        /// <param name="claims"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<TokenResponse> CreateTokenAsync(Claim[] claims, string userId = null)
        {
            var now = DateTime.UtcNow;

            var refreshToken = Guid.NewGuid().ToString("N");

            var claimList = new List<Claim>(claims);

            if (!string.IsNullOrEmpty(userId) && !claims.Any(c => c.Type == AppConst.ClaimUserId))
                claimList.Add(new Claim(AppConst.ClaimUserId, userId));

            if (!claims.Any(c => c.Type == AppConst.ClaimRefreshToken))
                claimList.Add(new Claim(AppConst.ClaimRefreshToken, refreshToken));

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claimList),
                Expires = now.AddSeconds(_options.ExpiresIn),
                Issuer = _options.Issuer,
                Audience = _options.Audience,
                SigningCredentials = new SigningCredentials(_options.SecurityKey, SecurityAlgorithms.HmacSha256Signature)
            };

            var handler = new JwtSecurityTokenHandler();

            var securityToken = handler.CreateToken(tokenDescriptor);

            var accessToken = handler.WriteToken(securityToken);

            var tokenResponse = await CreateTokenResponseAsync(accessToken, refreshToken, now);

            await AddTokenAsync(claimList, tokenResponse, userId, now);

            return tokenResponse;
        }

        #region private
        private async Task<TokenResponse> CreateTokenResponseAsync(string accessToken, string refreshToken, DateTime now)
        {
            var expiresIn = new DateTimeOffset(now).AddSeconds(_options.ExpiresIn).ToUnixTimeSeconds();

            var refreshExpiresIn = new DateTimeOffset(now).AddSeconds(_options.RefreshExpiresIn).ToUnixTimeSeconds();

            var token = new TokenResponse(accessToken, expiresIn, refreshToken, refreshExpiresIn);

            return await Task.FromResult(token);
        }

        /// <summary>
        /// 持久化Token
        /// </summary>
        /// <param name="claimList"></param>
        /// <param name="response"></param>
        /// <param name="userId"></param>
        /// <param name="now"></param>
        /// <returns></returns>
        private async Task AddTokenAsync(List<Claim> claimList, TokenResponse response, string userId, DateTime now)
        {
            var data = claimList.ToDictionary(key => key.Type, value => value.Value);

            var refreshToken = new RefreshToken
            {
                Token = response.RefreshToken,
                ClientId = null,
                Created = now,
                Expiration = now.AddSeconds(_options.RefreshExpiresIn),
                Id = Guid.NewGuid().ToString("N"),
                IsUsed = false,
                IsRevorked = false,
                UserId = userId,
                Data = JsonConvert.SerializeObject(data, setting)
            };

            var accessToken = new AccessToken
            {
                Token = response.AccessToken,
                Created = now,
                Expiration = now.AddSeconds(_options.ExpiresIn),
                Id = Guid.NewGuid().ToString("N"),
                UserId = userId,
            };

            await _store.AddRefreshTokenAsync(refreshToken);
            await _store.AddAccessTokenAsync(accessToken);
        }

        /// <summary>
        /// 检验Token的有效性
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        private TokenResponse CheckRefreshToken(RefreshToken token)
        {
            if (token == null)
                return new TokenResponse(true, "refresh token 不存在");

            if (DateTime.UtcNow > token.Expiration)
                return new TokenResponse(true, "refreshToken 过期");

            if (token.IsUsed && _options.RefreshTokenUseLimit)
                return new TokenResponse(true, "refresh token已使用");

            if (token.IsRevorked)
                return new TokenResponse(true, "refresh token已撤销");

            return new TokenResponse(false, "refresh token已撤销");
        }

        /// <summary>
        /// 生成RefreshToken
        /// </summary>
        /// <param name="ipAddress"></param>
        /// <returns></returns>
        private string GenerateRefreshToken()
        {
            using (var rngCryptoServiceProvider = new RNGCryptoServiceProvider())
            {
                var randomBytes = new byte[32];
                rngCryptoServiceProvider.GetBytes(randomBytes);
                return Convert.ToBase64String(randomBytes);
            }
        }
        #endregion
    }
}
