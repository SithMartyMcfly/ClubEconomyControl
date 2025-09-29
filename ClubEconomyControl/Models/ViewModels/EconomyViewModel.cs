using System.ComponentModel.DataAnnotations;

namespace ClubEconomyControl.Models.ViewModels
{
    public class EconomyViewModel
    {
        [Required]
        public int ClubId { get; set; }
        public string? ModelType { get; set; }

        //Creamos los objetos para añadir nuevos ingresos y gastos
        public OrdinaryIncome? NewOrdinaryIncome { get; set; }
        public ExtraordinaryIncome? NewExtraordinaryIncome { get; set; }
        public OrdinaryExpense? NewOrdinaryExpense { get; set; }
        public ExtraordinaryExpense? NewExtraordinaryExpense { get; set; }
    }
}
