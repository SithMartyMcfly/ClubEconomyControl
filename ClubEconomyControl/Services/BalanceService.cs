using ClubEconomyControl.Context;
using ClubEconomyControl.Models;
using Microsoft.EntityFrameworkCore;


namespace ClubEconomyControl.Services
{
    public class BalanceService
    {
        private readonly ClubEconomyDbContext _context;

        public BalanceService(ClubEconomyDbContext context)
        {
            _context = context;
        }
        public async Task<int> CalculateBalanceAsync(Club club)
        {
            var clubId = club.Id;
            var incomes =
                    await _context.OrdinaryIncomes
                .Where(i => i.ClubId == clubId)
                .SumAsync(i => i.Amount) +
                    await _context.ExtraordinaryIncomes
                .Where(i => i.ClubId == clubId)
                .SumAsync(i => i.Amount);
            var expenses =
                    await _context.OrdinaryExpenses
                .Where(i => i.ClubId == clubId)
                .SumAsync(i => i.Amount) +
                    await _context.ExtraordinaryExpenses
                    .Where(i => i.ClubId == clubId)
                    .SumAsync(i => i.Amount);

            var balance = incomes - expenses;
            //Recalculamos con este servicio el Balance
            club.Balance = balance;
            club.SquadLimitEconomy = balance;
            _context.Clubs.Update(club);
            await _context.SaveChangesAsync();



            return balance;
        }
    }
}
