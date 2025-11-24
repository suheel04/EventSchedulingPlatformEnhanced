namespace AccountService.Core.Interfaces
{
    public interface IJwtTokenService
    {
        string GenerateJwtToken(Guid userId, string username, string role);
    }
}
