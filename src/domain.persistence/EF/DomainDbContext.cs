using domain.model;
using Microsoft.EntityFrameworkCore;

namespace domain.persistence.EF
{
    public class DomainDbContext : DbContext
    {
        public DomainDbContext(DbContextOptions<DomainDbContext> options)
            : base(options)
        { }

        public virtual DbSet<DomainEntity> DomainEntities { get; internal set; }
    }
}