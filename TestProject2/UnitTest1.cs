using lab6.Controllers;
using lab6.Data;
using lab6.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Xunit;

namespace TestProject1.Tests
{
    public class CustomersControllerTests
    {
        private readonly CustomersAPIController _controller;
        private readonly Mock<DbSet<Customer>> _mockSet;
        private readonly Mock<ProdajnikContext> _mockContext;

        public CustomersControllerTests()
        {
            // Мокируем DbSet для Customer
            _mockSet = new Mock<DbSet<Customer>>();

            // Мокируем контекст базы данных
            _mockContext = new Mock<ProdajnikContext>();
            _mockContext.Setup(m => m.Customers).Returns(_mockSet.Object);

            // Создаем контроллер с моком
            _controller = new CustomersAPIController(_mockContext.Object);
        }

        [Fact]
        public async Task GetCustomer_ReturnsNotFound_WhenCustomerDoesNotExist()
        {
            // Arrange
            var customerId = 999; // Идентификатор не существует
            _mockContext.Setup(m => m.Customers.FindAsync(customerId)).ReturnsAsync((Customer)null);

            // Act
            var result = await _controller.GetCustomer(customerId);

            // Assert
            Assert.IsType<NotFoundResult>(result.Result);
        }

        [Fact]
        public async Task PostCustomer_CreatesCustomer()
        {
            // Arrange
            var newCustomer = new Customer { OrganizationName = "Tech Corp", City = "Moscow", Address = "123 Tech St", PhoneNumber = "123-456-7890" };
            _mockContext.Setup(m => m.Customers.AddAsync(newCustomer, default)).ReturnsAsync((Microsoft.EntityFrameworkCore.ChangeTracking.EntityEntry<Customer>)null);

            // Act
            var result = await _controller.PostCustomer(newCustomer);

            // Assert
            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result.Result);
            Assert.Equal("GetCustomer", createdAtActionResult.ActionName);
            Assert.Equal(newCustomer, createdAtActionResult.Value);
        }

        [Fact]
        public async Task DeleteCustomer_ReturnsNoContent_WhenCustomerIsDeleted()
        {
            // Arrange
            var customerId = 1;
            var customer = new Customer { CustomerId = customerId, OrganizationName = "Tech Corp", City = "Moscow", Address = "123 Tech St", PhoneNumber = "123-456-7890" };
            _mockContext.Setup(m => m.Customers.FindAsync(customerId)).ReturnsAsync(customer);

            // Act
            var result = await _controller.DeleteCustomer(customerId);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public void BuildingMaterial_Validation_ShouldPass_WhenAllFieldsAreValid()
        {
            // Arrange
            var material = new BuildingMaterial
            {
                MaterialName = "Concrete",
                Manufacturer = "BestConcrete",
                PurchaseVolume = 500,
                CertificateNumber = "12345",
                CertificateDate = new DateOnly(2023, 12, 31)
            };

            var validationResults = new List<ValidationResult>();
            var isValid = Validator.TryValidateObject(material, new ValidationContext(material), validationResults, true);

            // Assert
            Assert.True(isValid); // Object is valid
            Assert.Empty(validationResults); // No validation errors
        }

        [Fact]
        public void ConstructionObject_Validation_ShouldPass_WhenAllFieldsAreValid()
        {
            // Arrange
            var constructionObject = new ConstructionObject
            {
                ObjectName = "Building A",
                CustomerId = 1,
                GeneralContractor = "ABC Contractors",
                ContractDate = new DateOnly(2024, 5, 15)
            };

            var validationResults = new List<ValidationResult>();
            var isValid = Validator.TryValidateObject(constructionObject, new ValidationContext(constructionObject), validationResults, true);

            // Assert
            Assert.True(isValid); // Object is valid
            Assert.Empty(validationResults); // No validation errors
        }

        [Fact]
        public void Customer_Validation_ShouldPass_WhenAllFieldsAreValid()
        {
            // Arrange
            var customer = new Customer
            {
                OrganizationName = "Big Corp",
                City = "New York",
                Address = "123 Main St",
                PhoneNumber = "123-456-7890"
            };

            var validationResults = new List<ValidationResult>();
            var isValid = Validator.TryValidateObject(customer, new ValidationContext(customer), validationResults, true);

            // Assert
            Assert.True(isValid); // Object is valid
            Assert.Empty(validationResults); // No validation errors
        }
    }
}
