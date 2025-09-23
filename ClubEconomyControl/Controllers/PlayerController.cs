using System.Text.Json;
using ClubEconomyControl.Context;
using ClubEconomyControl.Models;
using Microsoft.AspNetCore.Mvc;

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

    }

}
