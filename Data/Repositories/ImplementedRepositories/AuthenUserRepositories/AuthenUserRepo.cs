using FindingPets.Business.JWT;
using FindingPets.Data.Commons;
using FindingPets.Data.Models.PostResponseModel;
using FindingPets.Data.Models.UserModel;
using FindingPets.Data.PostgreEntities;
using FindingPets.Data.Repositories.BaseRepositories;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace FindingPets.Data.Repositories.ImplementedRepositories.AuthenUserRepositories
{
    public class AuthenUserRepo : BaseRepo<Authenuser>, IAuthenUserRepo
    {
        public AuthenUserRepo(D8hclhg7mplh6sContext context) : base(context)
        {
        }

        public async Task<UserTokenModel?> GetAccountByEmail(string email)
        {
            var query = from u in context.Authenusers
                        join r in context.Userroles
                        on u.Userrole equals r.Id
                        where u.Email.ToLower().Trim().Equals(email.ToLower().Trim())
                        select new { u, r };

            var user = await query.Select(selector => new UserTokenModel()
            {
                Id = selector.u.Id,
                Name = selector.u.Fullname ?? string.Empty,
                Email = email,
                IsActive = selector.u.Isactive ?? true,
                RoleId = selector.u.Userrole,
                ImageURL = selector.u.Imageurl??string.Empty,
                Role = selector.r.Rolename,
                Phone = selector.u.Phone,
            }).FirstOrDefaultAsync();

            return user;
        }

        public async Task<UserWithPostsModel?> GetUserWithPosts(string email)
        {
            try
            {
                var result = await context.Authenusers
                .Where(x => x.Email.ToLower().Equals(email.ToLower()))
                .Include(a => a.Posts).Select(x => new UserWithPostsModel()
                {
                    Email = x.Email,
                    Id = x.Id,
                    ImageURL = x.Imageurl ?? string.Empty,
                    Name = x.Fullname,
                    Role = x.Userrole == 0 ? "admin" : "customer",
                    RoleId = x.Userrole,
                    Posts = x.Posts.Select(p => new PostView()
                    {
                        Contact = p.Contact,
                        Created = p.Created,
                        Description = p.Description,
                        Id = p.Id,
                        IsBanned = p.Isbanned,
                        IsClosed = p.Isclosed,
                        OwnerId = p.Ownerid,
                        Title = p.Title,
                        PostImages = p.Postimages.Select(i => new PostImageView()
                        {
                            Id = i.Id,
                            ImageBase64 = i.Imagebase64,
                            PostId = i.Postid,
                        }).ToList()
                    }).ToList()
                }).FirstOrDefaultAsync();

                return result;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<bool> IsEmailExist(string email)
        {
            try
            {
                var query = from u in context.Authenusers
                            where u.Email.ToLower().Trim().Equals(email.ToLower().Trim())
                            select new { u };
                var account = await query.FirstOrDefaultAsync();
                return account != null;
            }catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task UpdateProfile(UserProfileUpdateModel model, Guid userId)
        {
            var account = await context.Authenusers.FindAsync(userId) ?? throw new RecordNotFoundException($"User ID: {userId} not found in DB");
            account.Fullname = model.FullName;
            account.Phone = model.Phone;
            account.Imageurl = model.ImageUrl;
        }
    }
}
