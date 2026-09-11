using System.Net;
using Xunit;

namespace VetFlow.IntegrationTests;

[Collection("VetFlow API Collection")]
public class HealthCheckTests(VetFlowApiFixture fixture)
{
    [Fact]
    public async Task HealthLive_DeveRetornarStatusOk()
    {
        // Arrange
        var client = fixture.Client;

        // Act
        var response = await client.GetAsync("/health/live");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task Health_DeveIncluirCorrelationIdNoHeaderDeResposta()
    {
        // Arrange
        var client = fixture.Client;

        // Act
        var response = await client.GetAsync("/health/live");

        // Assert
        Assert.True(response.Headers.Contains("X-Correlation-Id"));
    }
}
