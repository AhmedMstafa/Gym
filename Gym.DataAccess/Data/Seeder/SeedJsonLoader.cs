using System.Reflection;
using System.Text.Json;

namespace Gym.DataAccess.Data.Seeder
{
    public class SeedJsonLoader
    {
        private static readonly JsonSerializerOptions jsonOptions = new JsonSerializerOptions()
        {
            PropertyNameCaseInsensitive = true
        };

        public static async Task<List<T>> LoadAsync<T>(string fileName)
        {
            // 1. Try AppContext.BaseDirectory / Data / Seeder / fileName
            var path = Path.Combine(AppContext.BaseDirectory, "Data", "Seeder", fileName);
            if (File.Exists(path))
            {
                await using var fileStream = File.OpenRead(path);
                return (await JsonSerializer.DeserializeAsync<List<T>>(fileStream, jsonOptions)) ?? new List<T>();
            }

            // 2. Try AppContext.BaseDirectory / fileName
            path = Path.Combine(AppContext.BaseDirectory, fileName);
            if (File.Exists(path))
            {
                await using var fileStream = File.OpenRead(path);
                return (await JsonSerializer.DeserializeAsync<List<T>>(fileStream, jsonOptions)) ?? new List<T>();
            }

            // 3. Fallback to EmbeddedResource if file is embedded in Gym.DataAccess assembly
            var assembly = typeof(SeedJsonLoader).Assembly;
            var resourceName = assembly.GetManifestResourceNames()
                .FirstOrDefault(r => r.EndsWith(fileName, StringComparison.OrdinalIgnoreCase));

            if (resourceName != null)
            {
                await using var stream = assembly.GetManifestResourceStream(resourceName);
                if (stream != null)
                {
                    return (await JsonSerializer.DeserializeAsync<List<T>>(stream, jsonOptions)) ?? new List<T>();
                }
            }

            throw new FileNotFoundException($"Seed file '{fileName}' not found on disk or as an embedded resource.");
        }
    }
}
