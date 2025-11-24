using AccountService.Core.DbModels;
using AccountService.Data;
using AccountService.Infrastructure.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;

namespace AccountService.Infrastructure.Tests
{
    public class UserRepositoryTests
    {
        [Fact]
        public async Task Register_ShouldAddUserAndReturnDto()
        {
            // Arrange
            var mockSet = new Mock<DbSet<User>>();
            var options = new DbContextOptionsBuilder<AccountDbContext>().Options;
            var mockContext = new Mock<AccountDbContext>(options);

            mockContext.Setup(m => m.Users).Returns(mockSet.Object);
            mockContext.Setup(m => m.SaveChangesAsync(It.IsAny<CancellationToken>()))
                       .ReturnsAsync(1); // Simulate successful save

            var mockLogger = new Mock<ILogger<UserRepository>>();

            var userRepository = new UserRepository(mockContext.Object, mockLogger.Object);

            var user = new User
            {
                UserId = Guid.NewGuid(),
                UserName = "TestUser",
                Email = "Test@Test.com"
            };

            // Act
            var result = await userRepository.Register(user);

            // Assert
            mockSet.Verify(m => m.AddAsync(user, It.IsAny<CancellationToken>()), Times.Once);
            mockContext.Verify(m => m.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
            mockLogger.Verify(
                x => x.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("User Saved in Db")),
                    null,
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Exactly(1) // structured logs
            );

            Assert.Equal(user.UserId, result.UserId);
            Assert.Equal(user.UserName, result.UserName);
            Assert.Equal(user.Email, result.UserEmail);
        }
    }
}

