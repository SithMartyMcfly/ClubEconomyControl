namespace ClubEconomyControl.Models
{
    public class Player
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int TransferFeeBuy { get; set; }
        public int TransferFeeSell { get; set; }
        public int Salary { get; set; }
        public int ContractStartDate { get; set; }
        public int ContractEndtDate { get; set; }
        public int AnnualAmortization { get; set; }
        public int AmortizationLeft { get; set; }

        // Foreign Key
        public int ClubId { get; set; }
        public Club? Club { get; set; }


        // Club al que se le compró (nullable)
        public int? BoughtFromClubId { get; set; }
        public Club? BoughtFromClub { get; set; }

        // Club al que se le vendió (nullable)
        public int? SoldToClubId { get; set; }
        public Club? SoldToClub { get; set; }


    }
}
