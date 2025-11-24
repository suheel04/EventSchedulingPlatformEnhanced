namespace AccountService.Core.Dtos
{
    public class RegisterRequestDto
    {

        public string? UserName { get; set; }
        public string? Password { get; set; }
        public string? Email { get; set; }
    }
}
