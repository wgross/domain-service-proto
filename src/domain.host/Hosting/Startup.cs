using Domain.Contract;
using Domain.Host.GrpcServices;
using Domain.Host.Hosting;
using Domain.Model;
using Domain.Persistence;
using Domain.Persistence.EF;
using Domain.Service;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

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
            // domain model and persistence
            services.AddScoped<IDomainModel, DomainModel>();
            services.AddScoped<IDomainService, DomainService>();
            services.AddDbContext<DomainDbContext>(opts => opts.UseSqlite(this.Configuration.GetConnectionString("DomainDatabase")));

            // web api
            services.AddControllers();
            services.AddHostedService<MigrateDatabaseService>();

            // open api
            services.AddSwaggerDocument();

            // grpc
            services.AddGrpc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            // allow requests from domain.ui
            // if this fails move up in configuration order
            app.UseCors(policy => policy
               .AllowAnyOrigin()//.WithOrigins("http://localhost:6000", "https://localhost:6001")
               .AllowAnyMethod()
               .AllowAnyHeader());

            // web api
            app.UseRouting();
            app.UseEndpoints(c =>
            {
                // web api
                c.MapControllers();

                // grpc
                c.MapGrpcService<GrpcDomainService>();
            });

            // open api
            app.UseOpenApi();
            app.UseSwaggerUi3();
        }
    }
}