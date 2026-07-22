using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace MaintenanceProject.Tests.IntegrationTests
{
    public class MaintenanceRequestsIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;

        public MaintenanceRequestsIntegrationTests(WebApplicationFactory<Program> factory)
        {
            _factory = factory;
        }

        [Fact]
        public async Task Get_Index_ReturnsSuccessAndHtml()
        {
            var client = _factory.CreateClient();
            var response = await client.GetAsync("/MaintenanceRequests");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var content = await response.Content.ReadAsStringAsync();
            Assert.Contains("Maintenance Requests", content);
        }
    }
}
