using ClubEconomyControl.Context;
using Microsoft.AspNetCore.Mvc;


namespace ClubEconomyControl.Controllers
{
    public class EconomyController : Controller
    {
        private readonly ClubEconomyDbContext _context;
        public EconomyController(ClubEconomyDbContext context)
        {
            _context = context;
        }

        // Get: /Economy/OrdinrayIncome
        [HttpGet]
        public IActionResult OrdinaryIncomes(int clubId)
        {
            var ordinaryIncomes = _context.OrdinaryIncomes
                .Where(oi => oi.ClubId == clubId)
                .ToList();

            if (ordinaryIncomes == null)
            {
                return NotFound();
            }

            return View("OrdinaryIncomes", ordinaryIncomes);
        }

        // Get: /Economy/OrdinaryExpense
        [HttpGet]
        public IActionResult OrdinaryExpenses(int clubId)
        {
            var ordinaryExpenses = _context.OrdinaryExpenses
                .Where(oe => oe.ClubId == clubId)
                .ToList();
            if (ordinaryExpenses == null)
            {
                return NotFound();
            }

            return View("OrdinaryExpenses", ordinaryExpenses);
        }

        // Get: /Economy/ExtraOrdinaryIncome
        [HttpGet]

        public IActionResult ExtraOrdinaryIncomes(int clubId)
        {
            var extraOrdinaryIncomes = _context.ExtraordinaryIncomes
                .Where(eoi => eoi.ClubId == clubId)
                .ToList();
            if (extraOrdinaryIncomes == null)
            {
                return NotFound();
            }
            return View("ExtraOrdinaryIncomes", extraOrdinaryIncomes);
        }

        // Get: /Economy/ExtraOrdinaryExpense
        [HttpGet]
        public IActionResult ExtraOrdinaryExpenses(int clubId)
        {
            var extraOrdinaryExpenses = _context.ExtraordinaryExpenses
                .Where(eoe => eoe.ClubId == clubId)
                .ToList();
            if (extraOrdinaryExpenses == null)
            {
                return NotFound();
            }
            return View("ExtraOrdinaryExpenses", extraOrdinaryExpenses);
        }

    }
}
