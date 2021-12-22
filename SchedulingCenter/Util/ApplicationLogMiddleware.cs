using ApiCore.JsonFilter;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using SchedulingCenter.DTO.Request;
using System;
using System.IO;
using System.Threading.Tasks;

namespace SchedulingCenter.Util
{
    public static class ApplicationLogEx
    {
        public static IApplicationBuilder ApplicationLog(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ApplicationLogMiddleware>();
        }
    }

    public class ApplicationLogMiddleware
    {
        private readonly ILogger<ApplicationLogMiddleware> _logger;
        private readonly IJsonHelper _jsonHelper;
        private readonly RequestDelegate _next;

        public ApplicationLogMiddleware(RequestDelegate next, ILogger<ApplicationLogMiddleware> logger, IJsonHelper jsonHelper)
        {
            _next = next;
            _logger = logger;
            _jsonHelper = jsonHelper;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                context.Request.EnableBuffering();//.EnableRewind();
                using (var reader = new StreamReader(context.Request.Body, encoding: System.Text.Encoding.UTF8))
                {
                     RequestLog requestLog = new RequestLog();
                    var bodyString = await reader.ReadToEndAsync();
                    if (context.Request.QueryString.HasValue)
                    {
                        requestLog.Path = context.Request.Path + "?" + context.Request.QueryString.Value;
                    }
                    else
                    {
                        requestLog.Path = context.Request.Path;
                    }
                    requestLog.Method = context.Request.Method;
                    requestLog.Ip = context.Connection.RemoteIpAddress.ToString();
                    requestLog.Body = bodyString;
                    _logger.LogInformation("任务调度请求管道日志{0}", _jsonHelper.ToJson(requestLog));
                    context.Request.Body.Position = 0;
                }
                 
               
            }
            catch (Exception e)
            {
                context.Request.Body.Position = 0;
                _logger.LogError("ApplicationLogEx.InvokeAsync报错" + e.Message + e.StackTrace);
            }
            await _next(context);
        }
    }
}
