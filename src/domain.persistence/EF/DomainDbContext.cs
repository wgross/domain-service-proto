using Domain.Model;
using Microsoft.EntityFrameworkCore;

namespace Domain.Persistence.EF
{
    public class DomainDbContext : DbContext
    {
        public DomainDbContext(DbContextOptions<DomainDbContext> options)
            : base(options)
        { }

        public virtual DbSet<DomainEntity> DomainEntities { get; internal set; }
    }
}