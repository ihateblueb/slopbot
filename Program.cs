using Markov;
using Newtonsoft.Json;
using slopbot;

class Program
{
    static void Main(string[] args)
    {
        if (args.Length > 0 && args[0].Equals("scrape"))
        {
            MastodonScraper.Scrape().Wait();
        }
        if (args.Length > 0 && args[0].Equals("backgroundProcess"))
        {
            BackgroundProcess.Run();
        }
        else
        {
            GenerateModel();
        }
    }

    public static void GenerateModel()
    {
        var config = Config.Get();

        List<List<MisskeyExportEntry>> mkExports = new List<List<MisskeyExportEntry>>();

        if (config.Sources?.MisskeyExport == null)
            throw new Exception("Missing exports");

        foreach (var mkExport in config.Sources?.MisskeyExport ?? [])
        {
            Console.WriteLine(mkExport);

            var path = Path.Combine(Directory.GetCurrentDirectory(), mkExport);
            var text = File.ReadAllText(path);

            MisskeyExportEntry[]? deserializedExport = JsonConvert.DeserializeObject<MisskeyExportEntry[]>(text);

            if (deserializedExport != null)
                mkExports.Add(deserializedExport.ToList());

        }

        var chain = new MarkovChain<string>(1);

        var numEntries = 0;

        foreach (var list in mkExports)
        {
            foreach (var entry in list)
            {
                var skip = false;

                if (string.IsNullOrEmpty(entry.Text) || string.IsNullOrEmpty(entry.Visibility))
                    skip = true;

                if (!skip && config.ExcludedVisibilities != null && config.ExcludedVisibilities.Length > 0)
                {
                    foreach (var excludedVisibility in config.ExcludedVisibilities)
                    {
                        if (entry.Visibility.Equals(excludedVisibility))
                            skip = true;
                    }
                }

                if (!skip && !string.IsNullOrEmpty(entry.Cw) && config.ExcludedCws != null)
                {
                    if (config.ExcludedCws.Contains != null && config.ExcludedCws.Contains.Length > 0)
                    {
                        foreach (var containKeyword in config.ExcludedCws.Contains)
                        {
                            if (entry.Cw.Contains(containKeyword)) skip = true;
                        }
                    }

                    if (config.ExcludedCws.Equals != null && config.ExcludedCws.Equals.Length > 0)
                    {
                        foreach (var equalsKeyword in config.ExcludedCws.Equals)
                        {
                            if (entry.Cw.Equals(equalsKeyword)) skip = true;
                        }
                    }
                }

                if (!skip && config.ExcludedText != null)
                {
                    if (config.ExcludedText.Contains != null && config.ExcludedText.Contains.Length > 0)
                    {
                        foreach (var containKeyword in config.ExcludedText.Contains)
                        {
                            if (entry.Text.Contains(containKeyword)) skip = true;
                        }
                    }

                    if (config.ExcludedText.Equals != null && config.ExcludedText.Equals.Length > 0)
                    {
                        foreach (var equalsKeyword in config.ExcludedText.Equals)
                        {
                            if (entry.Text.Equals(equalsKeyword)) skip = true;
                        }
                    }
                }

                if (skip)
                    continue;

                var editedString = entry.Text
                    .Replace("@", "@​")
                    .Replace("#", "#​");

                var splitEntry = editedString.Split(' ');

                chain.Add(splitEntry);
                numEntries++;
            }
        }

        Console.WriteLine("\nGenerated model from "+numEntries+" entries\n");

        static string GenerateSentence(MarkovChain<string> chain)
        {
            var rand = new Random();
            return string.Join(" ", chain.Chain(rand));
        }

        var generatedSentence = GenerateSentence(chain);
        Console.WriteLine("SENTENCE: "+generatedSentence);

        if (config.Type == null)
            throw new Exception("Missing type");

        if (config.Type is not ("mastodon" or "misskey"))
            throw new Exception("Invalid type");

        switch (config.Type)
        {
            case "mastodon":
                Post.PostToMastodon(generatedSentence).Wait();
                break;
            case "misskey":
                Post.PostToMisskey(generatedSentence).Wait();
                break;
        }
    }
}
