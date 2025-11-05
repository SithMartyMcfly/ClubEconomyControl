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
            Console.WriteLine(JsonSerializer.Serialize(player));
            foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
            {

                Console.WriteLine($"Error de validación: {error.ErrorMessage}");
            }
            if (ModelState.IsValid)
            {
                // Controlamos datos del jugador y sus amortizaciones
                player.ClubId = ClubID;
                player.isSelled = false;
                player.BoughtFromClub ??= "Libre";

                // Pasamos el servicio amortizaciones y recogemos sus datos en el modelo Player
                var amortizations = await _amortizationService.CalculateAmotizationAsync(player);
                player.AnnualExpense = amortizations.annualExpenseAmortization;
                player.AnualAmortization = amortizations.amortizationTransfer;
                player.RemainningAmortization = _amortizationService.CalculateRemainingAmortization(player);

                // Añadimos el jugador al context
                _context.Players.Add(player);
                await _context.SaveChangesAsync();

                // Se genera una transacción de compra en PlayerTransaction
                var transaction = new PlayerTransaction
                {
                    PlayerId = player.Id,
                    ClubId = player.ClubId,
                    type = TransactionType.Buy,
                    Amount = player.TransferFeeBuy,
                    ReferenceCode = $"BUY-{player.Id}-{player.ClubId}-{DateTime.Now:yyyyMM}"
                };

                _context.PlayerTransactions.Add(transaction);
                await _context.SaveChangesAsync();

                //La compra genera un ExtraordinaryExpense
                var extraordinaryExpense = new ExtraordinaryExpense()
                {
                    PlayerTransactionId = transaction.Id,
                    ReferenceCode = transaction.ReferenceCode,
                    Type = ExtraordinaryExpenseType.PlayerTransfer,
                    Amount = player.TransferFeeBuy,
                    ClubId = ClubID,
                    Description = "Compra " + player.Name,
                };

                //Añadimos el nuevo ExtraordinaryExpense al contexto
                _context.ExtraordinaryExpenses.Add(extraordinaryExpense);

                // Pasamos el servicio de SalaryCap para actualizar el SquadLimitEconomy
                await _context.SaveChangesAsync();
                await _salaryCapService.CalculateSalaryCap(ClubID);
                await _context.SaveChangesAsync();

                return RedirectToAction("SquadList", "Club", new { id = ClubID });
            }
            else
            {
                Console.WriteLine("Error en guardado");
                return View("CreatePlayer");
            }
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
        //  REVISAR LOS GUARDADOS Y UPDATES DE LAS TRANSACCIONES Y GASTOS EXTRAORDINARIOS
        [HttpPost]
        public async Task<IActionResult> SaveEditPlayer(Player player, int ClubId)
        {
            if (ModelState.IsValid)
            {
                // Recuperamos el jugador con Id y ClubId que nos trae en parámetros
                var playerToUpdate = await _context.Players
                    .FirstOrDefaultAsync(p => p.Id == player.Id && p.ClubId == ClubId);

                // Actualizar los campos editables
                if (playerToUpdate != null)
                {
                    playerToUpdate.Name = player.Name;
                    playerToUpdate.TransferFeeBuy = player.TransferFeeBuy;
                    playerToUpdate.Salary = player.Salary;
                    playerToUpdate.BoughtFromClub = player.BoughtFromClub;
                    playerToUpdate.ContractStartDate = player.ContractStartDate;
                    playerToUpdate.ContractEndDate = player.ContractEndDate;
                }

                // Datos dependientes del servicio Amortización
                var amortizations = await _amortizationService.CalculateAmotizationAsync(playerToUpdate);
                playerToUpdate.AnnualExpense = amortizations.annualExpenseAmortization;
                playerToUpdate.AnualAmortization = amortizations.amortizationTransfer;
                _context.Update(playerToUpdate);
                await _context.SaveChangesAsync();

                // Actualizar la PlayerTransaction
                var playertransaction = await _context.PlayerTransactions
                    .FirstOrDefaultAsync(pt => pt.PlayerId == player.Id && pt.ClubId == ClubId);

                if (playertransaction != null)
                {
                    playertransaction.Amount = player.TransferFeeBuy;
                    _context.PlayerTransactions.Update(playertransaction);
                }

                // Actualizar ExtraordinaryExpense
                var extraordinaryExpense = await _context.ExtraordinaryExpenses
                    .FirstOrDefaultAsync(ee => ee.ReferenceCode == playertransaction.ReferenceCode);

                if (extraordinaryExpense != null)
                {
                    extraordinaryExpense.Amount = player.TransferFeeBuy;
                    extraordinaryExpense.Description = $"Editado compra de {playerToUpdate.Name}";
                    _context.ExtraordinaryExpenses.Update(extraordinaryExpense);
                }


                // Guardar los cambios en la base de datos
                await _context.SaveChangesAsync();
                await _salaryCapService.CalculateSalaryCap(ClubId);
                await _context.SaveChangesAsync();

                return RedirectToAction("SquadList", "Club", new { id = ClubId });
            }
            else
            {
                Console.WriteLine("Error en guardado de edición");
                return View("EditPlayer", player);
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

            _context.Players.Update(playerSelled);

            var transaction = new PlayerTransaction()
            {
                PlayerId = player.Id,
                ClubId = player.ClubId,
                type = TransactionType.Sell, //TODO: Generar tipos
                Amount = player.TransferFeeSell,
                ReferenceCode = $"SELL-{player.Id}-{player.ClubId}-{DateTime.Now:yyyyMM}"
            };
            _context.PlayerTransactions.Add(transaction);

            //La venta genera un ExtraordinaryIncome
            // TODO: CREO QUE DEBERIAMOS ASIGNAR CON EL REFCODE Y EL ID
            var ExtraordinaryIncome = new ExtraordinaryIncome()
            {
                ReferenceCode = transaction.ReferenceCode,
                Type = ExtraordinaryIncomeType.PlayerSale,
                Amount = (decimal)playerSelled.TransferFeeSell,
                ClubId = ClubId,
                Description = "Venta " + playerSelled.Name,
            };

            //Añadimos el nuevo ExtraordinaryIncome al contexto
            _context.ExtraordinaryIncomes.Add(ExtraordinaryIncome);

            //Actualización del límite salarial del club y el nuevo balance
            // Primero guardamos en BBDD todos los cambios en las entidades
            await _context.SaveChangesAsync();
            // Luego llamamos al servicio de SalaryCap para recalcular el SquadLimitEconomy
            var club = await _salaryCapService.CalculateSalaryCapSale(ClubId, player.RemainningAmortization);

            //Salvar Datos
            await _context.SaveChangesAsync();

            Console.WriteLine(club.SquadLimitEconomy + "LIMITE SALARIAL");

            //Redirección
            return RedirectToAction("Index", "Club", new { ClubId = ClubId });
        }


    }

}
