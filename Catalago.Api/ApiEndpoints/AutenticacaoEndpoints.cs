using Catalago.Api.Models;
using Catalago.Api.Services;
using Microsoft.AspNetCore.Authorization;

namespace Catalago.Api.ApiEndpoints
{
    public static class AutenticacaoEndpoints
    {
        //---------------------Endpoints Login Autenticacao
        public static void MapAutenticacaoEndpoints(this WebApplication app)
        {
            app.MapPost("/login", [AllowAnonymous] (UserModel userModel, ITokenService tokenService) =>
            {
                if (userModel == null)
                {
                    return Results.BadRequest("Login Inválido");
                }
                if (userModel.UserName == "olifrans" && userModel.Password == "Admin@123")
                {
                    var tokenString = tokenService.GeraToken(app.Configuration["Jwt:Key"],
                        app.Configuration["Jwt:Issuer"],
                        app.Configuration["Jwt:Audience"],
                        userModel);
                    return Results.Ok(new { tokenService = tokenString });
                }
                else
                {
                    return Results.BadRequest("Login Inválido");
                }
            })
                .Produces(StatusCodes.Status400BadRequest)
                .Produces(StatusCodes.Status200OK)
                .WithName("LoginAcesso")
                .WithTags("Autenticacao");
        }
    }
}