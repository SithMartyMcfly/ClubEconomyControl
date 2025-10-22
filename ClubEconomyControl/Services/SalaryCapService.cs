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

        public async Task<decimal> CalculateSalaryCap(int ClubID, decimal nuevoGasto)
        {
            // Control de excepciones
            if (ClubID <= 0)
                throw new ArgumentException("ClubID no válido.");

            var salaryCap = await _context.Clubs
                .Where(c => c.Id == ClubID)
                .Select(c => c.SquadLimitEconomy)
                .FirstOrDefaultAsync();

            salaryCap -= nuevoGasto;

            // Solo controlo salayCap, ya que annualSquadExpenses al usar SumAsync devuelve 0 en caso de no haber jugadores
            if (salaryCap == null)
                throw new InvalidOperationException("No se encontró el límite económico del club.");

            return (decimal)salaryCap;
        }


    }
}
