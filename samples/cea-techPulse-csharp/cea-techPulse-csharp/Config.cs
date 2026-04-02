namespace cea_techPulse_csharp
{
    public class ConfigOptions
    {
        public OpenAIConfigOptions OpenAI { get; set; }
        public NewsApiConfigOptions NewsApi { get; set; }
    }

    /// <summary>
    /// Options for OpenAI
    /// </summary>
    public class OpenAIConfigOptions
    {
        public string ApiKey { get; set; }
        public string DefaultModel = "gpt-4o";
    }

    /// <summary>
    /// Options for the News API
    /// </summary>
    public class NewsApiConfigOptions
    {
        public string ApiKey { get; set; }
    }
}