using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Threading.Tasks;

namespace Lidya.SqlRepository
{
    public class Transaction : ITransaction, IDisposable
    {
        private IDbContextTransaction _dbContextTransaction;
        private readonly DbContext _context;
        public Transaction(DbContext context)
        {
            _context = context;
        }

        public async Task BeginAsync()
        {
            if (_dbContextTransaction != null)
                _dbContextTransaction.Dispose();

            _dbContextTransaction = await _context.Database.BeginTransactionAsync();
        }

        public async Task CommitAsync()
        {
            if (_dbContextTransaction != null)
                await _dbContextTransaction.CommitAsync();
        }

        public async Task RollbackAsync()
        {
            if (_dbContextTransaction != null)
                await _dbContextTransaction.RollbackAsync();
        }

        public void Begin()
        {
            if (_dbContextTransaction != null)
                _dbContextTransaction.Dispose();

            _dbContextTransaction = _context.Database.BeginTransaction();
        }


        public void Commit()
        {
            if (_dbContextTransaction != null)
                _dbContextTransaction.Commit();
        }

        public void Rollback()
        {
            if (_dbContextTransaction != null)
                _dbContextTransaction.Rollback();
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
    }
}
