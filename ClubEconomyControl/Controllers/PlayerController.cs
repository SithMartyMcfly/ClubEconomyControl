using System.Text.Json;
using ClubEconomyControl.Context;
using ClubEconomyControl.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ClubEconomyControl.Controllers
{
    public class PlayerController : Controller
    {
        private readonly ClubEconomyDbContext _context;
        public PlayerController(ClubEconomyDbContext context)
        {
            _context = context;
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
                var extraordinaryExpense = new ExtraordinaryExpense()
                {
                    Type = ExtraordinaryExpensetype.PlayerTransfer,
                    Amount = (int)player.TransferFeeBuy, //cambiar tipo de dato a INT en PLAYER
                    ClubId = ClubID,
                    Description = "Compra " + player.Name,
                };
                _context.ExtraordinaryExpenses.Add(extraordinaryExpense);
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

        public async Task<IActionResult> SellPlayer(int Id, int ClubId, int TransferFeeSell, string SoldToClub)
        {
            //Primero buscamos el jugador que tenga las IDs correctas
            var playerSelled = await _context.Players
                .FirstOrDefaultAsync(p => p.Id == Id && p.ClubId == ClubId);
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
            playerSelled.TransferFeeSell = TransferFeeSell;
            playerSelled.SoldToClub = SoldToClub;
            playerSelled.isSelled = true;
            playerSelled.ContractEndDate = DateTime.Today;

            //La venta genera un ExtraordinaryIncome
            var ExtraordinaryIncome = new ExtraordinaryIncome()
            {
                Type = ExtraordinaryIncomeType.PlayerSale,
                Amount = TransferFeeSell,
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

    }

}
