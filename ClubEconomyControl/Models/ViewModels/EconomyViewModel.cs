using System.ComponentModel.DataAnnotations;

namespace ClubEconomyControl.Models.ViewModels
{
    public class EconomyViewModel
    {
        [Required]
        public int ClubId { get; set; }

        //Creamos los objetos para añadir nuevos ingresos y gastos
        public OrdinaryIncome? NewOrdinaryIncome { get; set; } = new OrdinaryIncome();
        public ExtraordinaryIncome? NewExtraordinaryIncome { get; set; } = new ExtraordinaryIncome();
        public OrdinaryExpense? NewOrdinaryExpense { get; set; } = new OrdinaryExpense();
        public ExtraordinaryExpense? NewExtraordinaryExpense { get; set; } = new ExtraordinaryExpense();

    }
}
