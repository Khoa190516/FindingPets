using FindingPets.Business.JWT;
using FindingPets.Data.Entities;
using FindingPets.Data.Models.PostResponseModel;
using FindingPets.Data.Models.UserModel;
using FindingPets.Data.Repositories.BaseRepositories;
using Microsoft.EntityFrameworkCore;
using System.Collections;

namespace FindingPets.Data.Repositories.ImplementedRepositories.AuthenUserRepositories
{
    public class AuthenUserRepo : BaseRepo<AuthenUser>, IAuthenUserRepo
    {
        public AuthenUserRepo(FindingPetsDbContext context) : base(context)
        {
        }

        public async Task<UserTokenModel> GetAccountByEmail(string email)
        {
            var query = from u in context.AuthenUsers
                        join r in context.UserRoles
                        on u.UserRole equals r.Id
                        where u.Email.ToLower().Trim().Equals(email.ToLower().Trim())
                        select new { u, r };

            var user = await query.Select(selector => new UserTokenModel()
            {
                Id = selector.u.Id,
                Name = selector.u.FullName ?? string.Empty,
                Email = email,
                IsActive = selector.u.IsActive ?? true,
                RoleId = selector.u.UserRole,
                ImageURL = selector.u.ImageUrl??string.Empty,
                Role = selector.r.RoleName
            }).FirstOrDefaultAsync() ?? throw new Exception("Email Not Found");

            return user;
        }

        public async Task<IEnumerable> GetUserWithPosts(string email)
        {
            var result = await context.AuthenUsers
                .Where(x => x.Email.ToLower().Equals(email.ToLower()))
                .Include(a => a.Posts).Select(x => new UserWithPostsModel()
                {
                    Email = x.Email,
                    Id = x.Id,
                    ImageURL = x.ImageUrl ?? string.Empty,
                    Name = x.FullName,
                    Role = x.UserRole == 0 ? "admin" : "customer",
                    RoleId = x.UserRole,
                    Posts = x.Posts.Select(p => new PostView()
                    {
                        Contact = p.Contact,
                        Created = p.Created,
                        Description = p.Description,
                        Id = p.Id,
                        IsBanned = p.IsBanned,
                        IsClosed = p.IsClosed,
                        OwnerId = p.OwnerId,
                        PostImages = p.PostImages.Select(i => new PostImageView()
                        {
                            Id = i.Id,
                            ImageBase64 = i.ImageBase64,
                            PostId = i.PostId,
                        }).ToList()
                    }).ToList()
                }).ToListAsync();

            return result;
        }

        public async Task<bool> IsEmailExist(string email)
        {
            try
            {
                var query = from u in context.AuthenUsers
                            where u.Email.ToLower().Trim().Equals(email.ToLower().Trim())
                            select new { u };
                var account = await query.FirstOrDefaultAsync();
                return account != null;
            }catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<bool> UpdateProfile(UserProfileUpdateModel model, Guid userId)
        {
            var account = await context.AuthenUsers.FindAsync(userId);
            if (account == null) throw new Exception($"User ID: {userId} not found in DB");

            //account.Email = model.Email;
            account.FullName = model.FullName;
            account.Phone = model.Phone;
            account.ImageUrl = model.ImageUrl;

            await Update();

            return true;
        }
    }
}
