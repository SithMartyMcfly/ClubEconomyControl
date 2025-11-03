
using System.ComponentModel.DataAnnotations;
using ClubEconomyControl.Models.Interfaces;

namespace ClubEconomyControl.Models
{
    public class ExtraordinaryIncome : IEconomyModel
    {
        public int Id { get; set; }
        public ExtraordinaryIncomeType Type { get; set; }
        public decimal Amount { get; set; }
        public string? Description { get; set; }
        public int? PlayerTransactionId { get; set; }
        public string? ReferenceCode { get; set; }

        // Foreign Key
        public int ClubId { get; set; }
        public Club? Club { get; set; }
        public PlayerTransaction? PlayerTransaction { get; set; }
    }
    public enum ExtraordinaryIncomeType
    {
        [Display(Name = "Venta Jugadores")]
        PlayerSale,
        [Display(Name = "Injeción de Capital")]
        CapitalInjection,
        [Display(Name = "Otros Ingresos Extra")]
        OtherExtraordinaryIncome
    }
}
