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
                "/calculations/either",
                new
                {
                    probabilityA = "0.5",
                    probabilityB = "0.5"
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
                "/calculations/unknown",
                new { value = 123 });


            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [InlineData("1.5")]
        [InlineData("1.1")]
        [InlineData("1.2")]
        [InlineData("1.3")]
        [Theory]
        public async Task PostCalculation_ReturnsBadRequest_ForValidationError(string value)
        {
            var response = await _client.PostAsJsonAsync(
                "/calculations/either",
                new
                {
                    probabilityA = value,
                    probabilityB = "0.5"
                });

            // Act
            var result = await response.Content.ReadAsStringAsync();

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task PostCalculation_ReturnsBadRequest_ForWrongRequestBody()
        {
            var response = await _client.PostAsJsonAsync(
                "/calculations/either",
                new
                {
                    probabilityAAAAA = "1.0",
                    probabilityB = "0.5"
                });

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }
    }
}
