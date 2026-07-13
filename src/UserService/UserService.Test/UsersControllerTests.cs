using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using UserService.Application.Inputs;
using UserService.Application.Services;
using UserService.Domain.Models;
using UserService.WebApi.Controllers;

namespace UserService.Test
{
    public class UsersControllerTests
    {
        private readonly Mock<IUserApplicationService> _service = new();

        private UsersController Build(Guid? userId) =>
            new(_service.Object, Mock.Of<ILogger<UsersController>>())
            {
                ControllerContext = ControllerTestContext.WithUser(userId)
            };

        [Fact]
        public async Task GetMe_WithAuthenticatedUser_ReturnsOwnProfile()
        {
            var id = Guid.NewGuid();
            var dto = new UserResponseDto(id, "User", "user@test.com", "52998224725",
                new List<string> { Roles.Doador }, DateTimeOffset.Now);
            _service.Setup(s => s.GetById(id)).ReturnsAsync(BaseService.Ok<UserResponseDto?>(dto));

            var result = await Build(id).GetMe();

            var ok = Assert.IsType<ObjectResult>(result);
            Assert.Equal((int)HttpStatusCode.OK, ok.StatusCode);
            Assert.Equal(dto, ok.Value);
            _service.Verify(s => s.GetById(id), Times.Once);
        }

        [Fact]
        public async Task GetMe_WithoutClaim_ReturnsUnauthorized_AndSkipsService()
        {
            var result = await Build(null).GetMe();

            Assert.IsType<UnauthorizedResult>(result);
            _service.Verify(s => s.GetById(It.IsAny<Guid>()), Times.Never);
        }

        [Fact]
        public async Task UpdateMe_WithAuthenticatedUser_UpdatesOwnAccount()
        {
            var id = Guid.NewGuid();
            var dto = new UserUpdateDto { Name = "Novo Nome" };
            _service.Setup(s => s.Update(id, dto)).ReturnsAsync(BaseService.NoContent());

            var result = await Build(id).UpdateMe(dto);

            var status = Assert.IsType<StatusCodeResult>(result);
            Assert.Equal((int)HttpStatusCode.NoContent, status.StatusCode);
            _service.Verify(s => s.Update(id, dto), Times.Once);
        }

        [Fact]
        public async Task UpdateMe_WithoutClaim_ReturnsUnauthorized_AndSkipsService()
        {
            var result = await Build(null).UpdateMe(new UserUpdateDto { Name = "X" });

            Assert.IsType<UnauthorizedResult>(result);
            _service.Verify(s => s.Update(It.IsAny<Guid>(), It.IsAny<UserUpdateDto>()), Times.Never);
        }

        [Fact]
        public async Task DeleteMe_WithAuthenticatedUser_RemovesOwnAccount()
        {
            var id = Guid.NewGuid();
            _service.Setup(s => s.Remove(id)).ReturnsAsync(BaseService.NoContent());

            var result = await Build(id).DeleteMe();

            var status = Assert.IsType<StatusCodeResult>(result);
            Assert.Equal((int)HttpStatusCode.NoContent, status.StatusCode);
            _service.Verify(s => s.Remove(id), Times.Once);
        }

        [Fact]
        public async Task DeleteMe_WithoutClaim_ReturnsUnauthorized_AndSkipsService()
        {
            var result = await Build(null).DeleteMe();

            Assert.IsType<UnauthorizedResult>(result);
            _service.Verify(s => s.Remove(It.IsAny<Guid>()), Times.Never);
        }
    }
}
