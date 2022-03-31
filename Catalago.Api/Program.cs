using Catalago.Api.Context;
using Catalago.Api.Models;
using Catalago.Api.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
//using capp_api.infrastructure.Security;
using Microsoft.Extensions.DependencyInjection;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container --> Semelhante ao ConfigureServices da class Startup net5
builder.Services.AddEndpointsApiExplorer();



//Swagger teste API
builder.Services.AddSwaggerGen(c =>
{
    //Acesso token login JWT no Swagger
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Cabe�alho de autoriza��o JWT usando o esquema Bearer." +
        "\r\n\r\n Digite 'Portador' [espa�o] e, em seguida, seu token na entrada de texto abaixo." +
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


//Authentication configuration
builder.Services.AddAuthentication(x =>
{
  x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
  x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(x =>
{
    x.RequireHttpsMetadata = false;
    x.SaveToken = true;
    x.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Cryptography.Key),
        ValidateIssuer = false,
        ValidateAudience = false
    };
});


//Refatorado Francisco --> Authorization configuration
builder.Services.AddAuthorization(options =>
{
    //options.AddPolicy("user", policy => policy.RequireClaim("Store", "user"));
    //options.AddPolicy("admin", policy => policy.RequireClaim("Store", "admin"));
    //options.AddPolicy("CompletedTrue", policy => policy.Requirements.Add(new DefaultRequirement(true)));
    //options.AddPolicy("CompletedFalse", policy => policy.Requirements.Add(new DefaultRequirement(false)));
});


//Cenexao ao Context BD
var ConectaBD = builder.Configuration.GetConnectionString("OlifransConnection");
builder.Services.AddDbContext<OlifransDbContext>(options => options.UseNpgsql(ConectaBD));


// JWT Json Web Token JwtSecurityTokenHandler
builder.Services.AddSingleton<ITokenService>(new TokenService());   



var app = builder.Build();



/*
 * Continua curso 
 * 
 * posicao 14:20
 * */



//---------------------Endpoints Categorias

//get categorias produto
app.MapGet("/categoriaprodutos", async (OlifransDbContext dbContext) =>
 await dbContext.Categorias.Include(cp => cp.Produtos).ToListAsync()
 )
    .Produces<List<Categoria>>(StatusCodes.Status200OK)
    .WithTags("Categorias");

//get
app.MapGet("/categorias", async (OlifransDbContext dbContext) => await dbContext.Categorias.ToListAsync())
    .WithTags("Categorias");

//get id
app.MapGet("/categorias/{id:int}", async (int id, OlifransDbContext dbContext) =>
{
    return await dbContext.Categorias.FindAsync(id)
    is Categoria categoria
                ? Results.Ok(categoria) : Results.NotFound("Produto n�o encontrado");
}).WithTags("Categorias");

//post
app.MapPost("/categorias", async (Categoria categoria, OlifransDbContext dbContext) =>
{
    dbContext.Categorias.Add(categoria);
    await dbContext.SaveChangesAsync();
    return Results.Created($"/categoria/{categoria.CategoriaId}", categoria);
}).WithTags("Categorias");

//put
app.MapPut("/categorias/{id:int}", async (int id, Categoria categoria, OlifransDbContext dbContext) =>
{
    if (categoria.CategoriaId != id)
    {
        return Results.BadRequest();
    }
    var categoriaDoBD = await dbContext.Categorias.FindAsync(id);
    if (categoriaDoBD is null) return Results.NotFound();

    categoriaDoBD.Nome = categoria.Nome;
    categoriaDoBD.Descricao = categoria.Descricao;

    await dbContext.SaveChangesAsync();
    return Results.Ok(categoriaDoBD);
}).WithTags("Categorias");

//delete
app.MapDelete("/categorias/{id:int}", async (int id, OlifransDbContext dbContext) =>
{
    var categoria = await dbContext.Categorias.FindAsync(id);

    if (categoria != null)
        return Results.NotFound();

    dbContext.Categorias.Remove(categoria);
    await dbContext.SaveChangesAsync();
    return Results.NoContent();
}).WithTags("Categorias");








//---------------------Endpoints Produtos

//get
app.MapGet("/produtos", async (OlifransDbContext dbContext) =>
    await dbContext.Produtos.ToListAsync())
    .Produces<Produto>(StatusCodes.Status200OK)
    .WithTags("Produtos");

//get id
app.MapGet("/produtos/{id:int}", async (int id, OlifransDbContext dbContext) =>
{
    return await dbContext.Produtos.FindAsync(id)
    is Produto produto
    ? Results.Ok(produto)
    : Results.NotFound("Produto n�o encontrado");
})
    .Produces<Produto>(StatusCodes.Status201Created)
    .Produces(StatusCodes.Status404NotFound)
    .WithTags("Produtos");

//get criterio
app.MapGet("/produtos/nome/{criterio}", async (string criterio, OlifransDbContext dbContext) =>
{
    var produtosSelecionados = dbContext.Produtos.Where(p => p.Nome
                                .ToLower()
                                .Contains(criterio.ToLower()))
                                .ToList();
    
   return produtosSelecionados.Count > 0
    ? Results.Ok(produtosSelecionados)
    : Results.NotFound(Array.Empty<Produto>());
    
})
    .Produces<List<Produto>>(StatusCodes.Status200OK)
    .WithTags("FiltrarPorNome")
    .WithTags("Produtos");

//get paginacao
app.MapGet("/produtosporpagina", async (int numeroPagina, int tamanhoPagina, OlifransDbContext dbContext)

    => await dbContext.Produtos
    .Skip((numeroPagina -1) * tamanhoPagina)
    .Take(tamanhoPagina)
    .ToListAsync()
)
    .Produces<List<Produto>>(StatusCodes.Status200OK)
    .WithTags("FiltrarPorNome")
    .WithTags("Produtos");

//post
app.MapPost("/produtos", async (Produto produto, OlifransDbContext dbContext) =>
{
    dbContext.Produtos.Add(produto);
    await dbContext.SaveChangesAsync();
    return Results.Created($"/produtos/{produto.ProdutoId}", produto);
})
    .Produces<Produto>(StatusCodes.Status201Created)
    .WithName("CriarNovoProduto")
    .WithTags("Produtos");

//put pelo nome
app.MapPut("/produtos/{nome}", async (int produtoId, string produtoNome, string produtoDescricao, decimal produtoPreco, OlifransDbContext dbContext) =>
{
    var produtoDoBD = dbContext.Produtos.SingleOrDefault(p => p.ProdutoId == produtoId);
    if (produtoDoBD == null) return Results.NotFound("Produto n�o encontrado");

    produtoDoBD.Nome = produtoNome;
    produtoDoBD.Descricao = produtoDescricao;
    produtoDoBD.Preco = produtoPreco;

    await dbContext.SaveChangesAsync();
    return Results.Ok(produtoDoBD);
})
    .Produces<Produto>(StatusCodes.Status200OK)
    .Produces(StatusCodes.Status404NotFound)
    .WithName("AtualiazarNomeProduto")
    .WithTags("Produtos");

//put todos dados
app.MapPut("/produtos/{id:int}", async (int id, Produto produto, OlifransDbContext dbContext) =>
{   
    if (produto.ProdutoId != id)
    {
        return Results.BadRequest("Os Id do produto n�o confere");
    }

    var produtoDoBD = await dbContext.Produtos.FindAsync(id);
    if (produtoDoBD is null) return Results.NotFound("Produto n�o encontrado");

    produtoDoBD.Nome = produto.Nome;
    produtoDoBD.Descricao = produto.Descricao;
    produtoDoBD.Preco = produto.Preco;
    produtoDoBD.DataCompra = produto.DataCompra;
    produtoDoBD.Estoque = produto.Estoque;
    produtoDoBD.Imagem = produto.Imagem;
    produtoDoBD.CategoriaId = produto.CategoriaId;

    await dbContext.SaveChangesAsync();
    return Results.Ok(produtoDoBD);
})
    .Produces<Produto>(StatusCodes.Status200OK)
    .Produces(StatusCodes.Status400BadRequest)
    .Produces(StatusCodes.Status404NotFound)
    .WithName("AtualiazarProduto")
    .WithTags("Produtos");

//delete
app.MapDelete("/produtos/{id:int}", async (int id, OlifransDbContext dbContext) =>
{
    var produtoDB = await dbContext.Produtos.FindAsync(id);
    if (produtoDB is null)
    {
        return Results.NotFound("Produto n�o encontrado");
    } 
    dbContext.Produtos.Remove(produtoDB);
    await dbContext.SaveChangesAsync();
    return Results.Ok(produtoDB);
})
    .Produces<Produto>(StatusCodes.Status200OK)
    .Produces(StatusCodes.Status404NotFound)
    .WithName("ExcluirProduto")
    .WithTags("Produtos");



// Configure the HTTP request pipeline --> Semelhante ao Configure da class Startup net5
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//app.UseHttpsRedirection();

app.Run();


