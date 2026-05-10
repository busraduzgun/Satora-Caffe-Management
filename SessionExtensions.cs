using Microsoft.AspNetCore.Http;
using Newtonsoft.Json; // Bu kütüphane listeyi metne çevirip saklar

namespace SatoraCaffeRestaurantTracking
{
    public static class SessionExtensions
    {
        // Nesneyi (Sepeti) JSON'a çevirip kaydeder
        public static void SetObject(this ISession session, string key, object value)
        {
            session.SetString(key, JsonConvert.SerializeObject(value));
        }

        // Kaydedilen JSON'u tekrar Nesneye (Sepete) çevirir
        public static T GetObject<T>(this ISession session, string key)
        {
            var value = session.GetString(key);
            return value == null ? default(T) : JsonConvert.DeserializeObject<T>(value);
        }
    }
}