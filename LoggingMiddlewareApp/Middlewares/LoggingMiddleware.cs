using NLog;
using System.Text;

namespace LoggingMiddlewareApp.Middlewares
{
    public class LoggingMiddleware
    {
        private readonly RequestDelegate _next;

        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        public LoggingMiddleware(RequestDelegate next)
        {
            _next = next;

            // Configure NLog for file logging
            var config = new NLog.Config.LoggingConfiguration();
            var fileTarget = new NLog.Targets.FileTarget("logFile")
            {
                FileName = Path.Combine(AppContext.BaseDirectory, "log.txt"),
                Layout = "${longdate} ${level} ${message:witheventtype} ${callsite}"
            };
            config.AddTarget(fileTarget);
            config.AddRuleForAllLevels(fileTarget);
            LogManager.Configuration = config;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // Log request details
            var logger = LogManager.GetCurrentClassLogger();
            logger.Info($"HTTP Request:  {context.Request.Method}, Path: {context.Request.Path}");


            var injectedRequestStream = new MemoryStream();

            // Optionally log request body (for POST, PUT)
            if (context.Request.HasFormContentType || context.Request.ContentType == "application/json")
            {
                injectedRequestStream = new MemoryStream();

                try
                {
                    var requestLog = string.Empty;

                    using (var bodyReader = new StreamReader(context.Request.Body))
                    {
                        var bodyAsText =  await bodyReader.ReadToEndAsync();
                        if (string.IsNullOrWhiteSpace(bodyAsText) == false)
                        {
                            requestLog += $"Body : {bodyAsText}";
                        }

                        var bytesToWrite = Encoding.UTF8.GetBytes(bodyAsText);
                        injectedRequestStream.Write(bytesToWrite, 0, bytesToWrite.Length);
                        injectedRequestStream.Seek(0, SeekOrigin.Begin);
                        context.Request.Body = injectedRequestStream;
                    }

                    logger.Info(requestLog);

                    await _next.Invoke(context);
                }
                finally
                {
                    injectedRequestStream.Dispose();
                }
            }

            await _next(context);

            // Log response details (optional)
            logger.Info($"HTTP Response: {context.Response.StatusCode}");
        }
    }
}
