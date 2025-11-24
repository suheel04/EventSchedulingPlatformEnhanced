using AutoMapper;
using EventService.Core.DbModels;
using EventService.Core.Dtos;
using EventService.Core.Interfaces;
using EventService.Core.Service;
using FluentValidation;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventService.Core.Tests.Service
{
   public class EventManagementServiceTests
    {
        private readonly Mock<IEventRepository> _mockEventRepository;
        private readonly Mock<IValidator<EventCreateDto>> _mockValidator;
        private readonly Mock<ILogger<EventManagementService>> _mockLogger;
        private readonly EventManagementService _eventManagementService;
        private readonly Mock<IAccountApi> _mockAccountApi;

        public EventManagementServiceTests()
        {
            _mockEventRepository = new Mock<IEventRepository>();
            _mockValidator = new Mock<IValidator<EventCreateDto>>();
            _mockLogger = new Mock<ILogger<EventManagementService>>();
            _mockAccountApi = new Mock<IAccountApi>();
            _eventManagementService = new EventManagementService(
                _mockEventRepository.Object,
                _mockValidator.Object,
                _mockLogger.Object,
                _mockAccountApi.Object
                );
        }
        [Theory]
        [InlineData("Admin")]
        [InlineData("User")]
        public async Task Search_ReturnsTotalCorrectly(string userRole)
        {
            // Arrange
            var userId = Guid.NewGuid();

            var events = new List<EventItem>
        {
            new EventItem { Id = Guid.NewGuid(), Title = "Test1", Location = "BLR", Start = DateTime.UtcNow, End = DateTime.UtcNow.AddHours(2), UserId = userId, CategoryId = Guid.NewGuid() },
            new EventItem { Id = Guid.NewGuid(), Title = "Test2", Location = "BLR", Start = DateTime.UtcNow.AddDays(1), End = DateTime.UtcNow.AddDays(1).AddHours(2), UserId = userId, CategoryId = Guid.NewGuid() }
        };

            _mockEventRepository.Setup(x => x.QueryAsync(
                                            It.IsAny<Guid?>(),  
                                            It.IsAny<DateTime?>(),
                                            It.IsAny<DateTime?>(),
                                            It.IsAny<string>(),
                                            It.IsAny<int>(),
                                            It.IsAny<int>())).ReturnsAsync((events, events.Count));

            // Act
            var (dtoList, total) = await _eventManagementService.Search(userId, userRole, null, null, null);

            var list = dtoList.ToList();

            // Assert
            Assert.Equal(2, total);
        }

        [Theory]
        [InlineData("11111111-1111-1111-1111-111111111111", "Admin")]
        public async Task Search_When_Admin_Role_CallsRepositoryWith_NullUserId(string id, string userRole)
        {
            // Arrange
            var userId = Guid.Parse(id);

            var events = new List<EventItem>();

            _mockEventRepository.Setup(x => x.QueryAsync(userId, null, null, null, 1, 10))
                .ReturnsAsync((events, events.Count));


            // Act
            var (dtoList, total) = await _eventManagementService.Search(userId, userRole, null, null, null);

            // Verify repository called exactly once
            _mockEventRepository.Verify(x => x.QueryAsync(null, null, null, null, 1, 10), Times.Once);
        }

        [Theory]
        [InlineData("22222222-2222-2222-2222-222222222222", "User")]
        public async Task Search_When_User_Role_CallsRepositoryWith_UserId(string id, string userRole)
        {
            // Arrange
            var userId = Guid.Parse(id);

            var events = new List<EventItem>();

            _mockEventRepository.Setup(x => x.QueryAsync(userId, null, null, null, 1, 10))
                .ReturnsAsync((events, events.Count));


            // Act
            var (dtoList, total) = await _eventManagementService.Search(userId, userRole, null, null, null);

            // Verify repository called exactly once
            _mockEventRepository.Verify(x => x.QueryAsync(userId, null, null, null, 1, 10), Times.Once);
        }
    }
}
