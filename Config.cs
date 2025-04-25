using Newtonsoft.Json;

namespace slopbot;

public class Config
{
    public static Config Get()
    {
        var path = Path.Combine(Directory.GetCurrentDirectory(), "config.json");
        var text = File.ReadAllText(path);

        Config config = JsonConvert.DeserializeObject<Config>(text) ?? new Config();

        return config;
    }

    public string? Token { set; get; }
    public string? Url { set; get; }
    public string? Type { set; get; }
    public string? Cw { set; get; }
    public int? Timer { set; get; }
    public string[]? ExcludedVisibilities { set; get; }
    public string[]? AccountsToScrape { set; get; }
    public ConfigExclusionRules? ExcludedCws { set; get; }
    public ConfigExclusionRules? ExcludedText  { set; get; }
    public ConfigSources? Sources { set; get; }
}

public class ConfigExclusionRules
{
    public string[]? Contains { set; get; }
    public string[]? Equals { set; get; }
}

public class ConfigSources
{
    public string[]? MisskeyExport { set; get; }
}