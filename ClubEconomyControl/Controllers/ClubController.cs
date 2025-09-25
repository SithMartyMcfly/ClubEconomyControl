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

        public ClubController(ClubEconomyDbContext context, BalanceService balanceService)
        {
            _context = context;
            _balanceService = balanceService;
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
            //Retornamos la lista de jugadores
            return View(await _context.Players
                .Where(p => p.ClubId == id && p.isSelled == false)
                .ToListAsync()
                );
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

        // Get: /Club/Economy/Edit
        [HttpGet]
        public async Task<IActionResult> EditEconomy(int id, string type)
        {
            switch (type)
            {
                case "ordinaryIncome":
                    var ordinaryIncomeRecord = await _context.OrdinaryIncomes.FindAsync(id);
                    return View("DetailEdit", ordinaryIncomeRecord);
                case "extraordinaryIncome":
                    var extraordinaryIncomeRecord = await _context.ExtraordinaryIncomes.FindAsync(id);
                    return View("DetailEdit", extraordinaryIncomeRecord);
                case "ordinaryExpense":
                    var ordinaryExpenseRecord = await _context.OrdinaryExpenses.FindAsync(id);
                    return View("DetailEdit", ordinaryExpenseRecord);
                case "extraordinaryExpense":
                    var extraordinaryExpenseRecord = await _context.ExtraordinaryExpenses.FindAsync(id);
                    return View("DetailEdit", extraordinaryExpenseRecord);
                default:
                    return NotFound();




            }
        }

        /*// Put: /Club/Economy/Edit
        [HttpPut]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditEconomy(int id, EconomyViewModel model)
        {
            if (id != model.ClubId)
            {
                return NotFound();
            }
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

            if (model.NewOrdinaryIncome?.Id > 0)
            {
                model.NewOrdinaryIncome.ClubId = model.ClubId;
                _context.OrdinaryIncomes.Update(model.NewOrdinaryIncome);
            }
            if (model.NewExtraordinaryIncome?.Id > 0)
            {
                model.NewExtraordinaryIncome.ClubId = model.ClubId;
                _context.ExtraordinaryIncomes.Update(model.NewExtraordinaryIncome);
            }
            if (model.NewOrdinaryExpense?.Id > 0)
            {
                model.NewOrdinaryExpense.ClubId = model.ClubId;
                _context.OrdinaryExpenses.Update(model.NewOrdinaryExpense);
            }
            if (model.NewExtraordinaryExpense?.Id > 0)
            {
                model.NewExtraordinaryExpense.ClubId = model.ClubId;
                _context.ExtraordinaryExpenses.Update(model.NewExtraordinaryExpense);
            }
            await _context.SaveChangesAsync();

            return RedirectToAction("Detail", "Club", new { id = model.ClubId });
        }*/


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
