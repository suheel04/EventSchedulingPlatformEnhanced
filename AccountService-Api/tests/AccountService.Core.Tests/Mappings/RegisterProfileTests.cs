using AccountService.Core.DbModels;
using AccountService.Core.Dtos;
using AutoMapper;

namespace AccountService.Core.Tests.Mappings
{
    public class RegisterProfileTests
    {
        private readonly IMapper _mapper;

        public RegisterProfileTests()
        {
            var configuration = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<RegisterProfile>();
            });

            configuration.AssertConfigurationIsValid();
            _mapper = configuration.CreateMapper();
        }

        [Fact]
        public void RegisterRequestDto_MapsTo_User_Correctly()
        {
            // Arrange
            var dto = new RegisterRequestDto
            {
                UserName = "test",
                Password = "pass",
                Email = "test@test.com"
            };

            // Act
            var user = _mapper.Map<User>(dto);

            // Assert
            Assert.Equal(dto.UserName, user.UserName);
            Assert.Equal(dto.Password, user.PasswordHash); // currently copying directly
            Assert.Equal(dto.Email, user.Email); // currently copying directly
            Assert.Equal("User", user.Role);
            Assert.NotEqual(Guid.Empty, user.UserId); // new Guid assigned
        }
    }
}
