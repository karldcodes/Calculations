using Microsoft.AspNetCore.Http;
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
        public async Task PostCalculation_Either_ValidRequest_ReturnsExpectedResult()
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
        public async Task PostCalculation_UnknownCalculation_ReturnsNotFound()
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
        public async Task PostCalculation_Either_ValidationError_ReturnsBadRequest(string value)
        {
            var response = await _client.PostAsJsonAsync(
                "/calculations/either",
                new
                {
                    probabilityA = value,
                    probabilityB = "0.5"
                });

            // Act
            var problem = await response.Content.ReadFromJsonAsync<HttpValidationProblemDetails>();


            // Assert
            Assert.Contains(
                "ProbabilityA",
                problem!.Errors.Keys);
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [InlineData("1.5")]
        [InlineData("1.1")]
        [InlineData("1.2")]
        [InlineData("1.3")]
        [Theory]
        public async Task PostCalculation_Combinedwith_ValidationError_ReturnsBadRequest(string value)
        {
            var response = await _client.PostAsJsonAsync(
                "/calculations/combinedwith",
                new
                {
                    probabilityA = value,
                    probabilityB = "0.5"
                });

            // Act
            var problem = await response.Content.ReadFromJsonAsync<HttpValidationProblemDetails>();


            // Assert
            Assert.Contains(
                "ProbabilityA",
                problem!.Errors.Keys);
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task PostCalculation_Either_WrongRequestBody_ReturnsBadRequest()
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

        [Fact]
        public async Task PostCalculation_CombinedWith_ValidRequset_ReturnsExpectedResult()
        {
            var response = await _client.PostAsJsonAsync(
                "/calculations/combinedwith",
                new
                {
                    probabilityA = "0.5",
                    probabilityB = "0.5"
                });

            var result = await response.Content
                .ReadFromJsonAsync<CalculationResponse>();

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal(0.25m, result!.Value);
        }

        [Fact]
        public async Task PostCalculation_Either_InvalidJsonValue_ReturnsBadRequest()
        {
            var response = await _client.PostAsJsonAsync(
                "/calculations/either",
                new
                {
                    probabilityA = "banana",
                    probabilityB = "0.5"
                });

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task PostCalculation_Combinedwith_InvalidJsonValue_ReturnsBadRequest()
        {
            var response = await _client.PostAsJsonAsync(
                "/calculations/combinedwith",
                new
                {
                    probabilityA = "banana",
                    probabilityB = "0.5"
                });

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task GetCalculations_ReturnsMetadata()
        {
            var response = await _client.GetAsync("/calculations");

            var metadata =
                await response.Content.ReadFromJsonAsync<List<CalculationMetadata>>();

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            Assert.NotNull(metadata);
            Assert.True(metadata.Any());
        }
    }
}
