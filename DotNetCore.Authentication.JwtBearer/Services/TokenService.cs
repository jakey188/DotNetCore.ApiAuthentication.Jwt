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
using Microsoft.AspNetCore.Http;

namespace DotNetCore.Authentication.JwtBearer
{
    public class TokenService : ITokenService
    {
        private readonly JwtOptions _options;
        private readonly ITokenStore _store;
        private readonly TokenValidationParameters _tokenValidation;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly JsonSerializerSettings setting = new JsonSerializerSettings
        {
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore
        };
        public TokenService(IOptions<JwtOptions> options,
            ITokenStore tokenStore,
            TokenValidationParameters tokenValidation,
            IHttpContextAccessor httpContextAccessor)
        {
            _options = options.Value;
            _store = tokenStore;
            _tokenValidation = tokenValidation;
            _httpContextAccessor = httpContextAccessor;
        }

        /// <summary>
        /// 获取AccessToken
        /// </summary>
        /// <returns></returns>
        public async Task<AccessToken> GetAccessTokenAsync()
        {
            return await _store.GetAccessTokenAsync();
        }

        /// <summary>
        /// 移除当前用户关联Token
        /// </summary>
        /// <returns></returns>
        public async Task<bool> RemoveTokenAsync()
        {
            return await _store.RemoveAccessTokenAsync() && await _store.RemoveRefreshTokenAsync();
        }

        /// <summary>
        /// 获取RefreshToken
        /// </summary>
        /// <returns></returns>
        public async Task<RefreshToken> GetRefreshTokenAsync(string refreshToken = null)
        {
            return await _store.GetRefreshTokenAsync(refreshToken);
        }

        /// <summary>
        /// 创建RefreshToken
        /// </summary>
        /// <param name="refreshToken"></param>
        /// <param name="replaceClaim"></param>
        /// <returns></returns>
        public async Task<TokenResponse> CreateRefreshTokenAsync(string refreshToken = null, Dictionary<string, string> replaceClaim=null)
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

            if (replaceClaim != null && replaceClaim.Count > 0)
            {
                foreach (var data in replaceClaim)
                {
                    var claim = token.UserClaimIdentitys.FirstOrDefault(c => c.Type == data.Key);
                    if (claim != null)
                    {
                        claim.Value = data.Value;
                    }
                }
            }

            return await CreateTokenAsync(token.UserClaimIdentitys);
        }

        /// <summary>
        /// 创建TokenResponse
        /// </summary>
        /// <param name="claims"></param>
        /// <returns></returns>
        public async Task<TokenResponse> CreateTokenAsync(List<UserClaimIdentity> claims)
        {
            var checkResponse = CheckClaims(claims);

            if (checkResponse.IsError) return new TokenResponse(true, checkResponse.ErrorMessage);

            var now = DateTime.UtcNow;

            var refreshToken = Guid.NewGuid().ToString("N");

            var claimList = claims.Where(c => c.IsCacheKey).Select(c => new Claim(AppConst.ClaimCachePrefix + c.Type, c.Value))
                .Union(claims.Where(c => !c.IsCacheKey).Select(c => new Claim(c.Type, c.Value))).ToList();

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

            await AddTokenAsync(claims, tokenResponse, now);

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
        private async Task AddTokenAsync(List<UserClaimIdentity> claimList, TokenResponse response, DateTime now)
        {
            var primaryValue = claimList.FirstOrDefault(c => c.IsPrimaryKey)?.Value;

            var refreshToken = new RefreshToken
            {
                Token = response.RefreshToken,
                ClientId = null,
                Created = now,
                Expiration = now.AddSeconds(_options.RefreshExpiresIn),
                Id = Guid.NewGuid().ToString("N"),
                IsUsed = false,
                IsRevorked = false,
                UserId = primaryValue,
                UserClaimIdentitys = claimList
            };

            var accessToken = new AccessToken
            {
                Token = response.AccessToken,
                Created = now,
                Expiration = now.AddSeconds(_options.ExpiresIn),
                Id = Guid.NewGuid().ToString("N"),
                UserId = primaryValue,
            };

            await _store.AddTokenAsync(accessToken, refreshToken);
        }

        /// <summary>
        /// 检查Claim子项
        /// </summary>
        /// <param name="claims"></param>
        /// <returns></returns>
        private Response CheckClaims(List<UserClaimIdentity> claims)
        {
            if (claims == null || (claims != null && claims.Count == 0))
                return new ErrorResponse($"{nameof(claims)}不能为空");

            if (claims.Count(c => c.IsPrimaryKey) != 1)
                return new ErrorResponse($"{nameof(claims)}只能有一个IsPrimaryKey");

            if (claims.GroupBy(c => c.Type).Where(g => g.Count() > 1).Count() >= 1)
                return new ErrorResponse($"{nameof(claims)}不允许有重复的Type");

            return new OkResponse();
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

            return new TokenResponse(false, "ok");
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
