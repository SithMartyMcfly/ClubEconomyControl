
using System.ComponentModel.DataAnnotations;

namespace ClubEconomyControl.Models
{
    public class ExtraordinaryIncome
    {
        public int Id { get; set; }
        public ExtraordinaryIncomeType Type { get; set; }
        public int Amount { get; set; }
        public string? Description { get; set; }
        // Foreign Key
        public int ClubId { get; set; }
        public Club? Club { get; set; }
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
