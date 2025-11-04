using ClubEconomyControl.Context;
using ClubEconomyControl.Models;
using Microsoft.EntityFrameworkCore;


namespace ClubEconomyControl.Services
{
    public class SalaryCapService
    {
        private readonly ClubEconomyDbContext _context;
        private readonly BalanceService _balanceService;

        public SalaryCapService(ClubEconomyDbContext context, BalanceService balanceService)
        {
            _context = context;
            _balanceService = balanceService;
        }



        public async Task CalculateSalaryCap(int ClubID)
        {
            // Control de excepciones
            if (ClubID <= 0)
                throw new ArgumentException("ClubID no válido.");

            // Recupero el club
            var club = await _context.Clubs
                .Where(c => c.Id == ClubID)
                .FirstOrDefaultAsync() ?? throw new NullReferenceException("El club no existe");

            var AnnualExpensesSalary = await _context.Players
                 .Where(p => p.ClubId == ClubID && !p.isSelled)
                 .SumAsync(p => p.AnnualExpense);

            var balance = await _balanceService.CalculateBalanceAsync(club);

            club.Balance = balance;

            club.SquadLimitEconomy = balance - AnnualExpensesSalary;


            // Operaciones de actualización
            _context.Clubs.Update(club);
        }



        // TODO: Implementar método para recalcular Salary Cap EN CASOS DE VENTA

        public async Task<Club> CalculateSalaryCapSale(int ClubId, decimal? amortizationRemaining)
        {

            var club = await _context.Clubs.
                Where(club => club.Id == ClubId).
                FirstOrDefaultAsync();

            await CalculateSalaryCap(ClubId);

            club.SquadLimitEconomy = club.SquadLimitEconomy - amortizationRemaining;

            _context.Update(club);
            _context.SaveChanges();
            return club;
        }

    }
}
