using HttpMediatR.Samples.AspNetCoreMvc;
using HttpMediatR.Tests.Extensions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Logging;
using Moq;
using Newtonsoft.Json;
using Shouldly;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace HttpMediatR.Tests
{
    public class HttpHandlerTests
    {
        private readonly TestServer _server;
        private readonly HttpClient _client;

        public HttpHandlerTests()
        {
            _server = new TestServer(new WebHostBuilder().UseStartup<Startup>());
            _client = _server.CreateClient();
        }

        [Fact]
        public async Task GivenARequestForAProductThatExists_ReturnsExpectedResult()
        {
            // Arrange.
            var logger = new Mock<ILogger<Samples.AspNetCoreMvc.Handlers.GetProduct.Handler>>();
            var handler = new Samples.AspNetCoreMvc.Handlers.GetProduct.Handler(logger.Object);
            var query = new Samples.AspNetCoreMvc.Handlers.GetProduct.Input { ProductId = 1 };
            var expectedModel = new Samples.AspNetCoreMvc.Handlers.GetProduct.Output
            {
                ProductId = query.ProductId,
                Name = $"Product #{query.ProductId}"
            };

            // Act.
            var result = await _client.GetAsync($"/products/{query.ProductId}");

            // Assert.
            result.ShouldNotBeNull();
            result.StatusCode.ShouldBe(HttpStatusCode.OK);
            result.Content.ShouldNotBeNull();
            var resultObj = JsonConvert.DeserializeObject<Samples.AspNetCoreMvc.Handlers.GetProduct.Output>(await result.Content.ReadAsStringAsync());
            resultObj.ProductId.ShouldBe(expectedModel.ProductId);
            resultObj.Name.ShouldBe(expectedModel.Name);
        }

        [Fact]
        public async Task GivenARequestForAProductThatDoesntExist_ReturnsExpectedResult()
        {
            // Arrange.
            var logger = new Mock<ILogger<Samples.AspNetCoreMvc.Handlers.GetProduct.Handler>>();
            var handler = new Samples.AspNetCoreMvc.Handlers.GetProduct.Handler(logger.Object);
            var query = new Samples.AspNetCoreMvc.Handlers.GetProduct.Input { ProductId = 0 };

            // Act.
            var result = await _client.GetAsync($"/products/{query.ProductId}");

            // Assert.
            result.ShouldNotBeNull();
            result.StatusCode.ShouldBe(HttpStatusCode.NotFound);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task GivenAnOrderForAProductThatExists_ReturnsExpectedResult(bool includeModel)
        {
            // Arrange.
            var logger = new Mock<ILogger<Samples.AspNetCoreMvc.Handlers.OrderProduct.Handler>>();
            var handler = new Samples.AspNetCoreMvc.Handlers.OrderProduct.Handler(logger.Object);
            var command = new Samples.AspNetCoreMvc.Handlers.OrderProduct.Input { ProductId = 1, IncludeModel = includeModel };
            var expectedModel = new Samples.AspNetCoreMvc.Handlers.OrderProduct.Output
            {
                OrderId = command.ProductId + 1
            };

            // Act.
            var result = await _client.PostAsync("/orders", new JsonContent(command));

            // Assert.
            result.ShouldNotBeNull();
            result.StatusCode.ShouldBe(HttpStatusCode.Created);

            if (includeModel)
            {
                result.Content.ShouldNotBeNull();
                var resultObj = JsonConvert.DeserializeObject<Samples.AspNetCoreMvc.Handlers.OrderProduct.Output>(await result.Content.ReadAsStringAsync());
                resultObj.ShouldNotBeNull();
                resultObj.OrderId.ShouldBe(expectedModel.OrderId);
            }
        }

        [Fact]
        public async Task GivenAnOrderForAProductThatDoesntExist_ReturnsExpectedResult()
        {
            // Arrange.
            var logger = new Mock<ILogger<Samples.AspNetCoreMvc.Handlers.OrderProduct.Handler>>();
            var handler = new Samples.AspNetCoreMvc.Handlers.OrderProduct.Handler(logger.Object);
            var command = new Samples.AspNetCoreMvc.Handlers.OrderProduct.Input { ProductId = 0 };

            // Act.
            var result = await _client.PostAsync("/orders", new JsonContent(command));

            // Assert.
            result.ShouldNotBeNull();
            result.StatusCode.ShouldBe(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task GivenAnOrderForAProductThatCausesAConflict_ReturnsExpectedResult()
        {
            // Arrange.
            var logger = new Mock<ILogger<Samples.AspNetCoreMvc.Handlers.OrderProduct.Handler>>();
            var handler = new Samples.AspNetCoreMvc.Handlers.OrderProduct.Handler(logger.Object);
            var command = new Samples.AspNetCoreMvc.Handlers.OrderProduct.Input { ProductId = 1, CauseConflict = true };

            // Act.
            var result = await _client.PostAsync("/orders", new JsonContent(command));

            // Assert.
            result.ShouldNotBeNull();
            result.StatusCode.ShouldBe(HttpStatusCode.Conflict);
        }

        [Fact]
        public async Task GivenARequestTryingToDeleteAProductThatExists_ReturnsExpectedResult()
        {
            // Arrange.
            var logger = new Mock<ILogger<Samples.AspNetCoreMvc.Handlers.DeleteProduct.Handler>>();
            var handler = new Samples.AspNetCoreMvc.Handlers.DeleteProduct.Handler(logger.Object);
            var query = new Samples.AspNetCoreMvc.Handlers.DeleteProduct.Input { ProductId = 1 };

            // Act.
            var result = await _client.DeleteAsync($"/products/{query.ProductId}");

            // Assert.
            result.ShouldNotBeNull();
            result.StatusCode.ShouldBe(HttpStatusCode.NoContent);
        }

        [Fact]
        public async Task GivenARequestTryingToDeleteAProductThatDoesntExist_ReturnsExpectedResult()
        {
            // Arrange.
            var logger = new Mock<ILogger<Samples.AspNetCoreMvc.Handlers.DeleteProduct.Handler>>();
            var handler = new Samples.AspNetCoreMvc.Handlers.DeleteProduct.Handler(logger.Object);
            var query = new Samples.AspNetCoreMvc.Handlers.DeleteProduct.Input { ProductId = 0 };

            // Act.
            var result = await _client.DeleteAsync($"/products/{query.ProductId}");

            // Assert.
            result.ShouldNotBeNull();
            result.StatusCode.ShouldBe(HttpStatusCode.NotFound);
        }
    }
}