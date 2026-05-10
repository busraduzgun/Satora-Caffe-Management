namespace SatoraCaffeRestaurantTracking.Models
{
    // Bu sınıf veritabanında tablo olmayacak!
    // Sadece müşteri sitede gezerken sepetini geçici olarak tutacak.
    public class CartItem
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public int Quantity { get; set; }     // Kaç tane istediği (Adet)
        public decimal Price { get; set; }    // Birim Fiyat
        public string Image { get; set; }     // Sepette resmi görünsün

        // Toplam Tutar (Adet x Fiyat)
        public decimal TotalPrice => Quantity * Price;
    }
}