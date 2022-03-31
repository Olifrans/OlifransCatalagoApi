using Catalago.Api.Models;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Catalago.Api.Services
{
    public class TokenService : ITokenService
    {
        public string GeraToken(string key, string issuer, string audience, UserModel user)
        {
            //Declaração de Claims do usuario para composição do token
            var claims = new[]
            {
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.NameIdentifier, Guid.NewGuid().ToString()),
            };

            //Gerador de chave no base na chave secreta
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));


            //Especificação da chave de assinatura
            var credential = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);


            //Assinatura digital e geração do token
            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.Now.AddMinutes(20),
                signingCredentials: credential
                );            

            //Serealização do tokem compactado retornando uma string
            var tokenHandler = new JwtSecurityTokenHandler();
            var stringToken = tokenHandler.WriteToken(token);
            return stringToken;


        }
    }
}
