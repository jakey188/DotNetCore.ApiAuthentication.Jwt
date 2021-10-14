using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Text;

namespace DotNetCore.Authentication.JwtBearer
{
    public class JwtOptions
    {
        public JwtOptions()
        {
            var secret = "ABCDEFGHIJKLMNOPQRSTUVWXYZ123";
            SignKey = secret;
            ExpiresIn = 1800;
            RefreshExpiresIn = 3600 * 24 * 15;
            Issuer = null;
            Audience = null;
            SecurityKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(secret));
            RefreshTokenUseLimit = true;
            CachePrefix = "DotNetCore";
        }

        public string CachePrefix { get; set; }

        /// <summary>
        /// 签名Key
        /// </summary>
        public string SignKey { get; set; }

        /// <summary>
        /// AccessToken过期Unix时间戳(秒),默认30分钟
        /// </summary>
        public int ExpiresIn { get; set; }

        /// <summary>
        /// RefreshToken过期Unix时间戳(秒),默认15天
        /// </summary>
        public int RefreshExpiresIn { get; set; }

        /// <summary>
        /// 颁发给谁
        /// </summary>
        public string Issuer { get; set; }

        /// <summary>
        /// 谁颁发的
        /// </summary>
        public string Audience { get; set; }

        /// <summary>
        /// signing credentials
        /// </summary>
        public SecurityKey SecurityKey { get; private set; }

        /// <summary>
        /// RefreshToken使用限制,默认true
        /// </summary>
        public bool RefreshTokenUseLimit { get; set; }
    }
}
