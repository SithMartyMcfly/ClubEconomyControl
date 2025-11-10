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
        private readonly SalaryCapService _salaryCapService;
        private readonly ReferenceCode _referenceCode;


        public ClubController(ClubEconomyDbContext context, BalanceService balanceService, AmortizationService amortizationService, SalaryCapService salaryCapService, ReferenceCode referenceCode)
        {
            _context = context;
            _balanceService = balanceService;
            _amortizationService = amortizationService;
            _salaryCapService = salaryCapService;
            _referenceCode = referenceCode;
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
                await _amortizationService.UpdateAmortizationValuesAsnync(player);
                dictionaryAmortizations[player.Id] = player.RemainningAmortization ?? 0;
            }

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
                model.NewOrdinaryIncome.ReferenceCode = _referenceCode.GeneratorEconomyCode(model.NewOrdinaryIncome.Type.ToString(), model.ClubId);
                _context.OrdinaryIncomes.Add(model.NewOrdinaryIncome);
            }

            if (model.NewOrdinaryExpense?.Amount > 0)
            {
                model.NewOrdinaryExpense.ClubId = model.ClubId;
                model.NewOrdinaryExpense.ReferenceCode = _referenceCode.GeneratorEconomyCode(model.NewOrdinaryExpense.Type.ToString(), model.ClubId);
                _context.OrdinaryExpenses.Add(model.NewOrdinaryExpense);
            }

            if (model.NewExtraordinaryIncome?.Amount > 0)
            {
                model.NewExtraordinaryIncome.ClubId = model.ClubId;
                model.NewExtraordinaryIncome.ReferenceCode = _referenceCode.GeneratorEconomyCode(model.NewExtraordinaryIncome.Type.ToString(), model.ClubId);
                _context.ExtraordinaryIncomes.Add(model.NewExtraordinaryIncome);
            }

            if (model.NewExtraordinaryExpense?.Amount > 0)
            {
                model.NewExtraordinaryExpense.ClubId = model.ClubId;
                model.NewExtraordinaryExpense.ReferenceCode = _referenceCode.GeneratorEconomyCode(model.NewExtraordinaryExpense.Type.ToString(), model.ClubId);
                _context.ExtraordinaryExpenses.Add(model.NewExtraordinaryExpense);
            }

            await _context.SaveChangesAsync();
            await _salaryCapService.CalculateSalaryCap(model.ClubId);
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
    }
}
