using EventService.Api.Controllers;
using EventService.Core.Dtos;
using EventService.Core.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System.Security.Claims;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace EventService.Api.Tests.Controllers
{
    public class EventControllerTests
    {
        private readonly Mock<IEventManagementService> _mockEventManagementService;
        private readonly Mock<ILogger<EventsController>> _mockLogger;
        private readonly EventsController _controller;
        public EventControllerTests()
        {
            _mockEventManagementService = new Mock<IEventManagementService>();
            _mockLogger = new Mock<ILogger<EventsController>>();
            _controller = new EventsController(_mockEventManagementService.Object, _mockLogger.Object);
        }
        [Fact]
        public async Task Search_ReturnsOk_WithExpectedData()
        {
            // Arrange
            //var mockService = new Mock<IEventManagementService>();
            var testUserId = Guid.NewGuid();

            var sampleEvents = new List<EventDto>
        {
            new EventDto { Id = Guid.NewGuid(), Title = "Event1", Location = "Office", Start = DateTime.UtcNow, End = DateTime.UtcNow.AddHours(1) },
            new EventDto { Id = Guid.NewGuid(), Title = "Event2", Location = "Home", Start = DateTime.UtcNow, End = DateTime.UtcNow.AddHours(2) }
        };

            // Setup mock to return sample events
            _mockEventManagementService.Setup(s => s.Search(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<DateTime?>(), It.IsAny<DateTime?>(),
                                            It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()))
                       .ReturnsAsync((sampleEvents, sampleEvents.Count));

            //setup claims
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.Role, "Admin")
            };

            var identity = new ClaimsIdentity(claims, "TestAuth");
            var claimsPrincipal = new ClaimsPrincipal(identity);

            // Attach to controller context
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = claimsPrincipal }
            };
            // Act
            var result = await _controller.Search(null, null, "Office", 1, 10);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            dynamic data = okResult.Value;

            Assert.Equal(sampleEvents.Count,  (int)data.GetType().GetProperty("total").GetValue(data));
            Assert.Equal(1, (int)data.GetType().GetProperty("pageNumber").GetValue(data));
            Assert.Equal(10, (int) data.GetType().GetProperty("pageSize").GetValue(data));

            var items = data.GetType().GetProperty("items").GetValue(data) as IEnumerable<EventDto>;
            Assert.NotNull(items);
            Assert.Contains(items, e => e.Title == "Event1");
        }

        [Fact]
        public async Task Search_WhenUserNotAuthenticated_ReturnsUnauthorized()
        {
            // Arrange
            // Mock unauthenticated user
            var identity = new ClaimsIdentity(); // NOT authenticated
            var user = new ClaimsPrincipal(identity);
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = user }
            };

            // Act
            var result = await _controller.Search(null, null, null, 1, 10);

            // Assert
            var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(result);
            dynamic value = unauthorizedResult.Value!;
            Assert.Equal("User is not authenticated.", (string)value!.GetType().GetProperty("message").GetValue(value));
        }
    }

}
