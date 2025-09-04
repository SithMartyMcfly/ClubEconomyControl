using System.ComponentModel.DataAnnotations;

namespace ClubEconomyControl.Models
{
    public class OrdinaryIncome
    {
        public int Id { get; set; }
        public OrdinaryIncomeType Type { get; set; }
        public int Amount { get; set; }
        //FKs
        public int ClubId { get; set; }
        public Club? Club { get; set; }
    }
}

public enum OrdinaryIncomeType
{

    Merchandising,
    [Display(Name = "Taquilla")]
    Tiketing,
    [Display(Name = "Premios")]
    Prizing,
    TV,
    [Display(Name = "Patrocinio")]
    Sponsorship,
    [Display(Name = "Otros ingresos")]
    OtherIncome
}