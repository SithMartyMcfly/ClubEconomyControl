using ClubEconomyControl.Context;
using ClubEconomyControl.Models;


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
        public async Task<decimal> CalculateAnualAmotizationAsync(Player player)
        {
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
            if (YearsSigned > 5)
            {
                // TODO: Sacar este aviso por el front
                Console.WriteLine("El contrato no puede ser mayor de 5 años. Se amortiza en 5 años");
                YearsSigned = 5;
            }

            //control de años
            Console.WriteLine(YearsSigned);
            //Hacemos casteo de yearsSigned a decimal para evitar problemas de precisión
            var amortizationTransfer = player.TransferFeeBuy / (decimal)YearsSigned;

            //Coste anual en plantilla
            var amortizationRemaining = amortizationTransfer + player.Salary;

            return Math.Round(amortizationTransfer, 2);
        }
    }

}
