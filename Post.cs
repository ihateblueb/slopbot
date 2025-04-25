using System.Net.Http.Json;
using System.Text;
using System.Text.Json;

namespace slopbot;

public class Post
{
    private static HttpClient HttpClient = new();

    public static async Task PostToMastodon(string sentence)
    {
        var config = Config.Get();

        if (config.Token == null)
            throw new Exception("Missing token");
        if (config.Url == null)
            throw new Exception("Missing URL");

        HttpClient.BaseAddress = new Uri(config.Url);
        HttpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {config.Token}");

        using StringContent postBody = new(
            JsonSerializer.Serialize(new
            {
                status = sentence,
                visibility = "unlisted",
                spoiler_text = config.Cw
            }),
            Encoding.UTF8,
            "application/json");

        using HttpResponseMessage response = await HttpClient.PostAsync(
            "/api/v1/statuses",
            postBody);

        Console.WriteLine($"\n{response.StatusCode} :: {await response.Content.ReadAsStringAsync()}");

        HttpClient = new HttpClient();
    }

    public static async Task PostToMisskey(string sentence)
    {
        var config = Config.Get();

        if (config.Token == null)
            throw new Exception("Missing token");
        if (config.Url == null)
            throw new Exception("Missing URL");

        HttpClient.BaseAddress = new Uri(config.Url);
        HttpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {config.Token}");

        using StringContent postBody = new(
            JsonSerializer.Serialize(new
            {
                text = sentence,
                visibility = "home",
                cw = config.Cw
            }),
            Encoding.UTF8,
            "application/json");

        using HttpResponseMessage response = await HttpClient.PostAsync(
            "/api/notes/create",
            postBody);

        Console.WriteLine($"\n{response.StatusCode} :: {await response.Content.ReadAsStringAsync()}");

        HttpClient = new HttpClient();
    }
}