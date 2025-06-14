using GAC.WMS.Application.Tests;
using GAC.WMS.Infrastructure.Data;
using GAC.WMS.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace GAC.WMS.Infrastructure.Tests.Repositories
{
    public class SalesOrderRepositoryTests
    {
        private GacDbContext GetDbContext(string dbName)
        {
            var options = new DbContextOptionsBuilder<GacDbContext>()
                .UseInMemoryDatabase(databaseName: dbName)
                .Options;
            return new GacDbContext(options);
        }

        [Fact]
        public async Task AddAsync_ShouldAddSalesOrder()
        {
            using var context = GetDbContext(nameof(AddAsync_ShouldAddSalesOrder));
            var repo = new SalesOrderRepository(context);
            var order = Setup.GetSalesOrderEntity(1);

            var result = await repo.AddAsync(order);

            Assert.NotNull(result);
            Assert.Equal(order.CustomerId, result.CustomerId);
            Assert.Single(context.SalesOrders);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnSalesOrder_WhenExists()
        {
            using var context = GetDbContext(nameof(GetByIdAsync_ShouldReturnSalesOrder_WhenExists));
            var order = Setup.GetSalesOrderEntity(2);
            context.SalesOrders.Add(order);
            context.SaveChanges();

            var repo = new SalesOrderRepository(context);
            var result = await repo.GetByIdAsync(order.Id);

            Assert.NotNull(result);
            Assert.Equal(order.CustomerId, result.CustomerId);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnNull_WhenNotExists()
        {
            using var context = GetDbContext(nameof(GetByIdAsync_ShouldReturnNull_WhenNotExists));
            var repo = new SalesOrderRepository(context);

            var result = await repo.GetByIdAsync(999);

            Assert.Null(result);
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnAllSalesOrders()
        {
            using var context = GetDbContext(nameof(GetAllAsync_ShouldReturnAllSalesOrders));
            context.SalesOrders.AddRange(
                Setup.GetSalesOrderEntity(1),
                Setup.GetSalesOrderEntity(2)
            );
            context.SaveChanges();

            var repo = new SalesOrderRepository(context);
            var result = await repo.GetAllAsync();

            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
        }

        [Fact]
        public async Task UpdateAsync_ShouldUpdateSalesOrder()
        {
            using var context = GetDbContext(nameof(UpdateAsync_ShouldUpdateSalesOrder));
            var order = Setup.GetSalesOrderEntity(1);
            context.SalesOrders.Add(order);
            context.SaveChanges();

            var repo = new SalesOrderRepository(context);
            order.CustomerId = 99;
            await repo.UpdateAsync(order);

            var updated = await repo.GetByIdAsync(order.Id);
            Assert.Equal(order.CustomerId, updated.CustomerId);
        }

        [Fact]
        public async Task DeleteAsync_ShouldRemoveSalesOrder()
        {
            using var context = GetDbContext(nameof(DeleteAsync_ShouldRemoveSalesOrder));
            var order = Setup.GetSalesOrderEntity(1);
            context.SalesOrders.Add(order);
            context.SaveChanges();

            var repo = new SalesOrderRepository(context);
            await repo.DeleteAsync(order.Id);

            var deleted = await repo.GetByIdAsync(order.Id);
            Assert.Null(deleted);
            Assert.Empty(context.SalesOrders);
        }
        [Fact]
        public async Task GetWhereAsync_ShouldReturnMatchingSalesOrders()
        {
            using var context = GetDbContext(nameof(GetWhereAsync_ShouldReturnMatchingSalesOrders));
            var order1 = Setup.GetSalesOrderEntity(1);
            var order2 = Setup.GetSalesOrderEntity(2);
            var order3 = Setup.GetSalesOrderEntity(3);
            context.SalesOrders.AddRange(order1, order2, order3);
            context.SaveChanges();

            var repo = new SalesOrderRepository(context);

            // Act: Get all sales orders for CustomerId 10
            var result = await repo.GetWhereAsync(o => o.CustomerId == 1);

            // Assert
            Assert.NotNull(result);
            var list = result.ToList();
            Assert.Single(list);
            Assert.All(list, o => Assert.Equal(1, o.CustomerId));
        }

        [Fact]
        public async Task GetFirstAsync_ShouldReturnFirstMatchingSalesOrder()
        {
            using var context = GetDbContext(nameof(GetFirstAsync_ShouldReturnFirstMatchingSalesOrder));
            var order1 = Setup.GetSalesOrderEntity(1);
            var order2 = Setup.GetSalesOrderEntity(2);
            context.SalesOrders.AddRange(order1, order2);
            context.SaveChanges();

            var repo = new SalesOrderRepository(context);

            // Act: Get the first sales order for CustomerId 20
            var result = await repo.GetFirstAsync(o => o.CustomerId == 2);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.CustomerId);
            Assert.Equal(order2.Id, result.Id);
        }
    }
}
