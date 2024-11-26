using ProdajnikWeb.Data;
using ProdajnikWeb.Data.Initializer;

namespace ProdajnikWeb.Middleware
{
    public class DbInitializerMiddleware(RequestDelegate next)
    {
        private readonly RequestDelegate _next = next;

        public Task Invoke(HttpContext context, ProdajnikContext dbContext)
        {
            if (!(context.Session.Keys.Contains("starting")))
            {
                DbUserInitializer.Initialize(context);
                context.Session.SetString("starting", "Yes");
            }

            return _next.Invoke(context);
        }
    }
    public static class DbInitializerExtensions
    {
        public static IApplicationBuilder UseDbInitializer(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<DbInitializerMiddleware>();
        }
    }
}