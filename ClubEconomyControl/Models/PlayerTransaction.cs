using System.ComponentModel.DataAnnotations;

namespace ClubEconomyControl.Models
{
    public class PlayerTransaction
    {
        public int Id { get; set; }
        public int PlayerId { get; set; }
        public int ClubId { get; set; }
        [Required(ErrorMessage = "es obligatoria una transacción")]
        [Display(Name = "Tipo operación")]
        public TransactionType? Type { get; set; }
        public decimal? Amount { get; set; }
        public string? ReferenceCode { get; set; }

        // Foreign Key
        public Player? Player { get; set; }
        public Club? Club { get; set; }
    }

    public enum TransactionType
    {
        [Display(Name = "Compra")]
        Buy,
        [Display(Name = "Venta")]
        Sell,
        [Display(Name = "Cesión")]
        Loan,
        [Display(Name = "Renovación")]
        Renew
    }
}
