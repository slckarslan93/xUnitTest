using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Drawing.Printing;
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

        [Fact]
        public async void Details_IdInValid_ReturnNotFound()
        {
            Product product = null;
            _mockRepo.Setup(x => x.GetByIdAsync(0)).ReturnsAsync(product);

            var result = await _controller.Details(0);

            var redirect = Assert.IsType<NotFoundResult>(result);

            Assert.Equal<int>(404, redirect.StatusCode);
        }

        [Theory]
        [InlineData(1)]
        public async void Details_Action_ValidId_ReturnProduct(int ProductId)
        {
            Product product = _products.First(x => x.Id == ProductId);
            _mockRepo.Setup(repo => repo.GetByIdAsync(ProductId)).ReturnsAsync(product);

            var result = await _controller.Details(ProductId);

            var viewResult = Assert.IsType<ViewResult>(result);

            var resultProduct = Assert.IsAssignableFrom<Product>(viewResult.Model);

            Assert.Equal(product.Id, resultProduct.Id);
            Assert.Equal(product.Name, resultProduct.Name);
        }

        [Fact]
        public void Create_ActionExecute_ReturnView()
        {
            var result = _controller.Create();
            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public async void CreatePOST_InValidModelState_ReturnView()
        {
            _controller.ModelState.AddModelError("Name", "Name alanı gereklidir");
            var result = await _controller.Create(_products.First());
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.IsType<Product>(viewResult.Model);
        }

        [Fact]
        public async void CreatePOST_ValidModelState_ReturnRedirectToIndexAction()
        {
            var result = await _controller.Create(_products.First());

            var redirect = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirect.ActionName);
        }

        [Fact]
        public async void CreatePOST_ValidModelState_CreateMethodExecute()
        {
            Product newProduct = null;
            _mockRepo.Setup(repo => repo.CreateAsync(It.IsAny<Product>())).Callback<Product>(x => newProduct = x);

            var result = await _controller.Create(_products.First());

            _mockRepo.Verify(repo => repo.CreateAsync(It.IsAny<Product>()), Times.Once);

            Assert.Equal(_products.First().Id, newProduct.Id);
        }

        [Fact]
        public async void CreatePOST_InValidModalState_NeverCreateExecute()
        {
            _controller.ModelState.AddModelError("Name", "");
            var result = await _controller.Create(_products.First());
            _mockRepo.Verify(repo => repo.CreateAsync(It.IsAny<Product>()), Times.Never);
        }

        [Fact]
        public async void Edit_IdIsNull_ReturnRedirectToIndexAction()
        {
            var result = await _controller.Edit(null);
            var redirect = Assert.IsType<RedirectToActionResult>(result);

            Assert.Equal("Index", redirect.ActionName);
        }

        [Theory]
        [InlineData(3)]
        public async void Edit_IdInValid_ReturnNotFound(int productId)
        {
            Product product = null;
            _mockRepo.Setup(x => x.GetByIdAsync(productId)).ReturnsAsync(product);

            var result = await _controller.Edit(productId);
            var redirect = Assert.IsType<NotFoundResult>(result);
            Assert.Equal<int>(404, redirect.StatusCode);
        }
    }
}