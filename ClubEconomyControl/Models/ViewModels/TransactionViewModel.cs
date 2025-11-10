using System.ComponentModel.DataAnnotations;


namespace ClubEconomyControl.Models.ViewModels
{
    public class TransactionViewModel
    {
        public int Id { get; set; }

        [Required]
        public int PlayerId { get; set; }

        [Required]
        public int ClubId { get; set; }

        [Required(ErrorMessage = "Debes seleccionar un tipo de operación")]
        public TransactionType? Type { get; set; }

        public decimal? Amount { get; set; }

        public string? ReferenceCode { get; set; } // ← Hacerlo nullable

        public Player? player { get; set; } // ← Hacerlo nullable

        public Club? club { get; set; }     // ← Hacerlo nullable


    }
}
