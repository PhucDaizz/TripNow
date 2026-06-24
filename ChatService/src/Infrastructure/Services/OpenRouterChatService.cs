using ChatService.Application.Common.Interfaces;
using Microsoft.Extensions.Configuration;
using System.Net.Http.Json;
using System.Text.Json;

namespace ChatService.Infrastructure.Services
{
    public class OpenRouterChatService : IAiChatService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        public OpenRouterChatService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;
        }

        public async Task<string> GetChatCompletionAsync(string systemPrompt, string userMessage, CancellationToken cancellationToken = default)
        {
            var apiKey = _configuration["OpenRouter:ApiKey"];

            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");
            _httpClient.DefaultRequestHeaders.Add("HTTP-Referer", "http://localhost:7000");

            var requestBody = new
            {
                model = "z-ai/glm-4.5-air:free",
                messages = new[]
                {
                    new { role = "system", content = systemPrompt },
                    new { role = "user", content = userMessage }
                },
                temperature = 0.3
            };

            var response = await _httpClient.PostAsJsonAsync("https://openrouter.ai/api/v1/chat/completions", requestBody, cancellationToken);

            if (response.IsSuccessStatusCode)
            {
                var jsonDoc = await response.Content.ReadFromJsonAsync<JsonElement>(cancellationToken: cancellationToken);
                try
                {
                    var aiResponse = jsonDoc
                        .GetProperty("choices")[0]
                        .GetProperty("message")
                        .GetProperty("content")
                        .GetString();

                    return aiResponse ?? "Dạ, hệ thống AI không phản hồi, mong quý khách thông cảm.";
                }
                catch
                {
                    return "Dạ, có lỗi trong quá trình xử lý câu trả lời từ Lễ tân ảo.";
                }
            }

            var errorContent = await response.Content.ReadAsStringAsync(cancellationToken);
            return $"Dạ, Lễ tân ảo đang bận (Lỗi từ máy chủ AI). Chi tiết: {errorContent}";
        }
    }
}
