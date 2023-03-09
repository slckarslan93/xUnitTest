using Microsoft.AspNetCore.Mvc;
using Moq;
using xUnitTest.Web.Controllers;
using xUnitTest.Web.Entities;
using xUnitTest.Web.Repository;

namespace xUnitTest.Test
{
    public class ProductControllerTest
    {
        private readonly Mock<IRepository<Product>> _mockRepo;
        private readonly ProductsController _controller;
        private List<Product> _products;

        public ProductControllerTest()
        {
            _mockRepo = new Mock<IRepository<Product>>();
            _controller = new ProductsController(_mockRepo.Object);

            _products = new List<Product>() { new Product
            {Id=1,Name="Kalem",Price=100,Stock=50,Color="Kırmızı" },
            new Product
            {Id=1,Name="defter",Price=200,Stock=500,Color="Mavi" } };
        }

        [Fact]
        public async void Index_ActionExecutes_ReturnView()
        {
            var result = await _controller.Index();
            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public async void Index_ActionExecutes_ReturnProductList()
        {
            _mockRepo.Setup(repo => repo.GetAllAsync()).ReturnsAsync(_products);
            var result = await _controller.Index();

            var viewResult = Assert.IsType<ViewResult>(result);

            var ProductList = Assert.IsAssignableFrom<IEnumerable<Product>>(viewResult.Model);

            Assert.Equal<int>(2, ProductList.Count());
        }

        [Fact]
        public async void Detail_IdIsNull_ReturnRedirectToIndexAction()
        {
            var result = await _controller.Details(null);
            var redirect = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirect.ActionName);
        }
    }
}