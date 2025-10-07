using System.ComponentModel.DataAnnotations;

namespace ClubEconomyControl.Models
{
    public class Player
    {
        public int Id { get; set; }
        [Display(Name = "Nombre")]
        public string Name { get; set; }
        [Display(Name = "Valor Compra")]
        public decimal TransferFeeBuy { get; set; }
        [Display(Name = "Valor Venta")]
        public decimal? TransferFeeSell { get; set; }
        [Display(Name = "Sueldo")]
        public decimal Salary { get; set; }
        [Display(Name = "Fecha Inicio Contrato")]
        public DateTime ContractStartDate { get; set; }
        [Display(Name = "Fecha Finalización Contrato")]
        public DateTime ContractEndDate { get; set; }
        [Display(Name = "Vendido")]
        public Boolean isSelled { get; set; } = false;
        public decimal? AnualAmortization { get; set; }
        public decimal? RemainningAmortization { get; set; }

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
