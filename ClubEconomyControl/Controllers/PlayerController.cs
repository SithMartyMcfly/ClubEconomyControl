using ClubEconomyControl.Context;
using ClubEconomyControl.Models;
using ClubEconomyControl.Models.ViewModels;
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


        // GUARDAR JUGADORES
        // Get: /Club/Players/CreatePlayer
        [HttpGet]
        public IActionResult CreatePlayer(int ClubId)
        {
            var model = new TransactionViewModel
            {
                ClubId = ClubId,
                player = new Player() // inicializar para que asp-for="player.*" no falle
            };
            return View();
        }


        // Post: /Player/SavePlayer
        [HttpPost]

        //REPASAR EL AÑADIDO DEL VIEWMODEL NUEVO
        public async Task<IActionResult> SavePlayer(TransactionViewModel model, int ClubID)
        {

            // Validar que el club existe en la base de datos
            var clubExists = await _context.Clubs.AnyAsync(c => c.Id == ClubID);
            if (!clubExists)
            {
                ModelState.AddModelError("ClubId", "El club especificado no existe.");
                return View("CreatePlayer", model);
            }

            // Inicializar el jugador si viene null
            model.player ??= new Player();

            // Asignar ClubId al ViewModel y al jugador
            model.ClubId = ClubID;
            model.player.ClubId = ClubID;

            // Validar manualmente el objeto anidado
            TryValidateModel(model.player);

            // Mostrar errores de validación en consola
            foreach (var entry in ModelState)
            {
                foreach (var error in entry.Value.Errors)
                {
                    Console.WriteLine($"Campo: {entry.Key} → Error: {error.ErrorMessage}");
                }
            }

            if (!ModelState.IsValid)
            {
                Console.WriteLine("Error en guardado");
                return View("CreatePlayer", model);
            }

            // Iniciamos una transacción en la BBDD
            using var transaction = await _context.Database.BeginTransactionAsync();

            // Inicializar campos del jugador
            model.player.isSelled = false;
            model.player.BoughtFromClub ??= "Libre";

            // Calcular amortizaciones
            var amortizations = await _amortizationService.CalculateAmotizationAsync(model.player);
            model.player.AnnualExpense = amortizations.annualExpenseAmortization;
            model.player.AnualAmortization = amortizations.amortizationTransfer;
            model.player.RemainningAmortization = _amortizationService.CalculateRemainingAmortization(model.player);

            try
            {
                // Guardar jugador
                _context.Players.Add(model.player);
                await _context.SaveChangesAsync(); // model.player.Id ya disponible

                // Crear transacción
                var transactionEntity = new PlayerTransaction
                {
                    PlayerId = model.player.Id,
                    ClubId = model.player.ClubId,
                    Type = model.Type,
                    Amount = model.player.TransferFeeBuy,
                    ReferenceCode = $"BUY-{model.player.Id}-{model.player.ClubId}-{DateTime.Now:yyyyMM}"
                };

                _context.PlayerTransactions.Add(transactionEntity);
                await _context.SaveChangesAsync();

                // Crear gasto extraordinario
                var extraordinaryExpense = new ExtraordinaryExpense
                {
                    PlayerTransactionId = transactionEntity.Id,
                    ReferenceCode = transactionEntity.ReferenceCode,
                    Type = ExtraordinaryExpenseType.PlayerTransfer,
                    Amount = model.player.TransferFeeBuy,
                    ClubId = ClubID,
                    Description = $"Compra {model.player.Name}"
                };

                // Guardar transacción y gasto
                _context.ExtraordinaryExpenses.Add(extraordinaryExpense);
                await _context.SaveChangesAsync();

                // Actualizar límite salarial
                await _salaryCapService.CalculateSalaryCap(ClubID);
                // await _context.SaveChangesAsync();

                // Si todo va bien guarda en la BBDD
                await transaction.CommitAsync();
                // Redirigir a la lista de plantilla
                return RedirectToAction("SquadList", "Club", new { id = ClubID });

            }
            catch (Exception ex)
            {
                // Si no va bien todo, revierte los guardados realizados
                await transaction.RollbackAsync();
                Console.WriteLine($"Error en transacción: {ex.Message}");
                ModelState.AddModelError("", "Ocurrió un error al guardar los datos.");
                return View("CreatePlayer", model);

            }
        }

        // Get: Vista edición jugador
        [HttpGet]
        public async Task<IActionResult> EditPlayer(int id)
        {
            var player = await _context.Players.FindAsync(id);
            if (player == null)
            {
                return NotFound();
            }
            return View(player);
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

                if (playerToUpdate == null)
                    return NotFound();

                // Actualizar jugador
                playerToUpdate.Name = player.Name;
                playerToUpdate.TransferFeeBuy = player.TransferFeeBuy;
                playerToUpdate.Salary = player.Salary;
                playerToUpdate.BoughtFromClub = player.BoughtFromClub;
                playerToUpdate.ContractStartDate = player.ContractStartDate;
                playerToUpdate.ContractEndDate = player.ContractEndDate;


                // Datos dependientes del servicio Amortización
                var amortizations = await _amortizationService.CalculateAmotizationAsync(playerToUpdate);
                playerToUpdate.AnnualExpense = amortizations.annualExpenseAmortization;
                playerToUpdate.AnualAmortization = amortizations.amortizationTransfer;

                _context.Players.Update(playerToUpdate);
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
                Type = TransactionType.Sell, //TODO: Generar tipos
                Amount = player.TransferFeeSell,
                ReferenceCode = $"SELL-{player.Id}-{player.ClubId}-{DateTime.Now:yyyyMM}"
            };
            _context.PlayerTransactions.Add(transaction);

            //La venta genera un ExtraordinaryIncome
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

        // RENOVACIÓN

        //
        [HttpGet]
        public async Task<IActionResult> RenewPlayer(Player player)
        {
            return View(player);
        }

    }

}
