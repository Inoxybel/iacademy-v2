using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace IAcademyAPI.Controllers.v1
{
    [ApiController]
    [Route("api/token")]
    public class TokenController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public TokenController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public class Temp
        {
            public string Token { get; set; }
        }

        [HttpPost("get-master-token")]
        public Task<ActionResult<Temp>> MakeNewToken()
        {
            var expirationTimeInMinutes = int.Parse(_configuration.GetValue<string>("JwtSettings:ExpirationTimeInMinutes"));
            var secretKey = _configuration.GetValue<string>("JwtSettings:SecretKey");
            var key = Encoding.ASCII.GetBytes(secretKey);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim("OwnerId", "iacademy"),
                    new Claim("TextGenres", "[\"Informativo\",\"Explicativo\",\"Narrativo\",\"Argumentativo\"]")
                }),
                Expires = DateTime.UtcNow.AddMinutes(expirationTimeInMinutes),
                SigningCredentials = new SigningCredentials(
                     new SymmetricSecurityKey(key),
                     SecurityAlgorithms.HmacSha256Signature
                 )
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);

            return Task.FromResult((ActionResult<Temp>)Created(string.Empty, new Temp
            {
                Token = tokenHandler.WriteToken(token)
            }));
        }
    }
}
