using SQLite;

namespace PasswordManagerV1.Models
{
    [Table("UserAccounts")]
    public class UserAccount
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        public int UserId { get; set; } // Связь с пользователем
        public string ServiceName { get; set; } = string.Empty;
        public string Login { get; set; } = string.Empty;
        public string EncryptedPassword { get; set; } = string.Empty;
    }
}