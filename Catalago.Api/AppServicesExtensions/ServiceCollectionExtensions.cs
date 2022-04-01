using Catalago.Api.Context;
using Catalago.Api.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

namespace Catalago.Api.AppServicesExtensions
{
    public static class ServiceCollectionExtensions
    {
        public static WebApplicationBuilder AddSwagger(this WebApplicationBuilder builder)
        {
            builder.Services.AddSwagger();
            return builder;
        }

        //Swagger teste API
        public static IServiceCollection AddSwagger(this IServiceCollection services)
        {
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen(c =>
            {
                //Documentação e informação sobre a API
                c.SwaggerDoc("v1",
                    new OpenApiInfo
                    {
                        Title = "Olifrans Catalago Api",
                        Version = "v1",
                        Description = "Olifrans Catalago Api - Francisco Oliveira",

                        Contact = new OpenApiContact
                        {
                            Name = "Francisco Oliveira",
                            Email = "fransoliveira@gmail.com",
                            Url = new Uri("https://github.com/Olifrans")
                        },

                        License = new OpenApiLicense
                        {
                            Name = "Copyright © Olifrans Software",
                            Url = new Uri("https://github.com/Olifrans")
                        },
                        TermsOfService = new Uri("https://github.com/Olifrans")
                    });

                //Acesso token login JWT no Swagger
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "Cabeçalho de autorização JWT usando o esquema Bearer." +
                    "\r\n\r\n Digite 'Portador' [espaço] e, em seguida, seu token na entrada de texto abaixo." +
                    "\r\n\r\n Exemplo: \"O portador  12345abcdef\"",
                });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                       new string[] {}
                    }
                });
            });

            return services;
        }

        //Authentication configuration
        public static WebApplicationBuilder AddAuthenticationJwt(this WebApplicationBuilder builder)
        {
            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
             {
                 options.TokenValidationParameters = new TokenValidationParameters
                 {
                     ValidateIssuer = true,
                     ValidateAudience = true,
                     ValidateLifetime = true,
                     ValidateIssuerSigningKey = true,

                     ValidIssuer = builder.Configuration["Jwt:Issuer"],
                     ValidAudience = builder.Configuration["Jwt:Audience"],
                     IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
                 };
             });

            //Authentication configuration
            builder.Services.AddAuthorization();

            return builder;
        }

        public static WebApplicationBuilder AddPersistence(this WebApplicationBuilder builder)
        {
            //Cenexao ao Context BD
            var ConectaBD = builder.Configuration.GetConnectionString("OlifransConnection");

            builder.Services.AddDbContext<OlifransDbContext>(options =>
                options.UseNpgsql(ConectaBD));

            // Configuração JWT-Json-Web-Token JwtSecurityTokenHandler
            builder.Services.AddSingleton<ITokenService>(new TokenService());

            return builder;
        }
    }
}