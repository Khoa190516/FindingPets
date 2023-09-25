using FindingPets.Data.Commons;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace FindingPets.Business.JWT
{
    public class JWTUserToken
    {
        public static string GenerateJWTTokenUser(UserTokenModel user)
        {
            JwtSecurityToken? tokenUser = null;

            if (user == null || user.Role == null)
            {
                return "";
            }

            if (user.Role != null) //Admin || manager || show staff || ticket inspector || user || artist
            {
                tokenUser = new JwtSecurityToken(
                issuer: "https://securetoken.google.com/findingpets",
                audience: "findingpets",
                claims: new[] {
                 //Id
                 new Claim(Commons.JWTClaimID, user.Id.ToString()),
                 //fullname
                 new Claim(Commons.JWTClaimName, user.Name),
                 //Avatar
                 new Claim (Commons.JWTClaimEmail, user.Email),
                 //Role Id
                 new Claim(Commons.JWTClaimRoleID, user.RoleId.ToString()),
                 //Role
                 new Claim(ClaimTypes.Role, user.Role),
                },
                expires: DateTime.UtcNow.AddDays(90),
                signingCredentials: new SigningCredentials(
                        key: new SymmetricSecurityKey(Encoding.UTF8.GetBytes("findingpets16062000")),
                        algorithm: SecurityAlgorithms.HmacSha256
                        )
                );
            }
            return new JwtSecurityTokenHandler().WriteToken(tokenUser);
        }
    }
}
