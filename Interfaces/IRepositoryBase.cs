using System.Linq.Expressions;

namespace VitrineApi.Interfaces
{
    public interface IRepositoryBase<T> where T : class
    {
        Task<T> IncluirAsync(T entidade);
        Task<T?> BuscarPorIdAsync(int id);
        Task<IEnumerable<T>> ListarAsync(Expression<Func<T, bool>>? filtro = null);
        Task AtualizarAsync(T entidade);
        Task RemoverAsync(T entidade);
    }
}
