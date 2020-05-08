using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Pog.Auth.Commands.Login
{
    public class LoginUserHandler : IRequestHandler<LoginUser, object>
    {
        private readonly IConfiguration _configuration;
        private readonly KeyConfig _keyConfig;

        public LoginUserHandler(IConfiguration configuration, IOptions<KeyConfig> keyConfig)
        {
            _configuration = configuration;
            _keyConfig = keyConfig.Value;
        }

        public async Task<object> Handle(LoginUser request, CancellationToken cancellationToken)
        {
            if (request.Username.Length > 5 && request.Password.Length > 5)
            {
                var rsa = RSA.Create();
                rsa.ImportRSAPrivateKey(Convert.FromBase64String(_keyConfig.Kurwo), out _);
                var token = await CreateToken(request.Username, request.Resource, new RsaSecurityKey(rsa));
                return new {Token = token};
            }

            throw new Exception("whoopsie");
        }

        public async Task<string> CreateToken(string username, string resource, RsaSecurityKey key)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Audience = resource,
                Issuer = "krwnx",
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.Name, username)
                }),
                Expires = DateTime.UtcNow.AddDays(1),
                SigningCredentials = new SigningCredentials(key, SecurityAlgorithms.RsaSha256)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var wt = tokenHandler.WriteToken(token);
            return await Task.FromResult(wt);
        }
    }
}