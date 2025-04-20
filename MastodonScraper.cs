using System.Net.Http.Json;
using Newtonsoft.Json;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace slopbot;

public class MastodonScraper
{
    private static HttpClient HttpClient = new();
    private static List<MastodonStatus> statusesForConversion = new List<MastodonStatus>();

    public static async Task Scrape()
    {
        var config = Config.Get();

        if (config.Type != "mastodon")
            throw new Exception("Scrape feature only available for Mastodon");

        if (config.AccountsToScrape == null ||  config.AccountsToScrape.Length == 0)
            throw new Exception("No accounts configured to scrape");

        HttpClient.BaseAddress = new Uri(config.Url);
        HttpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {config.Token}");

        foreach (var account in config.AccountsToScrape)
        {
            using HttpResponseMessage accountResponse = await HttpClient.GetAsync(
                "/api/v1/accounts/lookup?acct="+account);

            Console.WriteLine($"\n{accountResponse.StatusCode} :: {await accountResponse.Content.ReadAsStringAsync()}");

            var mastoAccount = await accountResponse.Content.ReadFromJsonAsync<MastodonAccount>();
            Console.WriteLine($"\nGot Mastodon account @{mastoAccount.Username} ({mastoAccount.Id})\n");

            GetBatchOfPosts(mastoAccount.Id).Wait();

            List<MisskeyExportEntry> misskeyExport = new List<MisskeyExportEntry>();

            foreach (var status in statusesForConversion)
            {
                var mkEntry = new MisskeyExportEntry();

                mkEntry.Cw = status.SpoilerText;

                if (status.Visibility == "public")
                    mkEntry.Visibility = "public";

                if (status.Visibility == "unlisted")
                    mkEntry.Visibility = "home";

                if (status.Visibility == "private")
                    mkEntry.Visibility = "followers";

                if (status.Visibility == "direct")
                    mkEntry.Visibility = "specified";

                mkEntry.Text = status.Status ?? status.Text ?? null;

                misskeyExport.Add(mkEntry);
            }

            // clear it after done
            statusesForConversion = new List<MastodonStatus>();

            string misskeyExportJson = JsonSerializer.Serialize(misskeyExport);

            var path = Path.Combine(Directory.GetCurrentDirectory(), $"{account}_{DateTimeOffset.Now.ToUnixTimeSeconds()}_scrape.json");
            using (StreamWriter outputFile = new StreamWriter(path))
            {
                outputFile.WriteLine(misskeyExportJson);
            }
        }
    }

    private static async Task GetBatchOfPosts(string id, string? maxId = null)
    {
        Console.WriteLine($"Getting batch of posts for {id}, max_id: {maxId}");

        using HttpResponseMessage response = await HttpClient.GetAsync(
            $"/api/v1/accounts/{id}/statuses/{(maxId != null ? ("?max_id=" + maxId) : null)}");

        var batch = await response.Content.ReadFromJsonAsync<MastodonStatus[]>();

        if (batch == null || batch.Length == 0)
        {
            Console.WriteLine("No content returned, assumed end of posts");
        }
        else
        {
            foreach (var status in batch)
            {
                statusesForConversion.Add(status);
            }

            GetBatchOfPosts(id, batch.Last().Id).Wait();
        }
    }
}