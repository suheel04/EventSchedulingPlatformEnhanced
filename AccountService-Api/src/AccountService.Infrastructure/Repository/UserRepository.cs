using AccountService.Core.DbModels;
using AccountService.Core.Dtos;
using AccountService.Core.Interfaces;
using AccountService.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace AccountService.Infrastructure.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly AccountDbContext _dbContext;
        private readonly ILogger<UserRepository> _logger;
        public UserRepository(AccountDbContext dbContext, ILogger<UserRepository> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }
        public async Task<User?> GetByUserNameAsync(string userName)
        {
            var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.UserName == userName);
            return user;           
        }

        public async Task<User?> GetByUserIdAsync(Guid userId)
        {
            var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.UserId == userId);
            return user;
        }

        public async Task<UserResponseDto> Register(User user)
        {
            await _dbContext.Users.AddAsync(user);
            await _dbContext.SaveChangesAsync();
            //****Below 2 loginfo is to show difference between structured Logging and Unstructured Logging****
            //Unstructured Logging logs Stored in Plain text.Free - form plain text;No key/value fields
            //_logger.LogInformation($"Plain text:User Saved in Db. Userid:{user.UserId}, Username:{user.UserName}");
            //Structured logging means logs are stored as key-value pairs instead of plain text.Uses named placeholders ({UserId}) , Great for searching, filtering.
            _logger.LogInformation("Structured logging:User Saved in Db. Userid:{userid}, Username:{username}", user.UserId, user.UserName);
            return new UserResponseDto { UserId = user.UserId, UserName = user.UserName,UserEmail=user.Email };    
        }

        public async Task UpdateUserAsync(User user)
        {
            _dbContext.Users.Update(user);
            await _dbContext.SaveChangesAsync();
        }
    }
}
