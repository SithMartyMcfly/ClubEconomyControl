using ClubEconomyControl.Context;
using ClubEconomyControl.Models;
using ClubEconomyControl.Models.ViewModels;
using ClubEconomyControl.Services;
using Microsoft.AspNetCore.Mvc;


namespace ClubEconomyControl.Controllers
{
    public class EconomyController : Controller
    {
        private readonly ClubEconomyDbContext _context;
        private readonly SalaryCapService _salaryCapService;
        public EconomyController(ClubEconomyDbContext context, SalaryCapService salaryCapService)
        {
            _context = context;
            _salaryCapService = salaryCapService;
        }

        //OBTIENEN VISTAS

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

        // EDICIÓN ECONOMÍA

        // PUT: /Club/DetailEdit
        [HttpGet]
        public async Task<IActionResult> EditEconomy(int id, string type)
        {
            var viewModel = new EconomyViewModel();

            switch (type)
            {
                case "ordinaryIncome":
                    var ordinaryIncome = await _context.OrdinaryIncomes.FindAsync(id);
                    if (ordinaryIncome == null) return NotFound();

                    viewModel.NewOrdinaryIncome = ordinaryIncome;
                    viewModel.ClubId = ordinaryIncome.ClubId;
                    viewModel.ModelType = nameof(OrdinaryIncome);
                    break;

                case "extraordinaryIncome":
                    var extraordinaryIncome = await _context.ExtraordinaryIncomes.FindAsync(id);
                    if (extraordinaryIncome == null) return NotFound();

                    viewModel.NewExtraordinaryIncome = extraordinaryIncome;
                    viewModel.ClubId = extraordinaryIncome.ClubId;
                    viewModel.ModelType = nameof(ExtraordinaryIncome);
                    break;

                case "ordinaryExpense":
                    var ordinaryExpense = await _context.OrdinaryExpenses.FindAsync(id);
                    if (ordinaryExpense == null) return NotFound();

                    viewModel.NewOrdinaryExpense = ordinaryExpense;
                    viewModel.ClubId = ordinaryExpense.ClubId;
                    viewModel.ModelType = nameof(OrdinaryExpense);
                    break;

                case "extraordinaryExpense":
                    var extraordinaryExpense = await _context.ExtraordinaryExpenses.FindAsync(id);
                    if (extraordinaryExpense == null) return NotFound();

                    viewModel.NewExtraordinaryExpense = extraordinaryExpense;
                    viewModel.ClubId = extraordinaryExpense.ClubId;
                    viewModel.ModelType = nameof(ExtraordinaryExpense);
                    break;

                default:
                    return NotFound();
            }

            return View("DetailEdit", viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PutEconomy(EconomyViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("DetailEdit", model);
            }

            switch (model.ModelType)
            {
                case nameof(OrdinaryIncome):
                    var ordinaryIncome = await _context.OrdinaryIncomes.FindAsync(model.NewOrdinaryIncome.Id);
                    if (ordinaryIncome == null) return NotFound();

                    ordinaryIncome.Type = model.NewOrdinaryIncome.Type;
                    ordinaryIncome.Amount = model.NewOrdinaryIncome.Amount;
                    ordinaryIncome.Description = model.NewOrdinaryIncome.Description;
                    ordinaryIncome.ClubId = model.ClubId;
                    break;

                case nameof(ExtraordinaryIncome):
                    var extraordinaryIncome = await _context.ExtraordinaryIncomes.FindAsync(model.NewExtraordinaryIncome.Id);
                    if (extraordinaryIncome == null) return NotFound();

                    extraordinaryIncome.Type = model.NewExtraordinaryIncome.Type;
                    extraordinaryIncome.Amount = model.NewExtraordinaryIncome.Amount;
                    extraordinaryIncome.Description = model.NewExtraordinaryIncome.Description;
                    extraordinaryIncome.ClubId = model.ClubId;
                    break;

                case nameof(OrdinaryExpense):
                    var ordinaryExpense = await _context.OrdinaryExpenses.FindAsync(model.NewOrdinaryExpense.Id);
                    if (ordinaryExpense == null) return NotFound();

                    ordinaryExpense.Type = model.NewOrdinaryExpense.Type;
                    ordinaryExpense.Amount = model.NewOrdinaryExpense.Amount;
                    ordinaryExpense.Description = model.NewOrdinaryExpense.Description;
                    ordinaryExpense.ClubId = model.ClubId;
                    break;

                case nameof(ExtraordinaryExpense):
                    var extraordinaryExpense = await _context.ExtraordinaryExpenses.FindAsync(model.NewExtraordinaryExpense.Id);
                    if (extraordinaryExpense == null) return NotFound();

                    extraordinaryExpense.Type = model.NewExtraordinaryExpense.Type;
                    extraordinaryExpense.Amount = model.NewExtraordinaryExpense.Amount;
                    extraordinaryExpense.Description = model.NewExtraordinaryExpense.Description;
                    extraordinaryExpense.ClubId = model.ClubId;
                    break;

                default:
                    return NotFound();
            }

            // Guardo el modelo que he modificado en el context
            await _context.SaveChangesAsync();

            // Recalculo los totales económicos
            await _salaryCapService.CalculateSalaryCap(model.ClubId);

            // Vuelvo a guardar para que los totales se actualicen
            await _context.SaveChangesAsync();

            return RedirectToAction("Detail", "Club", new { id = model.ClubId });
        }


        //BORRAR ECONOMIA

        [HttpGet]
        public async Task<IActionResult> DeleteEconomy(int id, string type)
        {
            object? recordEconomy = null;
            int? clubId = null;

            switch (type)
            {
                case "ordinaryIncome":
                    var ordinaryIncome = await _context.OrdinaryIncomes.FindAsync(id);
                    recordEconomy = ordinaryIncome;
                    clubId = ordinaryIncome?.ClubId;
                    break;
                case "ordinaryExpense":
                    var ordinaryExpense = await _context.OrdinaryExpenses.FindAsync(id);
                    recordEconomy = ordinaryExpense;
                    clubId = ordinaryExpense?.ClubId;
                    break;
                case "extraOrdinaryIncome":
                    var extraordinaryIncome = await _context.ExtraordinaryIncomes.FindAsync(id);
                    recordEconomy = extraordinaryIncome;
                    clubId = extraordinaryIncome?.ClubId;
                    break;
                case "extraOrdinaryExpense":
                    var extraordinaryExpense = await _context.ExtraordinaryExpenses.FindAsync(id);
                    recordEconomy = extraordinaryExpense;
                    clubId = extraordinaryExpense?.ClubId;
                    break;

                default:
                    return NotFound();
            }

            if (recordEconomy != null)
            {
                _context.Remove(recordEconomy);
                await _context.SaveChangesAsync();
            }

            if (clubId.HasValue)
            {
                await _salaryCapService.CalculateSalaryCap(clubId.Value);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction("Detail", "Club", new { id = clubId });
        }

    }
}
