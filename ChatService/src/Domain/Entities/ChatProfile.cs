using ChatService.Domain.Common;
using ChatService.Domain.Enum;

namespace ChatService.Domain.Entities
{
    public class ChatProfile : BaseEntity
    {
        public string FullName { get; private set; }
        public string AvatarUrl { get; private set; }
        public AuthorType Type { get; private set; }


        private ChatProfile() { }

        public ChatProfile(Guid id, string fullName, string avatarUrl, AuthorType type = AuthorType.User)
        {
            Id = id;
            FullName = fullName;
            AvatarUrl = avatarUrl;
            CreatedAt = DateTime.UtcNow;
            Type = type;
        }

        public void UpdateInfo(string fullName, string avatarUrl)
        {
            FullName = fullName;
            AvatarUrl = avatarUrl;
            UpdatedAt = DateTime.UtcNow;
        }

        public void UpdateAvatar(string avatarUrl)
        {
            AvatarUrl = avatarUrl;
            UpdatedAt = DateTime.UtcNow;
        }
    }
}
