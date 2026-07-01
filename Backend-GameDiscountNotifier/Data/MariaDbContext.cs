using Backend_GameDiscountNotifier.Model.Contet;
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
            //clau primaria
            modelBuilder.Entity<JocEnPlataforma>()
                .HasKey(pk => pk.IdJocPlatataforma);

            //unique
            modelBuilder.Entity<JocEnPlataforma>()
                .HasAlternateKey(x => new { x.IdJoc, x.IdPlataforma });
            
            //autoincrement a clau primaria
            modelBuilder.Entity<JocEnPlataforma>()
                .Property(e => e.IdJocPlatataforma)
                .ValueGeneratedOnAdd();


            // taula Joc
            //clau primaria
            modelBuilder.Entity<Joc>()
                .HasKey(pk => pk.IdJoc);

            //autoincrement a clau primaria
            modelBuilder.Entity<Joc>()
                .Property(e => e.IdJoc)
                .ValueGeneratedOnAdd();


            // taula seller
            //clau primaria
            modelBuilder.Entity<SellerJoc>()
                .HasKey(pk => pk.IdSeller);

            //autoincrement a clau primaria
            modelBuilder.Entity<SellerJoc>()
                .Property(e => e.IdSeller)
                .ValueGeneratedOnAdd();

            // taula Oferta
            //clau primaria
            modelBuilder.Entity<Oferta>()
                .HasKey(pk => pk.IdExtretOferta);


            //relacions
            //relacio jocplataforma-plataforma
            modelBuilder.Entity<JocEnPlataforma>()
                .HasOne(e => e.Plataforma)
                .WithMany(e => e.JocsPlataforma)
                .HasForeignKey(e => e.IdPlataforma);

            //relacio jocplataforma-joc
            modelBuilder.Entity<JocEnPlataforma>()
                .HasOne(e => e.Joc)
                .WithMany(e => e.JocEnPlataformes)
                .HasForeignKey(e => e.IdJoc);

            //relacio joc-seller
            modelBuilder.Entity<Joc>()
                .HasOne(e => e.Seller)
                .WithMany(e => e.Jocs)
                .HasForeignKey(e => e.IdSeller);

            //relacio oferta-jocplataforma
            modelBuilder.Entity<Oferta>()
                .HasOne(e => e.JocPlatataforma)
                .WithMany(e => e.Ofertas)
                .HasForeignKey(e => e.IdJocPlatataforma);
        }
    }
}
