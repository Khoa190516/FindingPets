using FindingPets.Business.JWT;
using FindingPets.Data.Commons;
using FindingPets.Data.Models.UserModel;
using FindingPets.Data.PostgreEntities;
using FindingPets.Data.Repositories.BaseRepositories;
using FindingPets.Data.Repositories.ImplementedRepositories.AuthenUserRepositories;
using FindingPets.Data.UnitOfWork;

namespace FindingPets.Business.Services.AuthenUserServices
{
    public class AuthenUserService : IAuthenUserService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<AuthenUserService> _logger;

        public AuthenUserService(IUnitOfWork unitOfWork, ILogger<AuthenUserService> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<bool> CreatAccount(string email)
        {
            var isEmailExist = await _unitOfWork.AuthenUserRepo.IsEmailExist(email);

            if (isEmailExist)
            {
                throw new ResourceAlreadyExistsException($"Email {email} has been used");
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

            try
            {
                _unitOfWork.BeginTransaction();
                _unitOfWork.AuthenUserRepo.Add(newAccount);
                _unitOfWork.Dispose();
                return _unitOfWork.CommitTransaction() > 0;
            }
            catch(Exception)
            {
                _unitOfWork.RollbackTransaction();
                throw;
            }

        }

        public async Task<ProfileModel> GetProfile(Guid userId)
        {
            var account = await _unitOfWork.AuthenUserRepo.FindByID(userId);
            if(account != null)
            {
                var profile = await _unitOfWork.AuthenUserRepo.GetAccountByEmail(account.Email) ?? 
                    throw new RecordNotFoundException($"Email: {account.Email} Not Found");

                ProfileModel profileView = new()
                {
                    Id = profile.Id,
                    Email = profile.Email,
                    Name = profile.Name,
                    ImageURL = profile.ImageURL,
                    Role = profile.Role,
                    RoleId = profile.RoleId,
                    Phone = profile.Phone
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
            var result = await _unitOfWork.AuthenUserRepo.GetUserWithPosts(email);
            return result;
        }

        public async Task<UserTokenModel?> LoginWithEmail(UserLoginModel account)
        {
            // Check email is in DB and Create Token for this login
            _logger.LogInformation(message: $"Login with email: {account.Email}");
            var userJWT = await _unitOfWork.AuthenUserRepo.GetAccountByEmail(account.Email) ?? 
                throw new RecordNotFoundException($"Email: {account.Email} Not Found");
            userJWT.Token = JWTUserToken.GenerateJWTTokenUser(userJWT);
            return userJWT;
        }

        public async Task<bool> UpdateProfille(UserProfileUpdateModel model, Guid userId)
        {
            // Check email is in DB and Create Token for this login
            _logger.LogInformation(message: $"Begin updating profile ID: {userId}");
            try
            {
                _unitOfWork.BeginTransaction();
                await _unitOfWork.AuthenUserRepo.UpdateProfile(model, userId);
                var effectedRow = _unitOfWork.CommitTransaction();
                _unitOfWork.Dispose();
                return effectedRow > 0;
            }
            catch (Exception)
            {
                _unitOfWork.RollbackTransaction();
                _unitOfWork.Dispose();
                throw;
            }
        }
    }
}
