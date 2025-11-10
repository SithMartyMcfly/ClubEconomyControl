using System.ComponentModel.DataAnnotations;

namespace ClubEconomyControl.Models
{
    public class Player
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio")]
        [Display(Name = "Nombre")]
        public string Name { get; set; }

        [Required(ErrorMessage = "El valor de compra es obligatorio")]
        [Display(Name = "Valor Compra")]
        [Range(0, double.MaxValue, ErrorMessage = "El valor debe superior a 0")]
        public decimal TransferFeeBuy { get; set; }

        [Display(Name = "Valor Venta")]
        [Range(0, double.MaxValue, ErrorMessage = ("el valor debe ser superior a cero"))]
        public decimal? TransferFeeSell { get; set; }

        [Display(Name = "Sueldo")]
        [Range(0, double.MaxValue)]
        public decimal Salary { get; set; }

        [Required(ErrorMessage = "La fecha de inicio es obligatoria")]
        [Display(Name = "Fecha Inicio Contrato")]
        public DateTime ContractStartDate { get; set; }

        [Required(ErrorMessage = "La fecha de finalización es obligatoria")]
        [Display(Name = "Fecha Finalización Contrato")]
        public DateTime ContractEndDate { get; set; }

        [Display(Name = "Vendido")]
        public Boolean isSelled { get; set; } = false;

        public decimal? AnualAmortization { get; set; }
        public decimal? RemainningAmortization { get; set; }
        public decimal AnnualExpense { get; set; }


        // Foreign Key
        public int ClubId { get; set; }
        public Club? Club { get; set; }


        // Club al que se le compró (nullable)
        [Display(Name = "Club Procedencia")]
        public string? BoughtFromClub { get; set; }

        // Club al que se le vendió (nullable)
        [Display(Name = "Club Destino")]
        public string? SoldToClub { get; set; }


    }
}
