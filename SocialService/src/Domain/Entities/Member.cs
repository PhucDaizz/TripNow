using SocialService.Domain.Common;
using SocialService.Domain.Enum;

namespace SocialService.Domain.Entities
{
    public class Member : BaseEntity
    {
        public string FullName { get; private set; }
        public string AvatarUrl { get; private set; }

        private Member() { }    

        public Member(Guid id, string fullName, string avatarUrl)
        {
            Id = id; 
            FullName = fullName;
            AvatarUrl = avatarUrl;
            CreatedAt = DateTime.UtcNow;
        }

        public void UpdateInfo(string fullName, string avatarUrl)
        {
            FullName = fullName;
            AvatarUrl = avatarUrl;
            UpdatedAt = DateTime.UtcNow;
        }
    }
}
