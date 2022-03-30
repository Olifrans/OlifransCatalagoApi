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
    }
}