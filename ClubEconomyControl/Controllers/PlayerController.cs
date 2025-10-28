using System.Text.Json;
using ClubEconomyControl.Context;
using ClubEconomyControl.Models;
using ClubEconomyControl.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ClubEconomyControl.Controllers
{
    public class PlayerController : Controller
    {
        private readonly ClubEconomyDbContext _context;
        private readonly AmortizationService _amortizationService;
        private readonly SalaryCapService _salaryCapService;
        public PlayerController(ClubEconomyDbContext context, AmortizationService amortizationService, SalaryCapService salaryCapService)
        {
            _context = context;
            _amortizationService = amortizationService;
            _salaryCapService = salaryCapService;
        }

        // Get: /Club/Players/CreatePlayer
        [HttpGet]
        public IActionResult CreatePlayer(int ClubId)
        {
            return View();
        }

        // Get: /Club/Players/ViewPlayer
        [HttpGet]
        public async Task<IActionResult> Detail(int? id)
        {
            if (id == null)
                return NotFound();
            var player = await _context.Players.FindAsync(id);
            if (player == null)
                return NotFound();
            return View(player);
        }

        // Post: /Player/SavePlayer
        [HttpPost]
        public async Task<IActionResult> SavePlayer(Player player, int ClubID)
        {
            player.ClubId = ClubID;
            player.isSelled = false;


            Console.WriteLine(JsonSerializer.Serialize(player));
            foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
            {

                Console.WriteLine($"Error de validación: {error.ErrorMessage}");
            }
            if (ModelState.IsValid)
            {
                if (string.IsNullOrEmpty(player.BoughtFromClub))
                {
                    player.BoughtFromClub = "Libre";
                }
                _context.Players.Add(player);

                //La compra genera un ExtraordinaryExpense
                var extraordinaryExpense = new ExtraordinaryExpense()
                {
                    Type = ExtraordinaryExpenseType.PlayerTransfer,
                    Amount = (int)player.TransferFeeBuy, //cambiar tipo de dato a INT en PLAYER
                    ClubId = ClubID,
                    Description = "Compra " + player.Name,
                };

                //Añadimos el nuevo ExtraordinaryExpense al contexto
                _context.ExtraordinaryExpenses.Add(extraordinaryExpense);

                // Añadimos la amortización anual del jugador
                var amortizations = await _amortizationService.CalculateAmotizationAsync(player);
                var remainingAmortization = _amortizationService.CalculateRemainingAmortization(player);

                // Añadimos los valores de la Tupla de CalculateAmortizationAsync a la BBDD
                player.AnnualExpense = amortizations.annualExpenseAmortization;
                player.AnualAmortization = amortizations.amortizationTransfer;
                player.RemainningAmortization = remainingAmortization;

                // Pasamos el servicio de SalaryCap para actualizar el SquadLimitEconomy
                await _salaryCapService.CalculateSalaryCap(ClubID, player.AnnualExpense);

                await _context.SaveChangesAsync();
                return RedirectToAction("SquadList", "Club", new { id = ClubID });
            }
            else
            {
                Console.WriteLine("Error en guardado");
                return View("CreatePlayer");
            }
        }

        // Post: Método Venta jugador
        [HttpPost]
        public async Task<IActionResult> SellPlayer(int ClubId, Player player)
        {

            //Primero buscamos el jugador que tenga las IDs correctas
            var playerSelled = await _context.Players.FindAsync(player.Id);

            if (playerSelled == null)
            {
                return NotFound();
            }

            /*esta validación queda hecha para saber hacer, no tendría efecto
             puesto que no se muestran jugadores que tengan el camo isSelled*/
            if (playerSelled.isSelled)
            {
                Console.WriteLine("El jugador fue Vendido antes");
                TempData["ErrorMessage"] = $"El jugador {playerSelled.Name} ya ha sido vendido.";
                return RedirectToAction("Index", "Club", new { ClubId = ClubId });
            }

            //Asignación de datos Tabla Player
            playerSelled.TransferFeeSell = player.TransferFeeSell;
            playerSelled.SoldToClub = player.SoldToClub;
            playerSelled.isSelled = true;
            playerSelled.ContractEndDate = DateTime.Today;
            playerSelled.RemainningAmortization = player.RemainningAmortization;

            //Actualización del límite salarial del club
            await _salaryCapService.CalculateSalaryCap(ClubId, playerSelled.RemainningAmortization);

            //La venta genera un ExtraordinaryIncome
            var ExtraordinaryIncome = new ExtraordinaryIncome()
            {
                Type = ExtraordinaryIncomeType.PlayerSale,
                Amount = (decimal)playerSelled.TransferFeeSell,
                ClubId = ClubId,
                Description = "Venta " + playerSelled.Name,
            };
            //Añadimos el nuevo ExtraordinaryIncome al contexto
            _context.ExtraordinaryIncomes.Add(ExtraordinaryIncome);

            //Salvar Datos
            await _context.SaveChangesAsync();

            //Redirección
            return RedirectToAction("Index", "Club", new { ClubId = ClubId });
        }

        //GET: Edición Jugador
        [HttpGet]
        public async Task<IActionResult> EditPlayer(int id, int ClubId)
        {
            var player = await _context.Players.FindAsync(id);
            if (player == null)
            {
                return NotFound();
            }
            return View("EditPlayer", player);

        }

        //POST: Guardar Edición Jugador
        [HttpPost]
        public async Task<IActionResult> SaveEditPlayer(Player player, int ClubId)
        {
            if (ModelState.IsValid)
            {
                // Recuperamos el jugador con Id y ClubId que nos trae en parámetros
                var playerToUpdate = await _context.Players
                    .FirstOrDefaultAsync(p => p.Id == player.Id && p.ClubId == ClubId);

                if (playerToUpdate == null)
                {
                    return NotFound();
                }
                // Actualizar los campos editables
                playerToUpdate.Name = player.Name;
                playerToUpdate.TransferFeeBuy = player.TransferFeeBuy;
                playerToUpdate.Salary = player.Salary;
                playerToUpdate.BoughtFromClub = player.BoughtFromClub;
                playerToUpdate.ContractStartDate = player.ContractStartDate;
                playerToUpdate.ContractEndDate = player.ContractEndDate;

                var amortizations = await _amortizationService.CalculateAmotizationAsync(playerToUpdate);
                playerToUpdate.AnnualExpense = amortizations.annualExpenseAmortization;
                playerToUpdate.AnualAmortization = amortizations.amortizationTransfer;
                // Guardar los cambios en la base de datos

                await _context.SaveChangesAsync();
                return RedirectToAction("SquadList", "Club", new { id = ClubId });
            }
            else
            {
                Console.WriteLine("Error en guardado de edición");
                return View("EditPlayer", player);
            }
        }


    }

}
