using Microsoft.EntityFrameworkCore;
using TapiocaManager.API.Models;

namespace TapiocaManager.API.Data
{
    public class TapiocaDbContext : DbContext
    {

        public TapiocaDbContext(DbContextOptions<TapiocaDbContext> options) : base(options)
        {
        }

        public DbSet<Produtos> Produtos { get; set;}
        public DbSet<Pedido> Pedidos { get; set; }
        public DbSet<ItemPedido> ItemPedidos { get; set;}

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            //configura relacionamento entre pedidos e ItemPedidos
            modelBuilder.Entity<ItemPedido>()
                .HasOne<Pedido>()
                .WithMany(p => p.Itens)
                .HasForeignKey(ip => ip.PedidoId);

            // Configurar relacionamento entre ItemPedido e Produto
            modelBuilder.Entity<ItemPedido>()
                .HasOne<Produtos>()
                .WithMany()
                .HasForeignKey(ip => ip.ProdutoId);

        }
    }
}