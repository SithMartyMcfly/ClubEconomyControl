using ClubEconomyControl.Context;
using ClubEconomyControl.Models;
using ClubEconomyControl.Models.ViewModels;
using ClubEconomyControl.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ClubEconomyControl.Controllers
{
    public class ClubController : Controller
    {
        private readonly ClubEconomyDbContext _context;
        private readonly BalanceService _balanceService;
        private readonly AmortizationService _amortizationService;

        public ClubController(ClubEconomyDbContext context, BalanceService balanceService, AmortizationService amortizationService)
        {
            _context = context;
            _balanceService = balanceService;
            _amortizationService = amortizationService;
        }

        // Get: /Club
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            return View(await _context.Clubs.ToListAsync());
        }

        // Get: /Club/CreateClub
        [HttpGet]
        public IActionResult CreateClub()
        {
            return View();
        }





        // Post: /Club/CreateEconomy
        //Debemos recibir el id del club para poder mantenerlo y hacer un guardado correcto
        public IActionResult CreateEconomy(int id)
        {
            var model = new EconomyViewModel
            {
                ClubId = id
            };
            //y devolverlo
            return View(model);
        }



        // Get: /Club/Squad
        [HttpGet]
        public async Task<IActionResult> SquadList(int? id)
        {
            if (id == null)
                return NotFound();

            var club = await _context.Clubs
                .FindAsync(id);
            if (club == null)
                return NotFound();
            //Recogemos con ViewBag solo el nombre del club
            ViewBag.ClubName = club.Name;
            ViewBag.ClubId = club.Id;

            // Mostrar la amortización restante de cada jugador

            // Recuperamos la lista de jugadores del club
            var players = await _context.Players
                .Where(p => p.ClubId == id && p.isSelled == false)
                .ToListAsync();

            // Iniciamos un Diccionario
            var dictionaryAmortizations = new Dictionary<int, decimal>();

            // Añadimos a cada jugador su Amortización
            foreach (var player in players)
            {
                var remaining = await _amortizationService.CalculateRemainingAmortization(player);
                dictionaryAmortizations[player.Id] = remaining;
            }

            // Con un ViewBag sacamos el diccionario
            ViewBag.Amortizations = dictionaryAmortizations;


            //Retornamos la lista de jugadores
            return View(players);
        }

        // Get: /Club/Details/
        [HttpGet]
        public async Task<IActionResult> Detail(int? id)
        {
            var club = await _context.Clubs
                //tenemos que incluir los ingresos y gastos para poder calcular el balance,
                //para poder sacar en la vista los ingresos y gastos totales
                .Include(c => c.OrdinaryIncomes)
                .Include(c => c.ExtraordinaryIncomes)
                .Include(c => c.OrdinaryExpenses)
                .Include(c => c.ExtraordinaryExpenses)
                .Include(c => c.Players)
                .FirstOrDefaultAsync(m => m.Id == id);
            //usamos el servicio para calcular el balance
            var balance = await _balanceService.CalculateBalanceAsync(club);
            // Enviar el balance a la vista usando ViewBag
            ViewBag.Balance = balance;
            return View(club);
        }


        // Post: /Club/Save
        [HttpPost]
        public async Task<IActionResult> Save(Club club)
        {
            //Hemos hecho una depuración usando Console.WriteLine para ver qué datos llegan y si hay errores de validación
            Console.WriteLine($"Nombre recibido: {club.Name}");
            foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
            {
                Console.WriteLine($"Error de validación: {error.ErrorMessage}");
            }
            Console.WriteLine("Metodo Save ejecute");
            if (ModelState.IsValid)
            {
                _context.Clubs.Add(club);
                await _context.SaveChangesAsync();
                Console.WriteLine("Guardado");
                return RedirectToAction("Index");

            }
            Console.WriteLine("Modelo error");
            return View("Create", club); // En caso de error, vuelve al formulario
        }

        // Post: /Club/SaveEconomy
        [HttpPost]
        public async Task<IActionResult> SaveEconomy(EconomyViewModel model)
        {

            if (!ModelState.IsValid)
            {
                foreach (var kvp in ModelState)
                {
                    foreach (var error in kvp.Value.Errors)
                    {
                        Console.WriteLine($"Error en {kvp.Key}: {error.ErrorMessage}");
                    }
                }

                return RedirectToAction("Index", "Club");
            }
            if (model.NewOrdinaryIncome?.Amount > 0)
            {
                model.NewOrdinaryIncome.ClubId = model.ClubId;
                _context.OrdinaryIncomes.Add(model.NewOrdinaryIncome);
            }

            if (model.NewExtraordinaryIncome?.Amount > 0)
            {
                model.NewExtraordinaryIncome.ClubId = model.ClubId;
                _context.ExtraordinaryIncomes.Add(model.NewExtraordinaryIncome);
            }

            if (model.NewOrdinaryExpense?.Amount > 0)
            {
                model.NewOrdinaryExpense.ClubId = model.ClubId;
                _context.OrdinaryExpenses.Add(model.NewOrdinaryExpense);
            }

            if (model.NewExtraordinaryExpense?.Amount > 0)
            {
                model.NewExtraordinaryExpense.ClubId = model.ClubId;
                _context.ExtraordinaryExpenses.Add(model.NewExtraordinaryExpense);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction("Detail", "Club", new { id = model.ClubId });
        }

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

            await _context.SaveChangesAsync();
            return RedirectToAction("Detail", "Club", new { id = model.ClubId });
        }


        // Delete: /Club/Delete
        public async Task<IActionResult> Delete(int id)
        {
            var club = await _context.Clubs.FindAsync(id);
            if (club != null)
            {
                _context.Remove(club);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            else
            {
                return NotFound();
            }
        }

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
                return RedirectToAction("Detail", new { id = clubId });
            }
            else
            {
                return NotFound();
            }
        }

    }
}
