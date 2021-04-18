using Domain.Persistence.EF;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace Domain.Host.TestServer
{
    /// <summary>
    /// Provide a modofied starup configuration for the domain host servie during the integ test:
    /// - no persistent database
    /// - http handler is injected from the indentity servers test server
    /// - no SSL required
    /// </summary>
    public class DomainServiceTestHostStartup : Startup
    {
        private readonly Microsoft.AspNetCore.TestHost.TestServer identityTestServer;

        public DomainServiceTestHostStartup(IWebHostEnvironment environment, IConfiguration configuration, Microsoft.AspNetCore.TestHost.TestServer identityTestServer)
            : base(environment, configuration)
        {
            this.identityTestServer = identityTestServer;
        }

        protected override void ConfigureAuthenticationServices(IServiceCollection services)
        {
            services
                .AddAuthentication(configureOptions: c =>
                {
                    c.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                    c.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, configureOptions: c =>
                {
                    c.Authority = this.identityTestServer.BaseAddress.AbsoluteUri;
                    c.BackchannelHttpHandler = this.identityTestServer.CreateHandler();
                    c.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateAudience = false
                    };
                    c.RequireHttpsMetadata = false;
                });
        }

        #region // Create db for each test

        //protected override void ConfigureDbContext(IServiceCollection services)
        //{
        //    string createConnectionString(Guid instanceId, string path) => $@"Data Source={path}\DomainDatabaseTest.{instanceId}.db";

        //    var connectionString = createConnectionString(
        //        instanceId: Guid.NewGuid(),
        //        path: Path.GetDirectoryName(typeof(DomainServiceTestHost).GetTypeInfo().Assembly.Location));

        //    services.AddDbContext<DomainDbContext>(opts => opts.UseSqlite(connectionString));
        //}

        #endregion // Create db for each test

        #region In Memory Sqlite instance

        /// <summary>
        /// Singleton of the Db Configuration singleton of the memory sqllite instance.
        /// It holds the single db connections which keeps the in memory db alive
        /// </summary>
        public InMemoryDbContextOptionsBuilder IsMemoryDbConnectionFactory { get; private set; }

        protected override void ConfigureDbContext(IServiceCollection services)
        {
            this.IsMemoryDbConnectionFactory = new InMemoryDbContextOptionsBuilder();
            services.AddDbContext<DomainDbContext>(opts => this.IsMemoryDbConnectionFactory.CreateOptions(opts));
        }

        protected override void OnShuttingDown()
        {
            // singletons added as a specific instance won't be disposed by the framework
            // therefore dispose manually and hold sigleton as a startup class member
            this.IsMemoryDbConnectionFactory?.Dispose();
            this.IsMemoryDbConnectionFactory = null;

            base.OnShuttingDown();
        }

        #endregion In Memory Sqlite instance
    }
}