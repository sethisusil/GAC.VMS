using GAC.WMS.Infrastructure.Data;
using GAC.WMS.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using GAC.WMS.Application.Tests;
using GAC.WMS.Domain.Entities;

namespace GAC.WMS.Infrastructure.Tests.Repositories
{
    public class CustomerRepositoryTests
    {
        private GacDbContext GetDbContext(string dbName)
        {
            var options = new DbContextOptionsBuilder<GacDbContext>()
                .UseInMemoryDatabase(databaseName: dbName)
                .Options;
            return new GacDbContext(options);
        }

        [Fact]
        public async Task AddAsync_ShouldAddCustomer()
        {
            using var context = GetDbContext(nameof(AddAsync_ShouldAddCustomer));
            var repo = new CustomerRepository(context);
            var customer = Setup.GetGetCustomerEntity(1);

            var result = await repo.AddAsync(customer);

            Assert.NotNull(result);
            Assert.Equal(customer.Name, result.Name);
            Assert.Equal(1, context.Customers.Count());
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnCustomer_WhenExists()
        {
            using var context = GetDbContext(nameof(GetByIdAsync_ShouldReturnCustomer_WhenExists));
            var customer = Setup.GetGetCustomerEntity(1);
            context.Customers.Add(customer);
            context.SaveChanges();

            var repo = new CustomerRepository(context);
            var result = await repo.GetByIdAsync(customer.Id);

            Assert.NotNull(result);
            Assert.Equal(customer.Name, result.Name);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnNull_WhenNotExists()
        {
            using var context = GetDbContext(nameof(GetByIdAsync_ShouldReturnNull_WhenNotExists));
            var repo = new CustomerRepository(context);

            var result = await repo.GetByIdAsync(999);

            Assert.Null(result);
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnAllCustomers()
        {
            using var context = GetDbContext(nameof(GetAllAsync_ShouldReturnAllCustomers));
            context.Customers.AddRange(
                Setup.GetGetCustomerEntity(1),
                Setup.GetGetCustomerEntity(2)
            );
            context.SaveChanges();

            var repo = new CustomerRepository(context);
            var result = await repo.GetAllAsync();

            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
        }

        [Fact]
        public async Task UpdateAsync_ShouldUpdateCustomer()
        {
            using var context = GetDbContext(nameof(UpdateAsync_ShouldUpdateCustomer));
            var customer = Setup.GetGetCustomerEntity(1);
            context.Customers.Add(customer);
            context.SaveChanges();

            var repo = new CustomerRepository(context);
            customer.Name = "New Name";
            await repo.UpdateAsync(customer);

            var updated = await repo.GetByIdAsync(customer.Id);
            Assert.Equal("New Name", updated.Name);
        }

        [Fact]
        public async Task DeleteAsync_ShouldRemoveCustomer()
        {
            using var context = GetDbContext(nameof(DeleteAsync_ShouldRemoveCustomer)+1);
            var customer = Setup.GetGetCustomerEntity(6);
            context.Customers.Add(customer);
            context.SaveChanges();

            var repo = new CustomerRepository(context);
            await repo.DeleteAsync(6);

            var deleted = await repo.GetByIdAsync(6);
            Assert.Null(deleted);
            Assert.False(context.Customers.Any(x=>x.Id==6));
        }
        [Fact]
        public async Task GetWhereAsync_ReturnsMatchingCustomers()
        {
            // Arrange
            using var context = GetDbContext(nameof(DeleteAsync_ShouldRemoveCustomer));
            var customer = Setup.GetGetCustomerEntity(1);
            context.Customers.Add(customer);
            context.SaveChanges();
            var repo = new Repository<Customer>(context);

            // Act
            var activeCustomers = await repo.GetWhereAsync(c => c.Name==customer.Name);

            // Assert
            Assert.Single(activeCustomers);
            Assert.All(activeCustomers, c => Assert.True(c.Name == customer.Name));
        }

        [Fact]
        public async Task GetFirstAsync_ReturnsFirstMatchingCustomer()
        {
            // Arrange
            using var context = GetDbContext(nameof(DeleteAsync_ShouldRemoveCustomer));
            var customer = Setup.GetGetCustomerEntity(6);
            context.Customers.Add(customer);
            context.SaveChanges();
            var repo = new Repository<Customer>(context);

            // Act
            var activeCustomers = await repo.GetFirstAsync(c => c.Name == customer.Name);

            // Assert
            Assert.NotNull(activeCustomers);
            Assert.Equal(activeCustomers.Name, customer.Name);
        }
    }
}
