﻿using domain.model;
using domain.persistence.EF;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System.Threading.Tasks;

namespace domain.persistence
{
    public sealed class DomainModel : IDomainModel
    {
        public DomainModel(DomainDbContext dbContext)
        {
            this.DbContext = dbContext;
        }

        internal DomainDbContext DbContext { get; }

        public IDomainEntityRepository Entities => new DomainEntityRepository(this);

        public DatabaseFacade Database => this.DbContext.Database;

        public async Task<int> SaveChanges() => await this.DbContext.SaveChangesAsync();

        public void Dispose() => this.DbContext.Dispose();
    }
}