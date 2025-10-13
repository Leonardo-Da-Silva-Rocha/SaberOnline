namespace SaberOnline.Core.Agregrates
{
    public interface IUnitOfWork
    {
        Task<bool> Commit();
    }
}
