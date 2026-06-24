namespace ChatService.Application.Common.Interfaces
{
    public interface IAiChatService
    {
        Task<string> GetChatCompletionAsync(string systemPrompt, string userMessage, CancellationToken cancellationToken = default);
    }
}
