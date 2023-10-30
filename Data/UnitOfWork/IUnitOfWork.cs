using FindingPets.Data.Repositories.BaseRepositories;
using FindingPets.Data.Repositories.ImplementedRepositories.AuthenUserRepositories;
using FindingPets.Data.Repositories.ImplementedRepositories.PostImagesRepositories;
using FindingPets.Data.Repositories.ImplementedRepositories.PostRepositories;

namespace FindingPets.Data.UnitOfWork
{
    public interface IUnitOfWork : IDisposable
    {
        IAuthenUserRepo AuthenUserRepo { get; }
        IPostRepo PostRepo { get; }
        IPostImagesRepo PostImagesRepo { get; }

        void BeginTransaction();
        int CommitTransaction();
        void RollbackTransaction();
    }
}
