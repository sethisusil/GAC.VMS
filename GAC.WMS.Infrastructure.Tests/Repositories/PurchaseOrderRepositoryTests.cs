using GAC.WMS.Application.Tests;
using GAC.WMS.Infrastructure.Data;
using GAC.WMS.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace GAC.WMS.Infrastructure.Tests.Repositories
{
    public class PurchaseOrderRepositoryTests
    {
        private GacDbContext GetDbContext(string dbName)
        {
            var options = new DbContextOptionsBuilder<GacDbContext>()
                .UseInMemoryDatabase(databaseName: dbName)
                .Options;
            return new GacDbContext(options);
        }

        [Fact]
        public async Task AddAsync_ShouldAddPurchaseOrder()
        {
            using var context = GetDbContext(nameof(AddAsync_ShouldAddPurchaseOrder));
            var repo = new PurchaseOrderRepository(context);
            var order = Setup.GetPurchaseOrderEntity(1);

            var result = await repo.AddAsync(order);

            Assert.NotNull(result);
            Assert.Equal(order.CustomerId, result.CustomerId);
            Assert.Single(context.PurchaseOrders);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnPurchaseOrder_WhenExists()
        {
            using var context = GetDbContext(nameof(GetByIdAsync_ShouldReturnPurchaseOrder_WhenExists));
            var order = Setup.GetPurchaseOrderEntity(2);
            context.PurchaseOrders.Add(order);
            context.SaveChanges();

            var repo = new PurchaseOrderRepository(context);
            var result = await repo.GetByIdAsync(order.Id);

            Assert.NotNull(result);
            Assert.Equal(order.CustomerId, result.CustomerId);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnNull_WhenNotExists()
        {
            using var context = GetDbContext(nameof(GetByIdAsync_ShouldReturnNull_WhenNotExists));
            var repo = new PurchaseOrderRepository(context);

            var result = await repo.GetByIdAsync(999);

            Assert.Null(result);
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnAllPurchaseOrders()
        {
            using var context = GetDbContext(nameof(GetAllAsync_ShouldReturnAllPurchaseOrders));
            context.PurchaseOrders.AddRange(
                Setup.GetPurchaseOrderEntity(1),
                Setup.GetPurchaseOrderEntity(2)
            );
            context.SaveChanges();

            var repo = new PurchaseOrderRepository(context);
            var result = await repo.GetAllAsync();

            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
        }

        [Fact]
        public async Task UpdateAsync_ShouldUpdatePurchaseOrder()
        {
            using var context = GetDbContext(nameof(UpdateAsync_ShouldUpdatePurchaseOrder));
            var order = Setup.GetPurchaseOrderEntity(1);
            context.PurchaseOrders.Add(order);
            context.SaveChanges();

            var repo = new PurchaseOrderRepository(context);
            order.CustomerId = 99;
            await repo.UpdateAsync(order);

            var updated = await repo.GetByIdAsync(order.Id);
            Assert.Equal(order.CustomerId, updated.CustomerId);
        }

        [Fact]
        public async Task DeleteAsync_ShouldRemovePurchaseOrder()
        {
            using var context = GetDbContext(nameof(DeleteAsync_ShouldRemovePurchaseOrder));
            var order = Setup.GetPurchaseOrderEntity(1);
            context.PurchaseOrders.Add(order);
            context.SaveChanges();

            var repo = new PurchaseOrderRepository(context);
            await repo.DeleteAsync(order.Id);

            var deleted = await repo.GetByIdAsync(order.Id);
            Assert.Null(deleted);
            Assert.Empty(context.PurchaseOrders);
        }
        [Fact]
        public async Task GetWhereAsync_ShouldReturnMatchingPurchaseOrders()
        {
            using var context = GetDbContext(nameof(GetWhereAsync_ShouldReturnMatchingPurchaseOrders));
            var order1 = Setup.GetPurchaseOrderEntity(1);
            var order2 = Setup.GetPurchaseOrderEntity(2);
            var order3 = Setup.GetPurchaseOrderEntity(3);
            context.PurchaseOrders.AddRange(order1, order2, order3);
            context.SaveChanges();

            var repo = new PurchaseOrderRepository(context);

            // Act: Get all purchase orders for CustomerId 10
            var result = await repo.GetWhereAsync(o => o.CustomerId == 1);

            // Assert
            Assert.NotNull(result);
            var list = result.ToList();
            Assert.Single(list);
            Assert.All(list, o => Assert.Equal(1, o.CustomerId));
        }

        [Fact]
        public async Task GetFirstAsync_ShouldReturnFirstMatchingPurchaseOrder()
        {
            using var context = GetDbContext(nameof(GetFirstAsync_ShouldReturnFirstMatchingPurchaseOrder));
            var order1 = Setup.GetPurchaseOrderEntity(1);
            var order2 = Setup.GetPurchaseOrderEntity(2);
            context.PurchaseOrders.AddRange(order1, order2);
            context.SaveChanges();

            var repo = new PurchaseOrderRepository(context);

            // Act: Get the first purchase order for CustomerId 20
            var result = await repo.GetFirstAsync(o => o.CustomerId == 1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.CustomerId);
            Assert.Equal(order1.Id, result.Id);
        }
    }
}
