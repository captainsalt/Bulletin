using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using System;

namespace Bulletin.API.Tests.Helpers
{
    public class ConnectionFactory : IDisposable
    {

        private bool _disposedValue = false;

        public BulletinDBContext CreateContextForSQLite()
        {
            var connection = new SqliteConnection("DataSource=:memory:");
            connection.Open();

            DbContextOptions<BulletinDBContext> option = new DbContextOptionsBuilder<BulletinDBContext>()
                .UseSqlite(connection).Options;

            var context = new BulletinDBContext(option);

            if (context != null)
            {
                context.Database.EnsureDeleted();
                context.Database.EnsureCreated();
            }

            return context;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                }

                _disposedValue = true;
            }
        }

        public void Dispose() => Dispose(true);
    }
}
