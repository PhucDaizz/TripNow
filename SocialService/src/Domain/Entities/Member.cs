using SocialService.Domain.Common;
using SocialService.Domain.Enum;

namespace SocialService.Domain.Entities
{
    public class Member : BaseEntity
    {
        public string FullName { get; private set; }
        public string AvatarUrl { get; private set; }
        public AuthorType Type { get; private set; }


        private Member() { }    

        public Member(Guid id, string fullName, string avatarUrl, AuthorType type = AuthorType.User)
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
