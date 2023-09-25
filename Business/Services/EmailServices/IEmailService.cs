namespace FindingPets.Business.Services.EmailServices
{
    public interface IEmailService
    {
        public Task<string> SendOTP(string email);
    }
}
