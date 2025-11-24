using AccountService.Api.Controllers;
using AccountService.Core.Dtos;
using AccountService.Core.ExceptionMappers;
using AccountService.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;

namespace AccountService.Api.Tests.Controllers
{
    public class RegisterControllerTests
    {
        private readonly Mock<IRegisterService> _mockRegisterService;
        private readonly Mock<ILoginService> _mockLoginService;
        private readonly Mock<IUserService> _mockSetRoleService;
        private readonly Mock<ILogger<AccountController>> _mockLogger;
        private readonly AccountController _controller;
        public RegisterControllerTests()
        {
            _mockRegisterService = new Mock<IRegisterService>();
            _mockLoginService = new Mock<ILoginService>();
            _mockSetRoleService = new Mock<IUserService>();
            _mockLogger = new Mock<ILogger<AccountController>>();
            _controller=new AccountController(_mockRegisterService.Object, _mockLoginService.Object, _mockSetRoleService.Object, _mockLogger.Object);
        }

        [Fact]
        public async Task Register_ReturnOk_WhenValidRequest()
        {
            //Arrange
            var request= new RegisterRequestDto { UserName="test",Password="password",Email="test@test.com"};
            var response = new UserResponseDto { UserId = Guid.NewGuid(), UserName = "test",UserEmail= "test@test.com" };

            _mockRegisterService.Setup(s=>s.Register(request)).ReturnsAsync(response);

            //Act
            var result=await _controller.Register(request);

            //Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedValue = Assert.IsType<UserResponseDto>(okResult.Value);
            Assert.Equal("test",returnedValue.UserName);
            Assert.Equal("test@test.com", returnedValue.UserEmail);
        }

        [Theory]
        [InlineData("", "", "Username is Null or Empty")]
        [InlineData(null, null, "Username is Null or Empty")]
        [InlineData(" ", " ", "Username is Null or Empty")]
        [InlineData("", "", "Password is Null or Empty")]
        [InlineData(null, null, "Password is Null or Empty")]
        [InlineData(" ", " ", "Password is Null or Empty")]
        [InlineData("", "password", "Username is Null or Empty")]
        [InlineData(" ", "password", "Username is Null or Empty")]
        [InlineData(null, "password", "Username is Null or Empty")]
        [InlineData("test", "", "Password is Null or Empty")]
        [InlineData("test", " ", "Password is Null or Empty")]
        [InlineData("test", null, "Password is Null or Empty")]
        public async Task Register_ReturnsBadRequest_WhenBadRequestExceptionThrown(string userName, string password, string errorMessage)
        {
            // Arrange
            var request = new RegisterRequestDto { UserName = userName, Password = password, Email = "test@test.com" };
            var errors = new[] { errorMessage };

            _mockRegisterService.Setup(s => s.Register(request))
                        .ThrowsAsync(new BadRequestException(errors));

            // Act
            var result = await _controller.Register(request);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var errorDto = Assert.IsType<ErrorResponseDto>(badRequestResult.Value);
            Assert.Contains(errorMessage, errorDto.Errors);
        }

        [Fact]
        public async Task Register_Returns500_WhenGeneralExceptionThrown()
        {
            // Arrange
            var request = new RegisterRequestDto { UserName = "test", Password = "pass", Email = "test@test.com" };

            _mockRegisterService.Setup(s => s.Register(request))
                        .ThrowsAsync(new Exception("DB offline"));

            // Act
            var result = await _controller.Register(request);

            // Assert
            var statusResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, statusResult.StatusCode);
            Assert.Equal("Something went wrong!", statusResult.Value);
        }

        [Fact]
        public async Task Register_LogsWarning_OnValidationFailure()
        {
            // Arrange
            var errors = new[] { "Username is Null or Empty" };

            _mockRegisterService
                .Setup(s => s.Register(It.IsAny<RegisterRequestDto>()))
                .ThrowsAsync(new BadRequestException(errors));

            // Act
            await _controller.Register(new RegisterRequestDto());

            // Assert → Logger Warning Called
            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Warning,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, _) => v.ToString().Contains("Validation failed")),
                    null,
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);
        }

        [Fact]
        public async Task Register_LogsError_OnGeneralExceptionFailure()
        {
            // Arrange
            _mockRegisterService
        .Setup(s => s.Register(It.IsAny<RegisterRequestDto>()))
        .ThrowsAsync(new Exception("DB offline"));

            // Act
            await _controller.Register(new RegisterRequestDto());

            // Assert → Logger Error Called
                _mockLogger.Verify(
            x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) =>
                    v.ToString().Contains("Exception Mesage at Register") &&
                    v.ToString().Contains("StackTrace")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception, string>>()),
            Times.Once);
        }

    }
}
