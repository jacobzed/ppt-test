using Microsoft.Data.Sqlite;
using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;

namespace PacificTest.Services
{
    public class AvatarService : IAvatarService
    {
        const string defaultUrl = "https://api.dicebear.com/8.x/pixel-art/png?seed=default&size=150";

        string dbPath;
        ILogger log;

        HttpClient httpClient;


        public AvatarService(ILogger<AvatarService> log, HttpClient httpClient)
        {
            var rootPath = AppContext.BaseDirectory;
            dbPath = Path.Combine(rootPath, "data.db");
            this.httpClient = httpClient;
            this.log = log;
        }

        public string GetDefaultAvatar()
        {
            return defaultUrl;
        }

        public async Task<string> GetAvatar(string? userId)
        {
            if (String.IsNullOrWhiteSpace(userId))
                return defaultUrl;

            userId = userId.Trim();
            string? url = null;

            if (Regex.IsMatch(userId, "[6-9]$"))
            {
                // If the last character of the user identifier is [6, 7, 8, 9]
                try
                {
                    url = await GetRemoteAvatar(userId);
                }
                catch (Exception)
                {
                    log.LogError("Failed to fetch remote avatar for user {userId}");
                }
            }
            else if (Regex.IsMatch(userId, "[1-5]$"))
            {
                // If the user last character of the user identifier is [1, 2, 3, 4, 5]
                url = GetLocalAvatar(userId);
            }
            else if (Regex.IsMatch(userId, "[aeiou]", RegexOptions.IgnoreCase))
            {
                // If the user identifier contains at least one vowel character...
                url = "https://api.dicebear.com/8.x/pixel-art/png?seed=vowel&size=150";
            }
            else if (Regex.IsMatch(userId, "[^a-z0-9]"))
            {
                // If the user identifier contains a non-alphanumeric character...
                var rand = new Random();
                var randomNumber = rand.Next(5) + 1;
                url = $"https://api.dicebear.com/8.x/pixel-art/png?seed={randomNumber}&size=150";
            }

            return url ?? defaultUrl;
        }

        private async Task<string?> GetRemoteAvatar(string userId)
        {
            var endpoint = $"https://my-json-server.typicode.com/ck-pacificdev/tech-test/images/{userId.Substring(userId.Length - 1)}";
            var response = await httpClient.GetAsync(endpoint);
            response.EnsureSuccessStatusCode();
            var body = await response.Content.ReadAsStringAsync();
            var json = JsonSerializer.Deserialize<PacificAvatarResponse>(body ?? "{}");
            return json?.Url;
        }

        private string? GetLocalAvatar(string userId)
        {
            // Get the last digit
            var digit = Int32.Parse(userId.Substring(userId.Length - 1));

            using (var connection = GetConnection())
            {
                connection.Open();

                var command = connection.CreateCommand();
                command.CommandText = @"SELECT url FROM images WHERE id = $id";
                command.Parameters.AddWithValue("$id", digit);

                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                        return reader.GetString(0);
                }
            }

            return null;
        }

        private SqliteConnection GetConnection()
        {
            return new SqliteConnection("Data Source=" + dbPath);
        }


        class PacificAvatarResponse
        {
            [JsonPropertyName("id")]
            public int Id { get; set; }
            [JsonPropertyName("url")]
            public string Url { get; set; }
        }

    }
}
