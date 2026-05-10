namespace SatoraCaffeRestaurantTracking.Models;

public partial class Messages
{
    public int Id { get; set; }
    public string SenderName { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string? Subject { get; set; }
    public string Content { get; set; } = null!;
    public DateTime SendDate { get; set; }
    public bool IsRead { get; set; }
    public string? ReplyContent { get; set; } // adminin berdiği cevap
    public DateTime? ReplyDate { get; set; }  // adminin cevap verdiği tarih

    // Müşteri Bağlantısı
    public int? CustomerId { get; set; }
    public virtual Customer? Customer { get; set; }
}