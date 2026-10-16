
namespace RaceDay.API.Models
{
    public class User
    {
        public int UserID { get; set; }

        public int RoleID { get; set; }

        public string FullName { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string PasswordHash { get; set; } = string.Empty;

        public string PhoneNumber { get; set; } = string.Empty;

        public string? ProfilePictureUrl { get; set; }
    }
}