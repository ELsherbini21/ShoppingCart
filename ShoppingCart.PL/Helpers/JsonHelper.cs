using NuGet.Protocol;
using System.Text.Json;

namespace ShoppingCart.PL.Helpers
{
    public static class JsonHelper
    {
        public static async Task<T> ReadAsync<T>(string filePath)
        {
            using var stream = File.OpenRead(filePath);
            return await JsonSerializer.DeserializeAsync<T>(stream);
        }

        public static T Read<T>(string filePath)
        {
            string text = File.ReadAllText(filePath);
            return JsonSerializer.Deserialize<T>(text);
        }
    }
}
