using System;
using System.Collections.Generic;
using System.Text;

namespace DotNetCore.Authentication.JwtBearer.Responses
{
    public class TokenResponse : Response
    {
        public TokenResponse(bool isError, string message) : base(isError, message)
        { 
        }
        public TokenResponse(string accessToken, long expiresIn, string refreshToken, long refreshExpiresIn) : base(false, null)
        {
            AccessToken = accessToken;
            TokenType = "Bearer";
            ExpiresIn = expiresIn;
            RefreshToken = refreshToken;
            RefreshExpiresIn = refreshExpiresIn;
        }
        public string AccessToken { get; }

        public string TokenType { get; }

        public string RefreshToken { get; }

        public long ExpiresIn { get; set; }

        public long RefreshExpiresIn { get; }
    }
}
