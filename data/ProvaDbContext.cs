using Microsoft.EntityFrameworkCore;
using prova.models;

namespace prova.data
{
    public class ProvaDbContext : DbContext
    {
        public ProvaDbContext(DbContextOptions<ProvaDbContext> options) : base(options) { }

        public DbSet<Servico> Servicos { get; set; }
        public DbSet<Contrato> Contratos { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Contrato>()
                .HasOne(c => c.Servico)
                .WithMany()
                .HasForeignKey(c => c.ServicoId);
        }
    }
}
