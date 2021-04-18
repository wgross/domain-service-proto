using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using System;
using System.Data.Common;

namespace Domain.Persistence.EF
{
    /// <summary>
    /// Factory to create/reuse a SqLite database connection in memory
    /// https://www.meziantou.net/testing-ef-core-in-memory-using-sqlite.htm
    /// </summary>
    /// <remarks>
    /// As is understand the single connection is reused by EF core anyway.
    /// If this isn't viable and more connections are required the option ?cache=shared might be a viable solutions
    /// or give the in memory DB a name: https://sqlite.org/inmemorydb.html
    /// </remarks>
    public sealed class InMemoryDbContextOptionsBuilder : IDisposable
    {
        private DbConnection _connection;

        public DbContextOptions CreateOptions(DbContextOptionsBuilder opts)
        {
            if (_connection is null)
                this.InitializeDbConnection();

            return opts.UseSqlite(_connection).Options;
        }

        private void InitializeDbConnection()
        {
            _connection = new SqliteConnection("DataSource=:memory:");
            _connection.Open();
        }

        #region IDisposable

        public void Dispose()
        {
            _connection?.Dispose();
            _connection = null;
        }

        #endregion IDisposable
    }
}