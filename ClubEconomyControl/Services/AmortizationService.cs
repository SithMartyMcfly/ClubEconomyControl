using ClubEconomyControl.Context;


namespace ClubEconomyControl.Services
{
    public class AmortizationService
    {
        private readonly ClubEconomyDbContext _context;

        public AmortizationService(ClubEconomyDbContext context)
        {
            _context = context;
        }


        //Calcula la amortización anual de un jugador dado su ID
        public async Task<decimal> CalculateAmotizationAsync(int playerId)
        {
            var player = await _context.Players.FindAsync(playerId);

            if (player == null)
                throw new ArgumentException("Jugador no encontrado.");

            var inicio = player.ContractStartDate;
            var fin = player.ContractEndDate;

            //control de fechas
            if (fin <= inicio)
                throw new ArgumentException("La fecha de fin de contrato debe ser posterior a la fecha de inicio.");

            var DaysSigned = (fin - inicio).TotalDays;
            var YearsSigned = DaysSigned / 365;
            if (YearsSigned < 1)
                throw new ArgumentException("El contrato debe ser de al menos un año.");

            //control de años
            Console.WriteLine(YearsSigned);

            //Hacemos casteo de yearsSigned a decimal para evitar problemas de precisión
            var amortizationTransfer = player.TransferFeeBuy / (decimal)YearsSigned;

            var amortization = amortizationTransfer + player.Salary;


            return Math.Round(amortization, 1);
        }
    }

}
