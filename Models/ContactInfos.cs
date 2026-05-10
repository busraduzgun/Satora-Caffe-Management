namespace SatoraCaffeRestaurantTracking.Models;

public partial class ContactInfos
{
    public int Id { get; set; }
    public string Address { get; set; } = null!;
    public string Telephone { get; set; } = null!;
    public string? Email { get; set; }
    public string? MapLocation { get; set; }
    public string? OpeningHours { get; set; }
    public bool Status { get; set; }
}