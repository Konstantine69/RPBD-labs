using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Moq;
using ProdajnikWeb.Controllers;
using ProdajnikWeb.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TestProject.Tests
{
    [TestClass]
    public class AdminPageControllerTests
    {
        private Mock<UserManager<ApplicationUser>> _userManagerMock;
        private Mock<RoleManager<IdentityRole>> _roleManagerMock;
        private AdminPageController _controller;

        [TestInitialize]
        public void Initialize()
        {
            var userStoreMock = new Mock<IUserStore<ApplicationUser>>();
            _userManagerMock = new Mock<UserManager<ApplicationUser>>(
                userStoreMock.Object, null, null, null, null, null, null, null, null);

            var roleStoreMock = new Mock<IRoleStore<IdentityRole>>();
            _roleManagerMock = new Mock<RoleManager<IdentityRole>>(
                roleStoreMock.Object, null, null, null, null);

            _controller = new AdminPageController(_userManagerMock.Object, _roleManagerMock.Object);
        }



        [TestMethod]
        public async Task IndexAdmin()
        {
            // Arrange
            var users = new List<ApplicationUser>
            {
                new ApplicationUser { Id = "1", UserName = "User1" },
                new ApplicationUser { Id = "2", UserName = "User2" }
            }.AsQueryable();

            _userManagerMock.Setup(m => m.Users).Returns(users);

            _userManagerMock.Setup(m => m.GetRolesAsync(It.IsAny<ApplicationUser>()))
                .ReturnsAsync(new List<string> { "admin" });

            // Act
            var result = await _controller.Index() as ViewResult;

            // Assert
            Assert.IsNotNull(result);
            var model = result.Model as List<(ApplicationUser user, IList<string> roles)>;
            Assert.IsNotNull(model);
            Assert.AreEqual(2, model.Count);
            Assert.AreEqual("User1", model[0].user.UserName);
            Assert.AreEqual("admin", model[0].roles.First());
        }


        [TestMethod]
        public async Task DetailsAdmin()
        {
            // Arrange
            var user = new ApplicationUser { Id = "1", UserName = "User1" };
            _userManagerMock.Setup(m => m.FindByIdAsync("1"))
                .ReturnsAsync(user);

            // Act
            var result = await _controller.Details("1");
            var viewResult = result as ViewResult;

            // Assert
            Assert.IsNotNull(viewResult);
            var model = viewResult.Model as ApplicationUser;
            Assert.IsNotNull(model);
            Assert.AreEqual("1", model.Id);
            Assert.AreEqual("User1", model.UserName);
        }

        [TestMethod]
        public async Task DetailsAdmin1()
        {
            // Arrange
            _userManagerMock.Setup(m => m.FindByIdAsync("99"))
                .ReturnsAsync((ApplicationUser)null);

            // Act
            var result = await _controller.Details("99");

            // Assert
            Assert.IsInstanceOfType(result, typeof(NotFoundResult));
        }

        [TestMethod]
        public async Task DetailsNotFound()
        {
            // Act
            var result = await _controller.Details(null);

            // Assert
            Assert.IsInstanceOfType(result, typeof(NotFoundResult));
        }
    }
}