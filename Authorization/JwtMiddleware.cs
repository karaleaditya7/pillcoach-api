using Microsoft.AspNetCore.Http;
using OntrackDb.Service;
using System.Linq;
using System.Threading.Tasks;

namespace OntrackDb.Authorization
{
    public class JwtMiddleware
    {
        private readonly RequestDelegate _next;

        public JwtMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context, IUserService userService, IJwtUtils jwtUtils)
        {
            var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last(); ;
            var userName = jwtUtils.ValidateToken(token);
            if (userName != null)
            {
                // attach user to context on successful jwt validation
                context.Items["User"] = userService.GetUserByUserName(userName);
                //context.Items["User"] = null;
            }

            await _next(context);
        }
    }
}
