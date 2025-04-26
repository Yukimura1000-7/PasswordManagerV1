using SQLite;

namespace PasswordManagerV1.Models
{
    [Table("UserProfiles")]
    public class UserProfile
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        public string UserName { get; set; } = string.Empty;
        public string EncryptedPassword { get; set; } = string.Empty;
    }
}