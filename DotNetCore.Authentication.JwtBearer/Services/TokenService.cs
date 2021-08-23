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
        /// 创建RefreshToken
        /// </summary>
        /// <param name="refreshToken"></param>
        /// <returns></returns>
        public async Task<TokenResponse> CreateRefreshTokenAsync(string refreshToken)
        {
            var token = await _store.GetTokenAsync(refreshToken);
            if (token == null)
                return new TokenResponse(true, "refresh token 不存在");

            if (DateTime.UtcNow > token.Expiration)
                return new TokenResponse(true, "refresh token 过期");

            if (token.IsUsed && _options.RefreshTokenUseLimit)
                return new TokenResponse(true, "refresh token已使用");

            if (token.IsRevorked)
                return new TokenResponse(true, "refresh token已撤销");

            token.IsUsed = true;

            await _store.UpdateAsync(token);

            var data = JsonConvert.DeserializeObject<Dictionary<string, string>>(token.Data, setting);

            var claims = data.Select(c => new Claim(c.Key, c.Value)).ToArray();

            return await CreateTokenAsync(claims, token.UserId);
        }

        /// <summary>
        /// 创建Token
        /// </summary>
        /// <param name="claims"></param>
        /// <param name="id">用户主键Id</param>
        /// <returns></returns>
        public async Task<TokenResponse> CreateTokenAsync(Claim[] claims, string id = null)
        {
            var now = DateTime.UtcNow;

            var claimList = new List<Claim>(claims);

            var idKey = "id";

            if (!string.IsNullOrEmpty(id) && !claims.Any(c => c.Type == idKey))
                claimList.Add(new Claim(idKey, id));

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

            var tokenResponse = await CreateTokenResponseAsync(accessToken, now);

            await StoreTokenAsync(claimList, tokenResponse, id, now);

            return tokenResponse;
        }

        /// <summary>
        /// 创建TokenResponse
        /// </summary>
        /// <param name="accessToken">accessToken</param>
        /// <param name="now">系统时间</param>
        /// <returns></returns>
        private async Task<TokenResponse> CreateTokenResponseAsync(string accessToken, DateTime now)
        {
            var refreshToken = Guid.NewGuid().ToString("N");

            var expiresIn = new DateTimeOffset(now).AddSeconds(_options.ExpiresIn).ToUnixTimeSeconds();

            var refreshExpiresIn = new DateTimeOffset(now).AddSeconds(_options.RefreshExpiresIn).ToUnixTimeSeconds();

            var token = new TokenResponse(accessToken, expiresIn, refreshToken, refreshExpiresIn);

            return await Task.FromResult(token);
        }

        /// <summary>
        /// 持久化Token
        /// </summary>
        /// <param name="refreshToken"></param>
        /// <param name="userId"></param>
        /// <param name="now"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        private async Task StoreTokenAsync(List<Claim> claimList, TokenResponse response, string userId, DateTime now)
        {
            var data = claimList.ToDictionary(key => key.Type, value => value.Value);

            var token = new RefreshToken
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

            await _store.AddAsync(token);
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
    }
}
