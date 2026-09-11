using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using VetFlow.API;
using VetFlow.API.Security;
using Xunit;

namespace VetFlow.IntegrationTests;

/// <summary>
/// Fixture compartilhada entre os testes de integração: sobe uma instância
/// da API em memória uma única vez para toda a coleção de testes.
///
/// Configura uma Authentication:ApiKey de teste (nunca a chave real usada em
/// produção) e expõe dois clientes: um autenticado (Client) e um sem
/// credenciais (UnauthenticatedClient), para validar o fluxo completo de
/// autenticação — sucesso e falha (401) — nos testes de integração.
/// </summary>
public class VetFlowApiFixture : WebApplicationFactory<Program>, IAsyncLifetime
{
    public const string TestApiKey = "integration-tests-key";

    public HttpClient Client { get; private set; } = null!;
    public HttpClient UnauthenticatedClient { get; private set; } = null!;

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureAppConfiguration((_, config) =>
        {
            config.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Authentication:ApiKey"] = TestApiKey
            });
        });
    }

    public Task InitializeAsync()
    {
        UnauthenticatedClient = CreateClient();

        Client = CreateClient();
        Client.DefaultRequestHeaders.Add(ApiKeyAuthenticationOptions.HeaderName, TestApiKey);

        return Task.CompletedTask;
    }

    public new Task DisposeAsync()
    {
        Client.Dispose();
        UnauthenticatedClient.Dispose();
        return Task.CompletedTask;
    }
}

[CollectionDefinition("VetFlow API Collection")]
public class VetFlowApiCollection : ICollectionFixture<VetFlowApiFixture>
{
    // Marcador de collection fixture — sem implementação, só agrupa os testes
    // que compartilham a mesma instância de VetFlowApiFixture.
}
