using FindingPets.Business.JWT;
using FindingPets.Data.Entities;
using FindingPets.Data.Models.UserModel;
using FindingPets.Data.Repositories.BaseRepositories;
using System.Collections;

namespace FindingPets.Data.Repositories.ImplementedRepositories.AuthenUserRepositories
{
    public interface IAuthenUserRepo : IBaseRepo<AuthenUser>
    {
        public Task<UserTokenModel> GetAccountByEmail(string email);

        public Task<bool> IsEmailExist(string email);

        public Task<bool> UpdateProfile(UserProfileUpdateModel model, Guid userId);

        public Task<IEnumerable> GetUserWithPosts(string email);
    }
}
