using System.ComponentModel.DataAnnotations;

namespace ClubEconomyControl.Models
{
    public class Club
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre del club es obligatorio")]
        public string Name { get; set; }
        public decimal? SquadLimitEconomy { get; set; } = 0;
        public decimal? Balance { get; set; } = 0;

        // Propiedades de navegación desde club hacia otras entidades
        // Hay que iniciar las listas aunque vayan a quedar en nulas para evitar errores con EF
        public ICollection<OrdinaryIncome> OrdinaryIncomes { get; set; } = new List<OrdinaryIncome>();
        public ICollection<ExtraordinaryIncome> ExtraordinaryIncomes { get; set; } = new List<ExtraordinaryIncome>();
        public ICollection<OrdinaryExpense> OrdinaryExpenses { get; set; } = new List<OrdinaryExpense>();
        public ICollection<ExtraordinaryExpense> ExtraordinaryExpenses { get; set; } = new List<ExtraordinaryExpense>();
        public ICollection<Player> Players { get; set; } = new List<Player>();


    }
}
