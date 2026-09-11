using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;

namespace VetFlow.API.Security;

/// <summary>
/// Opções do esquema de autenticação por API Key.
/// A chave esperada vem de Authentication:ApiKey (appsettings, variável de
/// ambiente ou user-secrets) — nunca deve ser commitada em texto plano.
/// </summary>
public class ApiKeyAuthenticationOptions : AuthenticationSchemeOptions
{
    public const string DefaultScheme = "ApiKey";
    public const string HeaderName = "X-Api-Key";
}

/// <summary>
/// Autenticação simples por API Key enviada no header X-Api-Key.
/// Usada para cobrir o requisito de autenticação nos testes de integração
/// (WebApplicationFactory) desta sprint, sem exigir um sistema completo de
/// usuários/login na camada .NET — isso já é responsabilidade da API Java
/// (Spring Security) dentro do challenge.
/// </summary>
public class ApiKeyAuthenticationHandler(
    IOptionsMonitor<ApiKeyAuthenticationOptions> options,
    ILoggerFactory logger,
    UrlEncoder encoder,
    IConfiguration configuration)
    : AuthenticationHandler<ApiKeyAuthenticationOptions>(options, logger, encoder)
{
    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        if (!Request.Headers.TryGetValue(ApiKeyAuthenticationOptions.HeaderName, out var providedKey))
        {
            return Task.FromResult(AuthenticateResult.Fail(
                $"Header '{ApiKeyAuthenticationOptions.HeaderName}' ausente."));
        }

        var expectedKey = configuration["Authentication:ApiKey"];

        if (string.IsNullOrWhiteSpace(expectedKey))
        {
            // Falha de configuração do servidor (chave não configurada), não do cliente.
            return Task.FromResult(AuthenticateResult.Fail(
                "Autenticação não configurada no servidor (Authentication:ApiKey ausente)."));
        }

        if (!string.Equals(providedKey.ToString(), expectedKey, StringComparison.Ordinal))
        {
            return Task.FromResult(AuthenticateResult.Fail("API Key inválida."));
        }

        var claims = new[] { new Claim(ClaimTypes.Name, "vetflow-client") };
        var identity = new ClaimsIdentity(claims, Scheme.Name);
        var principal = new ClaimsPrincipal(identity);
        var ticket = new AuthenticationTicket(principal, Scheme.Name);

        return Task.FromResult(AuthenticateResult.Success(ticket));
    }

    protected override Task HandleChallengeAsync(AuthenticationProperties properties)
    {
        Response.StatusCode = StatusCodes.Status401Unauthorized;
        return Response.WriteAsJsonAsync(new { error = "Não autorizado. Informe um header X-Api-Key válido." });
    }
}
