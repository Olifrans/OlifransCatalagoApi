using Catalago.Api.Models;

namespace Catalago.Api.Services
{
    public interface ITokenService
    {
        string GeraToken(string Key, string Issuer, string Audience, UserModel user);
    }
}