using System.ComponentModel.DataAnnotations;
using ClubEconomyControl.Models.Interfaces;

namespace ClubEconomyControl.Models
{
    public class ExtraordinaryExpense : IEconomyModel
    {
        public int Id { get; set; }
        public ExtraordinaryExpenseType Type { get; set; }
        public int Amount { get; set; }
        public string? Description { get; set; }
        //FKs
        public int ClubId { get; set; }
        public Club? Club { get; set; }
    }
}

public enum ExtraordinaryExpenseType
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