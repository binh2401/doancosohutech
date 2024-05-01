using System.Text.Json;

namespace WebBanHang.Extensions
{
    public static class SessionExtensions
    {
        // ham lay du lieu vao trong session
        public static void SetObjectAsJson(this ISession session, string key,
        object value)
        {
            session.SetString(key, JsonSerializer.Serialize(value));
        }

        // ham
        public static T GetObjectFromJson<T>(this ISession session, string
        key)
        {
            var value = session.GetString(key);
            return value == null ? default : JsonSerializer.Deserialize<T>(value);
        }
    }
}