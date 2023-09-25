using System.IdentityModel.Tokens.Jwt;

namespace FindingPets.Business.JWT
{
    public class DecodeToken
    {
        private JwtSecurityTokenHandler _tokenHandler;

        public DecodeToken()
        {
            _tokenHandler = new JwtSecurityTokenHandler();
        }

        public int Decode(string token, string nameClaim)
        {
            var claim = _tokenHandler.ReadJwtToken(token).Claims.FirstOrDefault(selector => selector.Type.ToString().Equals(nameClaim));
            if (claim != null)
            {
                return Int32.Parse(claim.Value);
            }
            return 0;
        }

        public Guid DecodeID(string token, string nameClaim)
        {
            var claim = _tokenHandler.ReadJwtToken(token).Claims.FirstOrDefault(selector => selector.Type.ToString().Equals(nameClaim));
            if (claim != null)
            {
                return Guid.Parse(claim.Value);
            }
            return new Guid();
        }
    }
}
