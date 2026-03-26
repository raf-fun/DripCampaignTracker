using DripsCampaignTracker.Enums;
using Microsoft.SemanticKernel;

namespace DripsCampaignTracker.Services
{
    public class AIService : IAIService
    {
        private readonly Kernel kernel;
        private readonly ILogger<AIService> logger;
        public AIService(IConfiguration configuration, ILogger<AIService> logger)
        {
            var apiKey = configuration["AI:ApiKey"] ?? string.Empty;
            var model = configuration["AI:Model"] ?? string.Empty;

            kernel = Kernel.CreateBuilder()
                .AddGoogleAIGeminiChatCompletion(model, apiKey)
                .Build();
            this.logger = logger;
        }

        public async Task<Classification> ClassifyReplyAsync(string message)
        {
            try
            {
                var prompt = $"""
                You are classifying an SMS reply from a lead in a marketing campaign.
                Classify the following message as exactly one of: Yes, No, Ambiguous.
                - Yes: the lead is confirming they will take the desired action
                - No: the lead is declining or opting out
                - Ambiguous: the reply is unclear or non-committal
                Reply with a single word only: Yes, No, or Ambiguous.
                Message: "{message}"
                """;

                var result = await kernel.InvokePromptAsync(prompt);
                var text = result.ToString().Trim().ToLower();

                return text switch
                {
                    "yes" => Classification.Yes,
                    "no" => Classification.No,
                    _ => Classification.Ambiguous
                };
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "AI classification failed. Defaulting to Ambiguous.");
                return Classification.Ambiguous;
            }
        }

        public async Task<string> GenerateResponseAsync(Classification classification, string leadName)
        {
            try
            {
                var prompt = classification switch
                {
                    Classification.Yes => $"""
                    Write a short, enthusiastic SMS confirmation message to {leadName} 
                    who just confirmed they will attend. Keep it under 2 sentences. 
                    Be warm and excited. No hashtags or emojis.
                    """,
                    Classification.No => $"""
                    Write a short, polite SMS thank you message to {leadName} 
                    who just declined. Keep it under 2 sentences. 
                    Be gracious and respectful. No hashtags or emojis.
                    """,
                    _ => $"""
                    Write a short SMS message to {leadName} letting them know 
                    we will follow back up with them soon. Keep it under 2 sentences. 
                    Be friendly and low pressure. No hashtags or emojis.
                    """
                };

                var result = await kernel.InvokePromptAsync(prompt);
                return result.ToString().Trim();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"AI response generation failed for {leadName}. Using fallback message.");
                return GetFallbackResponse(classification, leadName);
            }
        }

        private static string GetFallbackResponse(Classification classification, string leadName)
        {
            return classification switch
            {
                Classification.Yes => $"Thank you {leadName}! We are excited to have you and will be in touch shortly.",
                Classification.No => $"Thank you for letting us know {leadName}. We appreciate your time.",
                _ => $"Hi {leadName}, we will follow back up with you soon. Have a great day!"
            };
        }
    }
}
