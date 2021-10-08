using System;
using System.Collections.Generic;
using System.Text;

namespace DotNetCore.Authentication.JwtBearer.Entities
{
    public class AccessToken
    {
        public string Id { get; set; }

        /// <summary>
        /// 用户id
        /// </summary>
        public string UserId { get; set; }

        /// <summary>
        /// AccessToken
        /// </summary>
        public string Token { get; set; }

        /// <summary>
        /// 过期
        /// </summary>
        public DateTime Expiration { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime Created { get; set; }
    }
}
