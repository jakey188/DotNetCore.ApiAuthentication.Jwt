using System;
using System.Collections.Generic;
using System.Text;

namespace DotNetCore.Authentication.JwtBearer.Entities
{
    public class RefreshToken
    {
        /// <summary>
        /// 主键
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// 客户端编号
        /// </summary>
        public string ClientId { get; set; }

        /// <summary>
        /// 用户id
        /// </summary>
        public string UserId { get; set; }

        /// <summary>
        /// RefreshToken
        /// </summary>
        public string Token { get; set; }

        /// <summary>
        /// 过期
        /// </summary>
        public DateTime Expiration { get; set; }

        /// <summary>
        /// 是否出于安全原因已将其撤销
        /// </summary>
        public bool IsRevorked { get; set; }

        /// <summary>
        /// 是否使用
        /// </summary>
        public bool IsUsed { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime Created { get; set; }

        /// <summary>
        /// Claim数据
        /// </summary>
        public List<UserClaimIdentity> UserClaimIdentitys { get; set; }

        /// <summary>
        /// 其他Claim数据
        /// </summary>
        public List<ClaimPayload> ClaimPayload { get; set; }
    }
}
