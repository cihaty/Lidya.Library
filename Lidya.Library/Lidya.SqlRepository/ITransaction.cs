using System.Threading.Tasks;

namespace Lidya.SqlRepository
{
    public interface ITransaction
    {
        Task BeginAsync();
        Task CommitAsync();
        Task RollbackAsync();
        void Begin();
        void Commit();
        void Rollback();
        void Dispose();
    }
}
