using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace Domain.Host.Hosting
{
    public class LoggingJwtBearerEvents : JwtBearerEvents
    {
        private readonly ILogger<LoggingJwtBearerEvents> logger;

        public LoggingJwtBearerEvents(ILogger<LoggingJwtBearerEvents> logger)
        {
            this.logger = logger;
        }

        public override Task TokenValidated(TokenValidatedContext context)
        {
            Log.TokenValidated(this.logger, context.Principal.Identity.Name, null);
            return base.TokenValidated(context);
        }

        public override Task Forbidden(ForbiddenContext context)
        {
            Log.AccessForbidden(this.logger, null);
            return base.Forbidden(context);
        }

        public override Task AuthenticationFailed(AuthenticationFailedContext context)
        {
            Log.AuthenticationFailed(this.logger, context.Principal?.Identity?.Name, context.Exception);
            return base.AuthenticationFailed(context);
        }

        private class Log
        {
            public static Action<ILogger, string, Exception> TokenValidated = LoggerMessage.Define<string>(
                 logLevel: LogLevel.Debug,
                 eventId: new EventId(1, nameof(TokenValidated)),
                 formatString: "User(name='{name}') validated");

            public static Action<ILogger, string, Exception> AuthenticationFailed = LoggerMessage.Define<string>(
                 logLevel: LogLevel.Error,
                 eventId: new EventId(2, nameof(AuthenticationFailed)),
                 formatString: "User(name='{name}') authentication failed");

            public static Action<ILogger, Exception> AccessForbidden = LoggerMessage.Define(
                 logLevel: LogLevel.Warning,
                 eventId: new EventId(3, nameof(AccessForbidden)),
                 formatString: "Access to api rejected:");
        }
    }
}