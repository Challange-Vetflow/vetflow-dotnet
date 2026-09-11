using VetFlow.Domain.Entities;
using Xunit;

namespace VetFlow.UnitTests.Domain;

public class TutorTests
{
    [Fact]
    public void Criar_DadosValidos_DeveCriarTutorComSucesso()
    {
        // Arrange
        var name = "Carlos Mendes";
        var email = "carlos.mendes@email.com";
        var phone = "(11) 99001-1111";

        // Act
        var tutor = new Tutor(name, email, phone);

        // Assert
        Assert.Equal(name, tutor.Name);
        Assert.Equal(email.ToLowerInvariant(), tutor.Email);
        Assert.Equal(phone, tutor.Phone);
    }

    [Fact]
    public void Criar_NomeVazio_DeveLancarExcecao()
    {
        // Arrange
        var email = "carlos.mendes@email.com";
        var phone = "(11) 99001-1111";

        // Act
        var act = () => new Tutor("", email, phone);

        // Assert
        Assert.Throws<Exception>(act);
    }

    [Theory]
    [InlineData("emailinvalido")]
    [InlineData("")]
    [InlineData("   ")]
    public void Criar_EmailInvalido_DeveLancarExcecao(string emailInvalido)
    {
        // Arrange
        var name = "Carlos Mendes";
        var phone = "(11) 99001-1111";

        // Act
        var act = () => new Tutor(name, emailInvalido, phone);

        // Assert
        Assert.Throws<Exception>(act);
    }

    [Fact]
    public void UpdateName_NomeValido_DeveAtualizarNome()
    {
        // Arrange
        var tutor = new Tutor("Carlos Mendes", "carlos@email.com", "11999999999");
        var novoNome = "Carlos Eduardo Mendes";

        // Act
        tutor.UpdateName(novoNome);

        // Assert
        Assert.Equal(novoNome, tutor.Name);
    }

    [Fact]
    public void UpdateEmail_EmailComMaiusculas_DeveNormalizarParaMinusculas()
    {
        // Arrange
        var tutor = new Tutor("Carlos Mendes", "carlos@email.com", "11999999999");

        // Act
        tutor.UpdateEmail("CARLOS.NOVO@EMAIL.COM");

        // Assert
        Assert.Equal("carlos.novo@email.com", tutor.Email);
    }

    [Fact]
    public void UpdatePhone_TelefoneVazio_DeveLancarExcecao()
    {
        // Arrange
        var tutor = new Tutor("Carlos Mendes", "carlos@email.com", "11999999999");

        // Act
        var act = () => tutor.UpdatePhone("");

        // Assert
        Assert.Throws<Exception>(act);
    }
}
