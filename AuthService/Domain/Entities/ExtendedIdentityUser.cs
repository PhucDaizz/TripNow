using Microsoft.AspNetCore.Identity;

namespace Domain.Entities
{
    public class ExtendedIdentityUser : IdentityUser
    {
        public string FullName { get; set; }
        public string AvatarUrl { get; set; }
        public int Follower { get; set; }
        public bool IsActive { get; set; } = true;  
        public string? RefreshToken { get; set; }
        public DateTime? RefreshTokenExpiry { get; set; }
        public bool? Gender { get; set; }
        public string? Address { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation property
        public virtual StaffProfile StaffProfile { get; set; }

        public void IncreaseFollow()
        {
            this.Follower += 1;
        }

        public void UnFollow()
        {
            if (this.Follower != 0 && this.Follower > 0)
                this.Follower -= 1;
        }
    }
}
