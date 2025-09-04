using ClubEconomyControl.Models;
using Microsoft.EntityFrameworkCore;

namespace ClubEconomyControl.Context
{
    public class ClubEconomyDbContext : DbContext
    {
        public ClubEconomyDbContext(DbContextOptions<ClubEconomyDbContext> options)
            : base(options)
        {
        }
        public DbSet<Club> Clubs { get; set; }
        public DbSet<Player> Players { get; set; }
        public DbSet<OrdinaryIncome> OrdinaryIncomes { get; set; }
        public DbSet<OrdinaryExpense> OrdinaryExpenses { get; set; }
        public DbSet<ExtraordinaryExpense> ExtraordinaryExpenses { get; set; }
        public DbSet<ExtraordinaryIncome> ExtraordinaryIncomes { get; set; }

        // Configuraciones adicionales del modelo
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Opcional: configurar relaciones, CONVERSIONES DE ENUMS, etc.
            modelBuilder.Entity<OrdinaryIncome>()
                .Property(i => i.Type)
                .HasConversion<string>();

            modelBuilder.Entity<OrdinaryExpense>()
                .Property(e => e.Type)
                .HasConversion<string>();

            modelBuilder.Entity<ExtraordinaryIncome>()
                .Property(e => e.Type)
                .HasConversion<string>();

            modelBuilder.Entity<ExtraordinaryExpense>()
                .Property(e => e.Type)
                .HasConversion<string>();

            modelBuilder.Entity<Club>()
                .HasMany(c => c.Players)
                .WithOne(p => p.Club)
                .HasForeignKey(p => p.ClubId);

        }
    }
}
