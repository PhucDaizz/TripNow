using MediatR;
using System.Text.Json.Serialization;

namespace SocialService.Application.Features.Member.EventHandlers.MemberRegister
{
    public class MemberRegisterEvent: INotification
    {
        [JsonPropertyName("userId")]
        public required string UserId { get; init; }
        [JsonPropertyName("email")]
        public required string Email { get; init; }
        [JsonPropertyName("fullName")]
        public string? FullName { get; init; }
    }
}
