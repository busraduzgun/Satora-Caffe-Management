using System.Collections.Generic;

namespace SatoraCaffeRestaurantTracking.Models
{
    public class OrderViewModel
    {
        // Masanın bilgileri (Masa No, Kapasite vs.)
        public RestaurantTable Table { get; set; }

        // Menüdeki Kategoriler (Tatlılar, İçecekler...)
        public List<Category> Categories { get; set; }

        // Tüm Ürünler
        public List<Product> Products { get; set; }

        // Eğer masada açık bir sipariş varsa, onun detayları
        public Order? CurrentOrder { get; set; }
    }
}