using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
    }
}