using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using System;

namespace Bulliten.API.Tests
{
    public class ConnectionFactory : IDisposable
    {

        private bool _disposedValue = false;

        public BullitenDBContext CreateContextForSQLite()
        {
            var connection = new SqliteConnection("DataSource=:memory:");
            connection.Open();

            DbContextOptions<BullitenDBContext> option = new DbContextOptionsBuilder<BullitenDBContext>()
                .UseSqlite(connection).Options;

            var context = new BullitenDBContext(option);

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
