using DotNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JwtDemo
{
    public class AllowApiAuthorizationFilter : IAuthorizationFilter
    {
        public async Task<bool> AllowAsync(HttpContext context)
        {
            var endpoints = context.GetEndpoint();

            var attb = endpoints?.Metadata.FirstOrDefault(c => c is AllowApiAttribute) as AllowApiAttribute;

            return await Task.FromResult(endpoints?.Metadata.GetMetadata<AllowApiAttribute>() != null);
        }
    }
}
