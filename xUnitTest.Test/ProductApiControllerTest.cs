using Microsoft.AspNetCore.Mvc;
using Moq;
using xUnitTest.Web.Controllers.ApiController;
using xUnitTest.Web.Entities;
using xUnitTest.Web.Repository;

namespace xUnitTest.Test
{
    public class ProductApiControllerTest
    {
        private readonly Mock<IRepository<Product>> _mockRepo;
        private readonly ProductsController _controller;

        private List<Product> _products;

        public ProductApiControllerTest()
        {
            _mockRepo = new Mock<IRepository<Product>>();
            _controller = new ProductsController(_mockRepo.Object);

            _products = new List<Product>() { new Product
            {Id=1,Name="Kalem",Price=100,Stock=50,Color="Kırmızı" },
            new Product
            {Id=1,Name="defter",Price=200,Stock=500,Color="Mavi" } };
        }

        [Fact]
        public async void GetProduct_Action_Execute_ReturnOkResultWithProduct()
        {
            _mockRepo.Setup(x => x.GetAllAsync()).ReturnsAsync(_products);

            var result = await _controller.GetProducts();

            var okResult = Assert.IsType<OkObjectResult>(result);

            var returnProducts = Assert.IsType<IEnumerable<Product>>(okResult.Value);

            Assert.Equal<int>(2, returnProducts.ToList().Count);
        }

        [Theory]
        [InlineData(0)]
        public async void GetProducts_IdInvalid_ReturnNotFound(int productId)
        {
            Product product = null;
            _mockRepo.Setup(X => X.GetByIdAsync(productId)).ReturnsAsync(product);

            var result = await _controller.GetProduct(productId);
            Assert.IsType<NotFoundResult>(result);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        public async void GetProduct_IdValid_ReturnOkResult(int productId)
        {
            var product = _products.First(x => x.Id == productId);
            _mockRepo.Setup(x => x.GetByIdAsync(productId)).ReturnsAsync(product);
            var result = await _controller.GetProduct(productId);
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnProduct = Assert.IsType<Product>(okResult.Value);
            Assert.Equal(productId, returnProduct.Id);
            Assert.Equal(product.Name, returnProduct.Name);
        }

        [Theory]
        [InlineData(1)]
        public void PutProduct_IdIsNotEqualProduct_ReturnBadRequestResult(int productId)
        {
            var product = _products.First(x => x.Id == productId);

            var result = _controller.PutProduct(2, product);

            var badRequestResult = Assert.IsType<BadRequestResult>(result);
        }
    }
}