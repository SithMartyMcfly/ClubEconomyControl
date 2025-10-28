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
        public async Task<(decimal annualExpenseAmortization, decimal amortizationTransfer)> CalculateAmotizationAsync(Player player)
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

            // Control limite de años, la amortización no puede ser mayor a 5 años
            if (YearsSigned < 1)
                throw new ArgumentException("El contrato debe ser de al menos 1 año.");
            if (YearsSigned > 5)
            {
                Console.WriteLine("El contrato no puede ser superior a 5 años");
                YearsSigned = 5;  //ajustamos a 5 años la amortización
            }


            //control de años
            Console.WriteLine(YearsSigned);

            //Hacemos casteo de yearsSigned a decimal para evitar problemas de precisión
            var amortizationTransfer = player.TransferFeeBuy / (decimal)YearsSigned;
            var annualExpenseAmortization = amortizationTransfer + player.Salary;


            return (Math.Round(annualExpenseAmortization, 2),
                Math.Round(amortizationTransfer, 2));
        }


        public decimal CalculateRemainingAmortization(Player player)
        {
            // Control de excepciones
            if (player == null)
                throw new ArgumentNullException(nameof(player));

            if (player.AnualAmortization == null)
                throw new InvalidOperationException("La amortización anual no está definida.");

            if (player.ContractStartDate == default)
                throw new InvalidOperationException("La fecha de inicio de contrato no está definida.");

            // Calculo los meses transcurridos desde el inicio del contrato
            var elapsedMonths = ((DateTime.Now.Year - player.ContractStartDate.Year) * 12)
                              + (DateTime.Now.Month - player.ContractStartDate.Month);

            // TODO: Arreglar el if/else
            if (elapsedMonths < 0)
                elapsedMonths = 0;
            if (elapsedMonths > 60)
            {
                elapsedMonths = 60;
                Console.WriteLine("SE AJUSTA A 5 AÑOS LA AMORTIZACIÓN");
            }
            // Calculo la amortización mensual
            var monthAmortization = player.AnualAmortization.Value / 12;

            // Calculo la amortización restante
            var remainingAmortization = player.TransferFeeBuy - (monthAmortization * elapsedMonths);

            // Devuelvo la amortización restante redondeada a 2 decimales
            return Math.Round(remainingAmortization, 2);
        }








    }

}
