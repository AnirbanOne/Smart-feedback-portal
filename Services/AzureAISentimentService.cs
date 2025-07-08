using Azure;
using Azure.AI.TextAnalytics;

namespace SmartFeedbackPortal.API.Services
{
    public class AzureAISentimentService
    {
        private readonly TextAnalyticsClient _client;

        public AzureAISentimentService(IConfiguration config)
        {
            var endpoint = new Uri(config["AzureAI:Endpoint"]);
            var key = new AzureKeyCredential(config["AzureAI:Key"]);

            _client = new TextAnalyticsClient(endpoint, key);
        }

        public async Task<string> AnalyzeSentimentAsync(string text)
        {
            var response = await _client.AnalyzeSentimentAsync(text);
            return response.Value.Sentiment.ToString();  // e.g., Positive/Negative/Neutral
        }
    }
}
