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

namespace HttpMediatR.Tests.HttpHandler
{
    public class HandleCoreTests
    {
        private readonly TestServer _server;
        private readonly HttpClient _client;

        public HandleCoreTests()
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
            var query = new Samples.AspNetCoreMvc.Handlers.GetProduct.Input { Id = 1 };
            var expectedModel = new Samples.AspNetCoreMvc.Handlers.GetProduct.Output
            {
                Id = query.Id,
                Name = $"Product #{query.Id}"
            };

            // Act.
            var result = await _client.GetAsync($"/products/{query.Id}");

            // Assert.
            result.ShouldNotBeNull();
            result.StatusCode.ShouldBe(HttpStatusCode.OK);
            result.Content.ShouldNotBeNull();
            var resultObj = JsonConvert.DeserializeObject<Samples.AspNetCoreMvc.Handlers.GetProduct.Output>(await result.Content.ReadAsStringAsync());
            resultObj.Id.ShouldBe(expectedModel.Id);
            resultObj.Name.ShouldBe(expectedModel.Name);
        }

        [Fact]
        public async Task GivenARequestForAProductThatDoesntExist_ReturnsExpectedResult()
        {
            // Arrange.
            var logger = new Mock<ILogger<Samples.AspNetCoreMvc.Handlers.GetProduct.Handler>>();
            var handler = new Samples.AspNetCoreMvc.Handlers.GetProduct.Handler(logger.Object);
            var query = new Samples.AspNetCoreMvc.Handlers.GetProduct.Input { Id = 0 };

            // Act.
            var result = await _client.GetAsync($"/products/{query.Id}");

            // Assert.
            result.ShouldNotBeNull();
            result.StatusCode.ShouldBe(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task GivenAnOrderForAProductThatExists_ReturnsExpectedResult()
        {
            // Arrange.
            var logger = new Mock<ILogger<Samples.AspNetCoreMvc.Handlers.OrderProduct.Handler>>();
            var handler = new Samples.AspNetCoreMvc.Handlers.OrderProduct.Handler(logger.Object);
            var command = new Samples.AspNetCoreMvc.Handlers.OrderProduct.Input { ProductId = 1 };
            var expectedModel = new Samples.AspNetCoreMvc.Handlers.OrderProduct.Output
            {
                OrderId = command.ProductId + 1
            };

            // Act.
            var result = await _client.PostAsync("/orders", new JsonContent(command));

            // Assert.
            result.ShouldNotBeNull();
            result.StatusCode.ShouldBe(HttpStatusCode.Created);
            result.Content.ShouldNotBeNull();
            var resultObj = JsonConvert.DeserializeObject<Samples.AspNetCoreMvc.Handlers.OrderProduct.Output>(await result.Content.ReadAsStringAsync());
            resultObj.ShouldNotBeNull();
            resultObj.OrderId.ShouldBe(expectedModel.OrderId);
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
    }
}