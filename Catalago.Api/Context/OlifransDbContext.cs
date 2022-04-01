using Catalago.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Catalago.Api.Context
{
    public class OlifransDbContext : DbContext
    {
        public OlifransDbContext(DbContextOptions<OlifransDbContext> options) : base(options)
        {
        }

        public DbSet<Produto> Produtos { get; set; }
        public DbSet<Categoria> Categorias { get; set; }

        // Fluent-API--> Reforçando e explicitando, configurando, mapeando propriedades, tipos e relacionamentos
        protected override void OnModelCreating(ModelBuilder mbOptions)
        {
            //Categoria
            mbOptions.Entity<Categoria>().HasKey(c => c.CategoriaId);
            mbOptions.Entity<Categoria>().Property(c => c.Nome).HasMaxLength(100).IsRequired();
            mbOptions.Entity<Categoria>().Property(c => c.Descricao).HasMaxLength(150).IsRequired();

            //Produto
            mbOptions.Entity<Produto>().HasKey(p => p.ProdutoId);
            mbOptions.Entity<Produto>().Property(p => p.Nome).HasMaxLength(100).IsRequired();
            mbOptions.Entity<Produto>().Property(p => p.Descricao).HasMaxLength(150).IsRequired();
            mbOptions.Entity<Produto>().Property(p => p.Imagem).HasMaxLength(150);
            mbOptions.Entity<Produto>().Property(p => p.Preco).HasPrecision(14, 2);

            //Relacionamentos
            mbOptions.Entity<Produto>()
                .HasOne<Categoria>(c => c.Categoria)
                .WithMany(p => p.Produtos)
                .HasForeignKey(c => c.CategoriaId);
        }
    }
}