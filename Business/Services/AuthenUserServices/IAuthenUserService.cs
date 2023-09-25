using FindingPets.Business.JWT;
using FindingPets.Data.Models.UserModel;
using System.Collections;

namespace FindingPets.Business.Services.AuthenUserServices
{
    public interface IAuthenUserService
    {
        public Task<UserTokenModel> LoginWithEmail(UserLoginModel user);
        public Task<bool> CreatAccount(string email);
        public Task<bool> UpdateProfille(UserProfileUpdateModel model, Guid userId);
        public Task<ProfileModel> GetProfile(Guid userId);
        public Task<IEnumerable> GetUserWithPost(string email);
    }
}
