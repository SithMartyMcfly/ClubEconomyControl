using System.ComponentModel.DataAnnotations;
using ClubEconomyControl.Models.Interfaces;

namespace ClubEconomyControl.Models
{
    public class OrdinaryExpense : IEconomyModel
    {
        public int Id { get; set; }
        public OrdinaryExpenseType Type { get; set; }
        public decimal Amount { get; set; }
        public string? Description { get; set; }

        //FKs
        public int ClubId { get; set; }
        public Club? Club { get; set; }

    }
}
public enum OrdinaryExpenseType
{
    [Display(Name = "Salarios Empleados")]
    StaffWages,
    [Display(Name = "Mantenimiento Instalaciones")]
    StadiumMaintenance,
    [Display(Name = "Cantera")]
    YouthDevelopmentCosts,
    [Display(Name = "Otros gastos")]
    OtherExpenses
}
