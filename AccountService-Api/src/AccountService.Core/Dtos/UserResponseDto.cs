namespace AccountService.Core.Dtos
{
    public class UserResponseDto
    {
        public Guid UserId { get; set; }
        public  string? UserName { get; set; }
        public string? UserEmail { get; set; }
    }
}
