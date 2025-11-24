using AccountService.Core.DbModels;
using AccountService.Core.Dtos;
using AccountService.Core.ExceptionMappers;
using AccountService.Core.Interfaces;
using AccountService.Core.Services;
using AutoMapper;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.Extensions.Logging;
using Moq;

namespace AccountService.Core.Tests.Services
{
    public class RegisterServiceTests
    {
        private readonly Mock<IUserRepository> _mockUserRepository;
        private readonly Mock<IValidator<RegisterRequestDto>> _mockValidator;
        private readonly Mock<IMapper> _mockMapper;
        private readonly Mock<ILogger<RegisterService>> _mockLogger;
        private readonly RegisterService _registerService;

        public RegisterServiceTests()
        {
            _mockUserRepository = new Mock<IUserRepository>();
            _mockValidator = new Mock<IValidator<RegisterRequestDto>>();
            _mockMapper = new Mock<IMapper>();
            _mockLogger = new Mock<ILogger<RegisterService>>();

            _registerService = new RegisterService(
                _mockUserRepository.Object,
                _mockValidator.Object,
                _mockMapper.Object,
                _mockLogger.Object);
        }

        [Fact]
        public async Task Register_ThrowsBadRequest_WhenRequestIsNull()
        {
            // Act
            var exception = await Assert.ThrowsAsync<BadRequestException>(() =>
                _registerService.Register(null));

            // Assert
            Assert.Contains("Request body cannot be null", exception.Errors);
        }

        [Fact]
        public async Task Register_ThrowsBadRequest_WhenValidationFails()
        {
            // Arrange
            var dto = new RegisterRequestDto();
            var failures = new List<ValidationFailure>
        {
            new ValidationFailure("UserName", "Username is required")
        };

            _mockValidator
                .Setup(v => v.Validate(dto))
                .Returns(new ValidationResult(failures));

            // Act
            var exception = await Assert.ThrowsAsync<BadRequestException>(() =>
                _registerService.Register(dto));

            // Assert
            Assert.Contains("Username is required", exception.Errors);
        }

        [Fact]
        public async Task Register_ReturnsResponse_WhenValid()
        {
            // Arrange
            var dto = new RegisterRequestDto { UserName = "Test",Password="Test",Email = "Test@Test.com" };
            var user = new User { UserId = Guid.NewGuid(), PasswordHash = "Test", UserName = "Test",Email = "Test@Test.com" };
            var response = new UserResponseDto { UserId = user.UserId, UserName = "Test",UserEmail="Test@Test.com"};

            _mockValidator
                .Setup(v => v.Validate(dto))
                .Returns(new ValidationResult()); // Valid

            _mockMapper
                .Setup(m => m.Map<User>(dto))
                .Returns(user);

            _mockUserRepository
                .Setup(r => r.Register(user))
                .ReturnsAsync(response);

            // Act
            var result = await _registerService.Register(dto);

            // Assert
            Assert.Equal(response.UserId, result.UserId);
            Assert.Equal(response.UserName, result.UserName);
            Assert.Equal(response.UserEmail, result.UserEmail);
            _mockUserRepository.Verify(r => r.Register(user), Times.Once);
        }

        [Fact]
        public async Task Register_ThrowsException_WhenRepositoryThrows()
        {
            // Arrange
            var dto = new RegisterRequestDto { UserName = "Test", Password = "Test",Email="Test@Test.com" };
            var user = new User { UserId = Guid.NewGuid(), UserName = "Test",PasswordHash = "Test" };

            _mockValidator
                .Setup(v => v.Validate(dto))
                .Returns(new ValidationResult()); // Valid

            _mockMapper
                .Setup(m => m.Map<User>(dto))
                .Returns(user);

            _mockUserRepository
                .Setup(r => r.Register(user))
                .ThrowsAsync(new Exception("Database down"));

            // Act & Assert
            var ex = await Assert.ThrowsAsync<Exception>(() =>
                _registerService.Register(dto));

            Assert.Equal("Database down", ex.Message);
            _mockUserRepository.Verify(r => r.Register(user), Times.Once);
        }

    }

}
