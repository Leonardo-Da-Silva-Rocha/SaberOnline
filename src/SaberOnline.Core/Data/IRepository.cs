using SaberOnline.Core.Agregrates;

namespace SaberOnline.Core.Data
{
    public interface IRepository<T> : IDisposable where T : IRaizAgregacao
    {
        IUnitOfWork UnitOfWork { get; }
    }
}
