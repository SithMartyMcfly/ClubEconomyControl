namespace ClubEconomyControl.Services
{
    public class ReferenceCode
    {

        public string GeneratorEconomyCode(string type, int clubId)
        {
            var prefix = type.Substring(0, 4).ToUpper();
            int month = DateTime.Now.Month;
            int year = DateTime.Now.Year;
            var economyCode = $"{prefix}-{clubId}-{month}-{year}";

            return economyCode;
        }

    }
}
