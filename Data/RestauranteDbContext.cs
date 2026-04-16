using A1_order_system.Entities;
using Microsoft.EntityFrameworkCore;

namespace A1_order_system.Data
{
    public class RestauranteDbContext : DbContext
    {
        public RestauranteDbContext(DbContextOptions<RestauranteDbContext> options) : base(options) { }

        public DbSet<Usuario> Usuarios => Set<Usuario>();
        public DbSet<Endereco> Enderecos => Set<Endereco>();
        public DbSet<ItemCardapio> ItensCardapio => Set<ItemCardapio>();
        public DbSet<Ingrediente> Ingredientes => Set<Ingrediente>();
        public DbSet<SugestaoChefe> SugestoesChefe => Set<SugestaoChefe>();
        public DbSet<Pedido> Pedidos => Set<Pedido>();
        public DbSet<PedidoItem> PedidoItens => Set<PedidoItem>();
        public DbSet<Atendimento> Atendimentos => Set<Atendimento>();
        public DbSet<AtendimentoPresencial> AtendimentosPresenciais => Set<AtendimentoPresencial>();
        public DbSet<AtendimentoDeliveryProprio> AtendimentosDeliveryProprio => Set<AtendimentoDeliveryProprio>();
        public DbSet<AtendimentoDeliveryApp> AtendimentosDeliveryApp => Set<AtendimentoDeliveryApp>();
        public DbSet<Mesa> Mesas => Set<Mesa>();
        public DbSet<Reserva> Reservas => Set<Reserva>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Atendimento>()
                .HasDiscriminator<string>("TipoAtendimento")
                .HasValue<AtendimentoPresencial>("Presencial")
                .HasValue<AtendimentoDeliveryProprio>("DeliveryProprio")
                .HasValue<AtendimentoDeliveryApp>("DeliveryApp");

            modelBuilder.Entity<Usuario>(e =>
            {
                e.HasKey(u => u.Id);
                e.Property(u => u.Nome).IsRequired().HasMaxLength(150);
                e.Property(u => u.Email).IsRequired().HasMaxLength(200);
                e.HasIndex(u => u.Email).IsUnique();
                e.Property(u => u.Senha).IsRequired();
                e.Property(u => u.Perfil)
                    .IsRequired()
                    .HasMaxLength(20)
                    .HasDefaultValue(PerfisUsuario.Cliente);
            });

            modelBuilder.Entity<Endereco>(e =>
            {
                e.HasKey(en => en.Id);
                e.HasOne(en => en.Usuario)
                 .WithMany(u => u.Enderecos)
                 .HasForeignKey(en => en.UsuarioId)
                 .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<ItemCardapio>(e =>
            {
                e.HasKey(i => i.Id);
                e.Property(i => i.PrecoBase).HasColumnType("decimal(10,2)");
                e.Property(i => i.Periodo).HasConversion<string>();
            });

            modelBuilder.Entity<ItemCardapio>()
                .HasMany(i => i.Ingredientes)
                .WithMany(ing => ing.ItensCardapio)
                .UsingEntity(j => j.ToTable("ItemCardapioIngrediente"));

            modelBuilder.Entity<SugestaoChefe>(e =>
            {
                e.HasKey(s => s.Id);
                e.Property(s => s.Desconto).HasColumnType("decimal(5,2)");
                e.Property(s => s.Periodo).HasConversion<string>();

                e.HasIndex(s => new { s.Data, s.Periodo }).IsUnique();

                e.HasOne(s => s.ItemCardapio)
                 .WithMany(i => i.SugestaoChefes)
                 .HasForeignKey(s => s.ItemCardapioId);
            });

            modelBuilder.Entity<Pedido>(e =>
            {
                e.HasKey(p => p.Id);
                e.Property(p => p.ValorTotal).HasColumnType("decimal(10,2)");


                e.HasOne(p => p.Usuario)
                 .WithMany(u => u.Pedidos)
                 .HasForeignKey(p => p.UsuarioId);

                e.HasOne(p => p.Atendimento)
                 .WithOne(a => a.Pedido)
                 .HasForeignKey<Pedido>(p => p.AtendimentoId);
            });

            modelBuilder.Entity<PedidoItem>(e =>
            {
                e.HasKey(pi => pi.Id);
                e.HasOne(pi => pi.Pedido)
                 .WithMany(p => p.Itens)
                 .HasForeignKey(pi => pi.PedidoId);
                e.HasOne(pi => pi.ItemCardapio)
                 .WithMany(i => i.PedidoItens)
                 .HasForeignKey(pi => pi.ItemCardapioId);
            });

            modelBuilder.Entity<Mesa>(e =>
            {
                e.HasKey(m => m.Id);
                e.HasIndex(m => m.Numero).IsUnique();
            });

            modelBuilder.Entity<Reserva>(e =>
            {
                e.HasKey(r => r.Id);


                e.HasOne(r => r.Mesa)
                 .WithMany(m => m.Reservas)
                 .HasForeignKey(r => r.MesaId);

                e.HasOne(r => r.Usuario)
                 .WithMany(u => u.Reservas)
                 .HasForeignKey(r => r.UsuarioId);
            });

            modelBuilder.Entity<AtendimentoDeliveryProprio>(e =>
            {
                e.Property(a => a.TaxaFixa).HasColumnType("decimal(10,2)");
            });

        }
    }
}

