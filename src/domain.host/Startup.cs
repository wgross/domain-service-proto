using domain.contract;
using domain.model;
using domain.persistence;
using domain.persistence.EF;
using domain.service;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace host
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
            services.AddScoped<IDomainModel, DomainModel>();
            services.AddScoped<IDomainService, DomainService>();
            services.AddDbContext<DomainDbContext>(opts => opts.UseSqlite(this.Configuration.GetConnectionString("DomainDatabase")));
            services.AddControllers();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();
            app.UseEndpoints(c => c.MapControllers());
        }
    }
}