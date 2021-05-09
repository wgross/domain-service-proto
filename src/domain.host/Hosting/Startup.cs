using Domain.Contract;
using Domain.Host.GrpcServices;
using Domain.Host.Hosting;
using Domain.Model;
using Domain.Persistence;
using Domain.Persistence.EF;
using Domain.Service;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NSwag;
using NSwag.Generation.Processors.Security;
using OpenTelemetry.Exporter;
using OpenTelemetry.Logs;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Serilog;
using Serilog.Formatting.Compact;
using Serilog.Sinks.SystemConsole.Themes;

namespace Domain.Host
{
    public class Startup
    {
        public IWebHostEnvironment Environment { get; }

        public IConfiguration Configuration { get; }

        public Startup(IWebHostEnvironment environment, IConfiguration configuration)
        {
            this.Environment = environment;
            this.Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            // reconfigure logging from appsettings
            Log.Logger = new LoggerConfiguration()
                .Enrich.FromLogContext()
                .WriteTo.Console(
                    outputTemplate: "[{Timestamp:HH:mm:ss} {Level}] {SourceContext}{NewLine}{Message:lj}{NewLine}{Exception}{NewLine}",
                    theme: AnsiConsoleTheme.Code)
                .ReadFrom.Configuration(this.Configuration)
                .WriteTo.File(new CompactJsonFormatter(), this.Configuration.GetValue<string>("JsonLogPath"))
                .CreateLogger();

            // open telemetry
            services.AddOpenTelemetryTracing(
                builder =>
                {
                    builder
                        .SetResourceBuilder(ResourceBuilder.CreateDefault().AddService(this.Environment.ApplicationName))
                        .AddAspNetCoreInstrumentation()
                        .AddConsoleExporter(options => options.Targets = ConsoleExporterOutputTargets.Console);
                });

            // domain model and persistence
            services.AddScoped<IDomainModel, DomainModel>();
            services.AddScoped<IDomainService, DomainService>();

            this.ConfigureDbContext(services);

            // web api
            services
                .AddControllers()
                // Add Controllers from this assembly explcitely bacause during test the test assembly would be
                // searched for Controllers without success
                .AddApplicationPart(typeof(Startup).Assembly);

            services.AddHostedService<MigrateDatabaseService>();

            this.ConfigureAuthenticationServices(services);

            // open api documentaion
            services.AddSwaggerDocument(document =>
            {
                // API accepts a JWT bearer token
                document.AddSecurity("JWT", new OpenApiSecurityScheme
                {
                    Type = OpenApiSecuritySchemeType.ApiKey,
                    Name = "Authorization", // header name
                    In = NSwag.OpenApiSecurityApiKeyLocation.Header,
                    Description = "OpenId Jwt with prefix 'Bearer'"
                });
                document.OperationProcessors.Add(new AspNetCoreOperationSecurityScopeProcessor("JWT"));
            });

            // grpc
            services.AddGrpc();
        }

        virtual protected void ConfigureDbContext(IServiceCollection services)
        {
            services.AddDbContext<DomainDbContext>(opts => opts.UseSqlite(this.Configuration.GetConnectionString("DomainDatabase")));
        }

        virtual protected void ConfigureAuthenticationServices(IServiceCollection services)
        {
            // web api authorization with open id bearer tokens
            services
                .AddSingleton<LoggingJwtBearerEvents>()
                // while http runs nicely without specifiying these schemes explicitely grpc fails to authorize the call w/o them.
                .AddAuthentication(configureOptions: opts => this.Configuration.Bind("Authentication", opts))
                .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, opts =>
                {
                    this.Configuration.Bind("Authentication:Bearer", opts);
                    opts.EventsType = typeof(LoggingJwtBearerEvents);
                });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.1
        public void Configure(IApplicationBuilder app, IHostApplicationLifetime appLifeTime)
        {
            if (this.Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            // allow requests from domain.ui
            // (if this fails move up in configuration order)
            app.UseCors(policy => policy
               .AllowAnyOrigin()//.WithOrigins("http://localhost:6000", "https://localhost:6001")
               .AllowAnyMethod()
               .AllowAnyHeader());

            // web api
            app.UseRouting();

            // enable authentication and authorization middleware
            app.UseAuthentication();
            app.UseAuthorization();

            // add routing middleware and enpoints of controllers and GRPC
            app.UseEndpoints(c =>
            {
                c.MapControllers();
                c.MapGrpcService<GrpcDomainService>();
            });

            // open api
            app.UseOpenApi();
            app.UseSwaggerUi3();

            // Register a delegate for graceful shutdown
            appLifeTime.ApplicationStopping.Register(this.OnShuttingDown);
        }

        protected virtual void OnShuttingDown()
        {
        }
    }
}