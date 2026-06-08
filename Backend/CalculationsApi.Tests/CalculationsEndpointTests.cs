using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;
using System.Net.Http.Json;

namespace CalculationsApi.Tests
{
    public class CalculationsEndpointTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;

        public CalculationsEndpointTests(WebApplicationFactory<Program> factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task PostCalculation_ReturnsOk_ForValidRequest()
        {
            // Arrange
            var response = await _client.PostAsJsonAsync(
                "/calculation/either",
                new
                {
                    probabilityA = 0.5m,
                    probabilityB = 0.5m
                });

            
            // Act
            var result = await response.Content
                .ReadFromJsonAsync<CalculationResponse>();


            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal(0.75m, result!.Value);
        }

        [Fact]
        public async Task PostCalculation_ReturnsNotFound_ForUnknownCalculation()
        {
            var response = await _client.PostAsJsonAsync(
                "/calculation/unknown",
                new { value = 123 });


            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task PostCalculation_ReturnsBadRequest_ForValidationError()
        {
            var response = await _client.PostAsJsonAsync(
                "/calculation/either",
                new
                {
                    probabilityA = 1.5m,
                    probabilityB = 0.5m
                });

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }
    }
}
