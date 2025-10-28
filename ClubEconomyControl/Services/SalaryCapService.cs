using ClubEconomyControl.Context;
using Microsoft.EntityFrameworkCore;


namespace ClubEconomyControl.Services
{
    public class SalaryCapService
    {
        private readonly ClubEconomyDbContext _context;

        public SalaryCapService(ClubEconomyDbContext context)
        {
            _context = context;
        }

        public async Task CalculateSalaryCap(int ClubID, decimal? nuevoGasto)
        {
            // Control de excepciones
            if (ClubID <= 0)
                throw new ArgumentException("ClubID no válido.");

            // Recupero el club
            var club = await _context.Clubs
                .Where(c => c.Id == ClubID)
                .FirstOrDefaultAsync() ?? throw new NullReferenceException("El club no existe");

            // Ajusto el límite económico
            club.SquadLimitEconomy -= nuevoGasto;

            //TODO: Añadir libera masa salarial al vender jugador

            var player = await _context.Players
                .Where(p => p.ClubId == ClubID)
                .FirstOrDefaultAsync();

            club.SquadLimitEconomy += player.AnnualExpense;

            // Operaciones de guardado
            _context.Clubs.Update(club);
            await _context.SaveChangesAsync();
        }


    }
}
