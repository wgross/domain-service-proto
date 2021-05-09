
# Domain.Host

The project Domain.Host implememts a webservice allowing access to the domain using HTTP+JSON and GRPC.

## Use SqLite in Memory DB

For Production use:

```csharp
virtual protected void ConfigureDbContext(IServiceCollection services)
{
    services.AddDbContext<DomainDbContext>(opts => opts.UseSqlite(this.Configuration.GetConnectionString("DomainDatabase")));
}

protected virtual void OnShuttingDown()
{
}
```

Configuration in appsettings,json:

```json
"ConnectionStrings": {
  "DomainDatabase": "Data Source=DomainDatabase.db"
}
```

For tests Startup is overwritten:

```csharp
// keep at least a sqllite instance in memory to avoid resetting the DB al the time
public InMemoryDbContextOptionsBuilder IsMemoryDbConnectionFactory { get; private set; }

protected override void ConfigureDbContext(IServiceCollection services)
{
    this.IsMemoryDbConnectionFactory = new InMemoryDbContextOptionsBuilder();
    services.AddDbContext<DomainDbContext>(opts => this.IsMemoryDbConnectionFactory.CreateOptions(opts));
}

protected override void OnShuttingDown()
{
    // singletons added as a specific instance won't be disposed by the framework
    // therefore dispose manually and hold singleton as a startup class member
    this.IsMemoryDbConnectionFactory?.Dispose();
    this.IsMemoryDbConnectionFactory = null;

    base.OnShuttingDown();
}
```

## Heath check for service and DbContext

See: https://docs.microsoft.com/en-us/aspnet/core/host-and-deploy/health-checks?view=aspnetcore-5.0

Unhealty status can be provoked by moving the '''DomainDatabase.db'''.
