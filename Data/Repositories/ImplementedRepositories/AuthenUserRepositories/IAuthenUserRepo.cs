using FindingPets.Business.JWT;
using FindingPets.Data.Models.UserModel;
using FindingPets.Data.PostgreEntities;
using FindingPets.Data.Repositories.BaseRepositories;

namespace FindingPets.Data.Repositories.ImplementedRepositories.AuthenUserRepositories
{
    public interface IAuthenUserRepo : IBaseRepo<Authenuser>
    {
        public Task<UserTokenModel?> GetAccountByEmail(string email);

        public Task<bool> IsEmailExist(string email);

        public Task UpdateProfile(UserProfileUpdateModel model, Guid userId);

        public Task<UserWithPostsModel?> GetUserWithPosts(string email);
    }
}
