using System.Net;
using System.Net.Http.Json;
using VetFlow.Application.DTOs;
using Xunit;

namespace VetFlow.IntegrationTests;

[Collection("VetFlow API Collection")]
public class TutorEndpointsTests(VetFlowApiFixture fixture)
{
    [Fact]
    public async Task GetAll_DeveRetornarStatusOk()
    {
        // Arrange
        var client = fixture.Client;

        // Act
        var response = await client.GetAsync("/api/Tutor");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task GetAll_SemApiKey_DeveRetornarUnauthorized()
    {
        // Arrange
        var client = fixture.UnauthenticatedClient;

        // Act
        var response = await client.GetAsync("/api/Tutor");

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task GetAll_ApiKeyInvalida_DeveRetornarUnauthorized()
    {
        // Arrange
        using var request = new HttpRequestMessage(HttpMethod.Get, "/api/Tutor");
        request.Headers.Add("X-Api-Key", "chave-errada");

        // Act
        var response = await fixture.UnauthenticatedClient.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Create_DadosValidos_DeveRetornarCreated()
    {
        // Arrange
        var client = fixture.Client;
        var request = new TutorRequest(
            $"Teste Integração {Guid.NewGuid():N}",
            $"teste.{Guid.NewGuid():N}@email.com",
            "11999999999");

        // Act
        var response = await client.PostAsJsonAsync("/api/Tutor", request);

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var body = await response.Content.ReadFromJsonAsync<TutorResponse>();
        Assert.NotNull(body);
        Assert.Equal(request.Name, body!.Name);
    }

    [Fact]
    public async Task Create_CamposObrigatoriosAusentes_DeveRetornarBadRequest()
    {
        // Arrange
        var client = fixture.Client;
        var requestInvalido = new TutorRequest("", "", "");

        // Act
        var response = await client.PostAsJsonAsync("/api/Tutor", requestInvalido);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task GetById_IdInexistente_DeveRetornarNotFound()
    {
        // Arrange
        var client = fixture.Client;
        var idInexistente = Guid.NewGuid();

        // Act
        var response = await client.GetAsync($"/api/Tutor/{idInexistente}");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task FluxoCompleto_CriarBuscarDeletar_DeveFuncionarDePontaAPonta()
    {
        // Arrange
        var client = fixture.Client;
        var request = new TutorRequest(
            $"Fluxo Completo {Guid.NewGuid():N}",
            $"fluxo.{Guid.NewGuid():N}@email.com",
            "11988887777");

        // Act 1 - Cria
        var createResponse = await client.PostAsJsonAsync("/api/Tutor", request);
        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);
        var created = await createResponse.Content.ReadFromJsonAsync<TutorResponse>();

        // Act 2 - Busca
        var getResponse = await client.GetAsync($"/api/Tutor/{created!.Id}");
        Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);

        // Act 3 - Deleta
        var deleteResponse = await client.DeleteAsync($"/api/Tutor/{created.Id}");
        Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);

        // Assert final - não deve mais existir
        var getAposDeleteResponse = await client.GetAsync($"/api/Tutor/{created.Id}");
        Assert.Equal(HttpStatusCode.NotFound, getAposDeleteResponse.StatusCode);
    }
}
