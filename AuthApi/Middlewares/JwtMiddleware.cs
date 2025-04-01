using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace AuthApi.Middlewares
{
    public class JwtMiddleware
    {
        private readonly RequestDelegate _next;
       

        public JwtMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            if (context.Request.Cookies.ContainsKey("jwt"))
            {
                var token = context.Request.Cookies["jwt"];

                if (!string.IsNullOrEmpty(token))
                {
                    context.Request.Headers.Append("Authorization", "Bearer " + token);
                }
            }

            await _next(context);
        }

    }
}
