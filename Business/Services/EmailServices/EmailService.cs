using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using static System.Net.WebRequestMethods;

namespace FindingPets.Business.Services.EmailServices
{
    public class EmailService : IEmailService
    {
        public async Task<string> SendOTP(string email)
        {
            var OTPCode = CreateOTP();

            string from = "lilo190516@gmail.com";
            string password = "hlizgiggrjgtxakn";

            MimeMessage message = new();
            message.From.Add(MailboxAddress.Parse(from));
            message.Subject = "FindingPets Sent You OTP Code";
            message.To.Add(MailboxAddress.Parse(email));
            message.Body = new TextPart(MimeKit.Text.TextFormat.Html) { Text = "<html><body> Your OTP code is: " + OTPCode + "</body></html>" };

            using var smtp = new SmtpClient();
            await smtp.ConnectAsync("smtp.gmail.com", 587, SecureSocketOptions.StartTls);
            await smtp.AuthenticateAsync(from, password);
            await smtp.SendAsync(message);
            await smtp.DisconnectAsync(true);

            return OTPCode;
        }

        private static string CreateOTP()
        {
            Random rnd = new();

            return (rnd.Next(100000, 999999)).ToString();
        }
    }
}
