using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace APIProject
{
    public class RequestTimer
    {
        private readonly RequestDelegate _next;

        public RequestTimer(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
                sw.Start();
                await _next.Invoke(context);

                sw.Stop();
                System.Diagnostics.Debug.WriteLine(string.Format("BASIC.RequestTimer: {1} - Request Time: {0} seconds", sw.Elapsed.TotalSeconds, context.Request.Path.ToString()));
            }
            catch { }
        }
    }

    public static class RequestTimerExtenstions
    {
        public static IApplicationBuilder UseRequestTimer(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<RequestTimer>();
        }
    }
}
