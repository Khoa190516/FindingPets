using FindingPets.Data.PostgreEntities;
using FindingPets.Data.Repositories.BaseRepositories;
using FindingPets.Data.Repositories.ImplementedRepositories.AuthenUserRepositories;
using FindingPets.Data.Repositories.ImplementedRepositories.PostImagesRepositories;
using FindingPets.Data.Repositories.ImplementedRepositories.PostRepositories;
using Microsoft.EntityFrameworkCore;
using System.Drawing.Text;

namespace FindingPets.Data.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly D8hclhg7mplh6sContext _context;

        public UnitOfWork(D8hclhg7mplh6sContext context)
        {
            _context = context;
        }

        public IPostRepo PostRepo
        {
            get
            {
                if (PostRepo == null)
                {
                    return new PostRepo(_context);
                }
                return PostRepo;
            }
        }

        public IAuthenUserRepo AuthenUserRepo
        {
            get
            {
                 if (AuthenUserRepo == null)
                 {
                    return new AuthenUserRepo(_context);
                 }
                 return AuthenUserRepo;
            }
        }

        public IPostImagesRepo PostImagesRepo
        {

            get
            {
                if (PostImagesRepo == null)
                {
                    return new PostImageRepo(_context);
                }
                return PostImagesRepo;
            }
        }

        public int CommitTransaction()
        {
            var effectedRows = _context.SaveChanges();
            _context.Database.CommitTransaction();
            return effectedRows;
        }

        public void BeginTransaction()
        {
            _context.Database.BeginTransaction();
        }

        public void RollbackTransaction()
        {
            _context.Database.RollbackTransaction();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
