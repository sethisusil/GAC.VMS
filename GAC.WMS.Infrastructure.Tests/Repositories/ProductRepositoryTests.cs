using GAC.WMS.Application.Tests;
using GAC.WMS.Infrastructure.Data;
using GAC.WMS.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace GAC.WMS.Infrastructure.Tests.Repositories
{
    public class ProductRepositoryTests
    {
        private GacDbContext GetDbContext(string dbName)
        {
            var options = new DbContextOptionsBuilder<GacDbContext>()
                .UseInMemoryDatabase(databaseName: dbName)
                .Options;
            return new GacDbContext(options);
        }

        [Fact]
        public async Task AddAsync_ShouldAddProduct()
        {
            using var context = GetDbContext(nameof(AddAsync_ShouldAddProduct));
            var repo = new ProductRepository(context);
            var product = Setup.GetProductEntity(1);

            var result = await repo.AddAsync(product);

            Assert.NotNull(result);
            Assert.Equal(product.Code, result.Code);
            Assert.Single(context.Products);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnProduct_WhenExists()
        {
            using var context = GetDbContext(nameof(GetByIdAsync_ShouldReturnProduct_WhenExists));
            var product = Setup.GetProductEntity(2);
            context.Products.Add(product);
            context.SaveChanges();

            var repo = new ProductRepository(context);
            var result = await repo.GetByIdAsync(product.Id);

            Assert.NotNull(result);
            Assert.Equal(product.Code, result.Code);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnNull_WhenNotExists()
        {
            using var context = GetDbContext(nameof(GetByIdAsync_ShouldReturnNull_WhenNotExists));
            var repo = new ProductRepository(context);

            var result = await repo.GetByIdAsync(999);

            Assert.Null(result);
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnAllProducts()
        {
            using var context = GetDbContext(nameof(GetAllAsync_ShouldReturnAllProducts));
            context.Products.AddRange(
                Setup.GetProductEntity(1),
                Setup.GetProductEntity(2)
            );
            context.SaveChanges();

            var repo = new ProductRepository(context);
            var result = await repo.GetAllAsync();

            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
        }

        [Fact]
        public async Task UpdateAsync_ShouldUpdateProduct()
        {
            using var context = GetDbContext(nameof(UpdateAsync_ShouldUpdateProduct));
            var product = Setup.GetProductEntity(1);
            context.Products.Add(product);
            context.SaveChanges();

            var repo = new ProductRepository(context);
            product.Title = "New";
            await repo.UpdateAsync(product);

            var updated = await repo.GetByIdAsync(product.Id);
            Assert.Equal("New", updated.Title);
        }

        [Fact]
        public async Task DeleteAsync_ShouldRemoveProduct()
        {
            using var context = GetDbContext(nameof(DeleteAsync_ShouldRemoveProduct));
            var product = Setup.GetProductEntity(1);
            context.Products.Add(product);
            context.SaveChanges();

            var repo = new ProductRepository(context);
            await repo.DeleteAsync(product.Id);

            var deleted = await repo.GetByIdAsync(product.Id);
            Assert.Null(deleted);
            Assert.Empty(context.Products);
        }
        [Fact]
        public async Task GetWhereAsync_ShouldReturnMatchingProducts()
        {
            using var context = GetDbContext(nameof(GetWhereAsync_ShouldReturnMatchingProducts));
            var product1 = Setup.GetProductEntity(1);
            product1.Title = "Widget";
            var product2 = Setup.GetProductEntity(2);
            product2.Title = "Gadget";
            var product3 = Setup.GetProductEntity(3);
            product3.Title = "Widget";
            context.Products.AddRange(product1, product2, product3);
            context.SaveChanges();

            var repo = new ProductRepository(context);

            // Act: Get all products with Title "Widget"
            var result = await repo.GetWhereAsync(p => p.Title == "Widget");

            // Assert
            Assert.NotNull(result);
            var list = result.ToList();
            Assert.Equal(2, list.Count);
            Assert.All(list, p => Assert.Equal("Widget", p.Title));
        }

        [Fact]
        public async Task GetFirstAsync_ShouldReturnFirstMatchingProduct()
        {
            using var context = GetDbContext(nameof(GetFirstAsync_ShouldReturnFirstMatchingProduct));
            var product1 = Setup.GetProductEntity(1);
            product1.Title = "Widget";
            var product2 = Setup.GetProductEntity(2);
            product2.Title = "Gadget";
            context.Products.AddRange(product1, product2);
            context.SaveChanges();

            var repo = new ProductRepository(context);

            // Act: Get the first product with Title "Gadget"
            var result = await repo.GetFirstAsync(p => p.Title == "Gadget");

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Gadget", result.Title);
            Assert.Equal(product2.Id, result.Id);
        }
    }
}
