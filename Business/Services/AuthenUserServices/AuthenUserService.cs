using FindingPets.Business.JWT;
using FindingPets.Data.Commons;
using FindingPets.Data.Models.UserModel;
using FindingPets.Data.PostgreEntities;
using FindingPets.Data.Repositories.ImplementedRepositories.AuthenUserRepositories;

namespace FindingPets.Business.Services.AuthenUserServices
{
    public class AuthenUserService : IAuthenUserService
    {
        private readonly IAuthenUserRepo _authenUserRepo;
        private readonly ILogger<AuthenUserService> _logger;

        public AuthenUserService(IAuthenUserRepo authenUserRepo, ILogger<AuthenUserService> logger)
        {
            _authenUserRepo = authenUserRepo;
            _logger = logger;
        }

        public async Task<bool> CreatAccount(string email)
        {
            try
            {
                var isEmailExist = await _authenUserRepo.IsEmailExist(email);

                if(isEmailExist)
                {
                    throw new Exception($"Email {email} has been used");
                }

                Authenuser newAccount = new()
                {
                    Id = Guid.NewGuid(),
                    Email = email,
                    Fullname = string.Empty,
                    Isactive = true,
                    Phone = string.Empty,
                    Userrole = Commons.CUSTOMER,
                };
                _logger.LogInformation(message: $"Begin create new authenUser with ID: {newAccount.Id} account at {DateTime.Now}");
                await _authenUserRepo.Insert(newAccount);
                return true;
            }catch(Exception ex)
            {
                _logger.LogError(message: $"Error when trying to insert new account with email: {email} at {DateTime.Now}. Error: {ex.Message}");
                return false;
            }
        }

        public async Task<ProfileModel> GetProfile(Guid userId)
        {
            var account = await _authenUserRepo.FindByID(userId);
            if(account != null)
            {
                var profile = await _authenUserRepo.GetAccountByEmail(account.Email) ?? 
                    throw new RecordNotFoundException($"Email: {account.Email} Not Found");

                ProfileModel profileView = new()
                {
                    Id = profile.Id,
                    Email = profile.Email,
                    Name = profile.Name,
                    ImageURL = profile.ImageURL,
                    Role = profile.Role,
                    RoleId = profile.RoleId,
                };

                return profileView;
            }
            else
            {
                throw new RecordNotFoundException($"User ID: {userId} Not Found");
            }
        }

        public async Task<UserWithPostsModel?> GetUserWithPost(string email)
        {
            var result = await _authenUserRepo.GetUserWithPosts(email);
            return result;
        }

        public async Task<UserTokenModel?> LoginWithEmail(UserLoginModel account)
        {
            // Check email is in DB and Create Token for this login
            _logger.LogInformation(message: $"Login with email: {account.Email}");
            var userJWT = await _authenUserRepo.GetAccountByEmail(account.Email) ?? 
                throw new RecordNotFoundException($"Email: {account.Email} Not Found");
            userJWT.Token = JWTUserToken.GenerateJWTTokenUser(userJWT);
            return userJWT;
        }

        public async Task<bool> UpdateProfille(UserProfileUpdateModel model, Guid userId)
        {
            // Check email is in DB and Create Token for this login
            _logger.LogInformation(message: $"Begin updating profile ID: {userId}");
            return await _authenUserRepo.UpdateProfile(model, userId);
        }
    }
}
