namespace ClubEconomyControl.Models.Interfaces
{
    public interface IEconomyModel
    {
        int Id { get; set; }
        int Amount { get; set; }
        string? Description { get; set; }
        int ClubId { get; set; }
    }

}
