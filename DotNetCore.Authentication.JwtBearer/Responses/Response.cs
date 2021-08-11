using System;
using System.Collections.Generic;
using System.Text;

namespace DotNetCore.Authentication.JwtBearer.Responses
{
    public abstract class Response
    {
        public Response(bool isError,string message)
        {
            IsError = isError;
            ErrorMessage = message;
        }
        public bool IsError { get; } 

        public string ErrorMessage { get;}
    }
}
