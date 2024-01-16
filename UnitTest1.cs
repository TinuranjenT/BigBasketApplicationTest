using BigBasketApplication;
using BigBasketApplication.Controllers;
using BigBasketApplication.Model;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace BigbasketTest
{
    public class Tests
    {
        private AdminController _adminController;
        private CustomerController _customerController;
        private Mock<IRepository> _mockRepository;

        [SetUp]
        public void Setup()
        {
            _mockRepository = new Mock<IRepository>();
            _adminController = new AdminController(_mockRepository.Object);
            _customerController = new CustomerController(_mockRepository.Object);
        }

        [Test]
        public async Task GetAllProducts_ReturnsAllProducts()
        {
            var product = new List<Product>
            {
                new Product { Id = 1, Name = "TestProduct1", Price = 50, StockQuantity = 100, DiscountPercentage = 2, GstPercentage = 3},
                new Product { Id = 2, Name = "TestProduct2", Price = 40, StockQuantity = 40, DiscountPercentage = 4, GstPercentage = 5},
                new Product { Id = 3, Name = "TestProduct3", Price = 250, StockQuantity = 20, DiscountPercentage = 7, GstPercentage = 2}
            };
            _mockRepository.Setup(x => x.GetAllProductsByAdmin()).ReturnsAsync(product);

            var actionResult = await _adminController.GetAllProductsByAdmin();
            var result = actionResult.Result as OkObjectResult;
            result.Should().NotBeNull();
            result.Value.Should().NotBeNull();
            result.Value.Should().Be(product);
        }

        [Test]
        [TestCase(1)]
        public async Task GetProductById_ReturnsSpecificProduct(int ProductId)
        {
            var product = new Product
            {
                Id = ProductId,
                Name = "TestProduct",
                Price = 50,
                StockQuantity = 100, 
                DiscountPercentage = 4,
                GstPercentage = 8
            };
            _mockRepository.Setup(x => x.GetProductById(ProductId)).ReturnsAsync(product);

            var actionResult = await _adminController.GetProductById(ProductId);
            var result = actionResult.Result as OkObjectResult;
            result.Should().NotBeNull();
            //result.First().Id.Should().Be(1);
            result.Value.Should().NotBeNull();
            result.Value.Should().Be(product);
           
        }

        
        [Test]
        [TestCase(1, 30)]
        public async Task RefillTheProduct_ReturnsUpdatedCount(int productId, int quantity)
        {
            var product = new Product { Id = productId, Name = "TestProduct3", Price = 70, StockQuantity = 50, DiscountPercentage = 4, GstPercentage = 5 };
            _mockRepository.Setup(x => x.RefillStock(productId, quantity)).ReturnsAsync(product);

            var actionResult = await _adminController.RefillStock(productId, quantity);
            var result = actionResult.Result as OkObjectResult;
            result.Should().NotBeNull();
            result.Value.Should().NotBeNull();
            result.Value.Should().Be(product);
        }


        [Test]
        public async Task GetAllProductsByCustomer()
        {
            var productForCustomer = new List<ProductForCustomer>
            {
                new ProductForCustomer { Id = 1, Name = "TestProduct1", Price = 50 },
                new ProductForCustomer { Id = 2, Name = "TestProduct2", Price = 40 },
                new ProductForCustomer { Id = 3, Name = "TEstProduct3", Price = 20 }

            };
            _mockRepository.Setup(x => x.GetAllProductsByCustomer()).ReturnsAsync(productForCustomer);

            var actionResult = await _customerController.GetAllProductsByCustomer();
            var result = actionResult.Result as OkObjectResult;
            result.Should().NotBeNull();
            result.Value.Should().NotBeNull();
            result.Value.Should().Be(productForCustomer);

        }

        [Test]
        [TestCase(1, 5)]
        public async Task AddToCart_ProductAvailable_ReturnsUpdatedCart(int productId, int quantity)
        {
            // Arrange
            var cart = new Cart
            {
                CustomerId = 1,
                Items = new List<CartItem>
        {
            new CartItem { ProductId = productId, Quantity = quantity }
        }
            };

            _mockRepository.Setup(x => x.AddToCart(cart.CustomerId, productId, quantity)).ReturnsAsync(cart);

            // Act
            var actionResult = await _customerController.AddToCart(cart);
            var result = actionResult.Result as OkObjectResult;

            // Assert
            result.Should().NotBeNull();
            result.Value.Should().NotBeNull();
            result.Value.Should().BeEquivalentTo(cart);
        }

        [Test]
        [TestCase(1)]
        public async Task GenerateBill_ItemsInCart_ReturnsOrderId(int customerId)
        {
            // Arrange
            var order = new Order { /* Your Order initialization logic here */ };
            _mockRepository.Setup(x => x.GenerateBill(customerId)).ReturnsAsync(order);

            // Act
            var actionResult = await _customerController.GenerateBill(customerId);
            var result = actionResult.Result as OkObjectResult;

            // Assert
            result.Should().NotBeNull();
            result.Value.Should().NotBeNull();
            result.Value.Should().BeEquivalentTo(order);
        }

        [Test]
        public async Task GetAllProductsFromCart_ReturnsListOfCarts()
        {
            // Arrange
            var carts = new List<Cart>
        {
            new Cart { Id = 1, CustomerId = 1, Items = new List<CartItem> { new CartItem { ProductId = 1, Quantity = 2 } } },
            new Cart { Id = 2, CustomerId = 1, Items = new List<CartItem> { new CartItem { ProductId = 2, Quantity = 3 } } },
            
        };
            _mockRepository.Setup(x => x.GetAllProductsFromCart()).ReturnsAsync(carts);

            var actionResult = await _customerController.GetAllProductsFromCart();
            var result = actionResult.Result as OkObjectResult;

            result.Should().NotBeNull();
            var returnedCarts = result.Value as List<Cart>;
            returnedCarts.Should().NotBeNull();
            returnedCarts.Should().BeEquivalentTo(carts);
        }

        [Test]
        public async Task PostProduct_PostTheProduct()
        {

            var productToPost = new Product
            {
                Id = 1,
                Name = "TestProduct2",
                Price = 20,
                StockQuantity = 30,
                DiscountPercentage = 5,
                GstPercentage = 4,
            };

            _mockRepository.Setup(x => x.PostNewProduct(productToPost));

            var actionResult = await _adminController.PostNewProduct(productToPost);
            var result = actionResult.Result as CreatedAtActionResult;

            result.Should().NotBeNull();
            result.Value.Should().NotBeNull();
            result.Value.Should().BeEquivalentTo(productToPost);

            //_mockRepository.Verify();
        }

    }
}