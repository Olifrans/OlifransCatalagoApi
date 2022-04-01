using Catalago.Api.Context;
using Catalago.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Catalago.Api.ApiEndpoints
{
    public static class ProdutoEndpoints
    {
        //---------------------Endpoints Produtos
        public static void MapProdutoEndpoints(this WebApplication app)
        {
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
                : Results.NotFound("Produto não encontrado");
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
                .Skip((numeroPagina - 1) * tamanhoPagina)
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
                if (produtoDoBD == null) return Results.NotFound("Produto não encontrado");

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
                    return Results.BadRequest("Os Id do produto não confere");
                }

                var produtoDoBD = await dbContext.Produtos.FindAsync(id);
                if (produtoDoBD is null) return Results.NotFound("Produto não encontrado");

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
                    return Results.NotFound("Produto não encontrado");
                }
                dbContext.Produtos.Remove(produtoDB);
                await dbContext.SaveChangesAsync();
                return Results.Ok(produtoDB);
            })
                .Produces<Produto>(StatusCodes.Status200OK)
                .Produces(StatusCodes.Status404NotFound)
                .WithName("ExcluirProduto")
                .WithTags("Produtos");
        }
    }
}