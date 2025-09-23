using System.ComponentModel.DataAnnotations;

namespace ClubEconomyControl.Models
{
    public class ExtraordinaryExpense
    {
        public int Id { get; set; }
        public ExtraordinaryExpensetype Type { get; set; }
        public int Amount { get; set; }
        public string? Description { get; set; }
        //FKs
        public int ClubId { get; set; }
        public Club? Club { get; set; }
    }
}

public enum ExtraordinaryExpensetype
{
    [Display(Name = "Compra Jugador")]
    PlayerTransfer,
    [Display(Name = "Ampliación del Estadio")]
    StadiumUpgrade,
    [Display(Name = "Créditos/Deuda")]
    Debts,
    [Display(Name = "Otros Gastos Extra")]
    OtherExtraordinaryExpenses
}