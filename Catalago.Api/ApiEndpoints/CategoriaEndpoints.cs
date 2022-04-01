using Catalago.Api.Context;
using Catalago.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Catalago.Api.ApiEndpoints
{
    public static class CategoriaEndpoints
    {
        //---------------------Endpoints Categorias
        public static void MapCategoriaEndpoints(this WebApplication app)
        {
            //get categorias produto
            app.MapGet("/categoriaprodutos", async (OlifransDbContext dbContext) =>
             await dbContext.Categorias.Include(cp => cp.Produtos).ToListAsync()
             )
                .Produces<List<Categoria>>(StatusCodes.Status200OK)
                .WithTags("Categorias");

            //get --> Para acesar este endpoint requer autorização
            app.MapGet("/categorias", async (OlifransDbContext dbContext) =>
            await dbContext.Categorias.ToListAsync())
                .WithTags("Categorias")
                .RequireAuthorization();

            //get id
            app.MapGet("/categorias/{id:int}", async (int id, OlifransDbContext dbContext) =>
            {
                return await dbContext.Categorias.FindAsync(id)
                is Categoria categoria
                            ? Results.Ok(categoria) : Results.NotFound("Produto não encontrado");
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
        }
    }
}