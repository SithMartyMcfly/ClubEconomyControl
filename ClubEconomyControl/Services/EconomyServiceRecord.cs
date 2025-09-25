using ClubEconomyControl.Context;
using ClubEconomyControl.Models.Interfaces;

namespace ClubEconomyControl.Services
{
    public class EconomyServiceRecord
    {
        private readonly ClubEconomyDbContext _context;

        public EconomyServiceRecord(ClubEconomyDbContext context)
        {
            _context = context;
        }

        public void AddRecord<T>(T record) where T : class, IEconomyModel
        {
            if (record.Amount > 0)
            {
                _context.Set<T>().Add(record);
            }
        }
        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
