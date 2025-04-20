namespace slopbot;

public class MastodonStatus
{
    public string? Id { get; set; }
    public string? Status { get; set; }
    public string? Text { get; set; } // from iceshrimp, better for markov because it will have any potential mfm
    public string? SpoilerText { get; set; }
    public string? Visibility { get; set; }
}