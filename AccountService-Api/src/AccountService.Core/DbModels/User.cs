namespace AccountService.Core.DbModels
{
    public class User
    {
        public Guid UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        // NOTE: For sample only: store plain text or simple value. In real apps hash + salt.
        public string PasswordHash { get; set; } = string.Empty;
        public string Role { get; set; } = "User";

        public string Email { get; set; } = string.Empty;
    }
}
