using Backend_GameDiscountNotifier.Model;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Metadata;

namespace Backend_GameDiscountNotifier.Data
{
    public class MariaDbContext : DbContext
    {
        public MariaDbContext(DbContextOptions<MariaDbContext> options) : base(options) {}
        public DbSet<JocEnPlataforma> JocEnPlataforma { get; set; }
        public DbSet<Joc> Jocs { get; set; }
        public DbSet<Plataforma> Plataformes { get; set; }
        public DbSet<SellerJoc> SellerJoc { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // taula JocEnPlataforma
            modelBuilder.Entity<JocEnPlataforma>().HasKey(x => new { x.IdJoc, x.IdPlataforma });

            modelBuilder.Entity<JocEnPlataforma>()
                .HasOne(e => e.Plataforma)
                .WithMany(e => e.JocsPlataforma)
                .HasForeignKey(e => e.IdPlataforma);

            modelBuilder.Entity<JocEnPlataforma>()
                .HasOne(e => e.Joc)
                .WithMany(e => e.JocEnPlataformes)
                .HasForeignKey(e => e.IdJoc);

            modelBuilder.Entity<JocEnPlataforma>()
                .Property(e => e.IdJocPlatataforma)
                .ValueGeneratedOnAdd();


            // taula Joc
            modelBuilder.Entity<Joc>().HasKey(pk => pk.IdJoc);

            modelBuilder.Entity<Joc>()
                .HasOne(e => e.Seller)
                .WithMany(e => e.Jocs)
                .HasForeignKey(e => e.IdSeller);

            modelBuilder.Entity<Joc>()
                .Property(e => e.IdJoc)
                .ValueGeneratedOnAdd();


            // taula Autor
            modelBuilder.Entity<SellerJoc>().HasKey(pk => pk.IdSeller);


            // taula Oferta
            modelBuilder.Entity<Oferta>().HasKey(pk => pk.IdExtretOferta);
        }
    }
}
