using Catalago.Api.Context;
using Catalago.Api.Models;
using Microsoft.EntityFrameworkCore;




var builder = WebApplication.CreateBuilder(args);

// Add services to the container --> Semelhante ao ConfigureServices da class Startup net5
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


//Cenexao ao Context BD
var ConectaBD = builder.Configuration.GetConnectionString("OlifransConnection");
builder.Services.AddDbContext<OlifransDbContext>(options => options.UseNpgsql(ConectaBD));





var app = builder.Build();


//Endpoints Categorias
//get
app.MapGet("/categorias", async (OlifransDbContext dbContext) => await dbContext.Categorias.ToListAsync());

////get id
//app.MapGet("/categorias/{id}", async (int id, OlifransDbContext dbContext) =>
//    await dbContext.Categorias.FirstOrDefaultAsync(a => a.CategoriaId == id));


//get id
app.MapGet("/categorias/{id:int}", async (int id, OlifransDbContext dbContext) =>
{
    return await dbContext.Categorias.FindAsync(id)
    is Categoria categoria
                ? Results.Ok(categoria) : Results.NotFound();
});

    



//post
app.MapPost("/categorias", async (Categoria categoria, OlifransDbContext dbContext) =>
{
    dbContext.Categorias.Add(categoria);
    await dbContext.SaveChangesAsync();
    return Results.Created($"/categoria/{categoria.CategoriaId}", categoria);
});

//put
app.MapPut("/categorias/{id}", async (int id, Categoria categoria,
    OlifransDbContext dbContext) =>
{
    dbContext.Entry(categoria).State = EntityState.Modified;
    await dbContext.SaveChangesAsync();
    return categoria;
});

//delete
app.MapDelete("/categorias/{id}", async (int id, OlifransDbContext dbContext) =>
{
    var categoria = await dbContext.Categorias.FirstOrDefaultAsync(a => a.CategoriaId == id);
    if (categoria != null)
    {
        dbContext.Categorias.Remove(categoria);
        await dbContext.SaveChangesAsync();
    }
    return;
});






//Endpoints Produtos
//get
app.MapGet("/produtos", async (OlifransDbContext dbContext) =>
    await dbContext.Produtos.ToListAsync());

//get id
app.MapGet("/produtos/{id}", async (int id, OlifransDbContext dbContext) =>
    await dbContext.Produtos.FirstOrDefaultAsync(a => a.ProdutoId == id));

//post
app.MapPost("/produtos", async (Produto produto, OlifransDbContext dbContext) =>
{
    dbContext.Produtos.Add(produto);
    await dbContext.SaveChangesAsync();
    return Results.Created($"/categoria/{produto.ProdutoId}", produto);
});

//put
app.MapPut("/produtos/{id}", async (int id, Produto produto,
    OlifransDbContext dbContext) =>
{
    dbContext.Entry(produto).State = EntityState.Modified;
    await dbContext.SaveChangesAsync();
    return produto;
});

//delete
app.MapDelete("/produtos/{id}", async (int id, OlifransDbContext dbContext) =>
{
    var produto = await dbContext.Produtos.FirstOrDefaultAsync(a => a.ProdutoId == id);
    if (produto != null)
    {
        dbContext.Produtos.Remove(produto);
        await dbContext.SaveChangesAsync();
    }
    return;
});







// Configure the HTTP request pipeline --> Semelhante ao Configure da class Startup net5
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//app.UseHttpsRedirection();

app.Run();


