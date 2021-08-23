using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using DotNetCore.Authentication.JwtBearer.Store;
using System;
using System.Collections.Generic;
using System.Text;
using CSRedis;

namespace DotNetCore.Authentication.JwtBearer
{
    public static class JwtBearerExtension
    {
        public static AuthenticationBuilder AddJwtAuthentication(this IServiceCollection services, Action<JwtOptions> action)
        {
            if (action == null) throw new ArgumentNullException($"{nameof(action)}不能为空");

            services.Configure(action);

            var options = services.BuildServiceProvider().GetRequiredService<IOptions<JwtOptions>>().Value;

            return AddServices(services, options);
        }

        public static AuthenticationBuilder AddJwtAuthentication(this IServiceCollection services)
        {
            Action<JwtOptions> action = x => new JwtOptions();

            services.Configure(action);

            var options = services.BuildServiceProvider().GetRequiredService<IOptions<JwtOptions>>().Value;

            return AddServices(services, options);
        }

        private static AuthenticationBuilder AddServices(IServiceCollection services,JwtOptions options)
        {
            services.AddSingleton<ITokenService, TokenService>();
            services.AddSingleton<ITokenStore, MemoryStore>();

            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,//是否验证SecurityKey
                IssuerSigningKey = options.SecurityKey,
                ValidateIssuer = false,//是否验证Issuer
                ValidIssuer = options.Issuer,
                ValidateAudience = false,//是否验证Audience
                ValidAudience = options.Audience,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.FromMinutes(1) //TimeSpan.Zero
            };

            services.AddSingleton(tokenValidationParameters);

            return services.AddAuthentication(x =>
                {
                    x.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                 .AddJwtBearer(opt =>
                 {
                     opt.RequireHttpsMetadata = false;
                     //opt.SaveToken = true;
                     opt.TokenValidationParameters = tokenValidationParameters;
                 });
        }


        public static AuthenticationBuilder AddRedisStore(this AuthenticationBuilder build, string redisConnection)
        {
            var redis = new CSRedisClient(redisConnection);

            RedisHelper.Initialization(redis);

            build.Services.AddSingleton(c =>
            {
                return redis;
            });

            build.Services.AddSingleton<ITokenStore, RedisStore>();

            return build;
        }
    }
}
