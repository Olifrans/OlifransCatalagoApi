using Catalago.Api.ApiEndpoints;
using Catalago.Api.AppServicesExtensions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container --> Semelhante ao ConfigureServices da class Startup net5

//Swagger teste API
builder.AddSwagger();
builder.AddPersistence();
builder.Services.AddCors();
builder.AddAuthenticationJwt();

var app = builder.Build();

//---------------------Endpoints
app.MapAutenticacaoEndpoints();
app.MapCategoriaEndpoints();
app.MapProdutoEndpoints();

// Configure the HTTP request pipeline --> Semelhante ao Configure da class Startup net5
var environment = app.Environment;
app.UserExceptionHandling(environment)
    .UseSwaggerEndpoints()
    .UserAppCors();

//Authentication configuration
app.UseAuthentication();

//Authorization configuration
app.UseAuthorization();

app.Run();