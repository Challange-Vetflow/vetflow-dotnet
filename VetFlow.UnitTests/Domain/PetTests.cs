using VetFlow.Domain.Entities;
using VetFlow.Domain.Enums;
using Xunit;

namespace VetFlow.UnitTests.Domain;

public class PetTests
{
    private static readonly Guid TutorIdValido = Guid.NewGuid();

    [Fact]
    public void Criar_DadosValidos_DeveCriarPetComSucesso()
    {
        // Arrange
        var name = "Rex";
        var species = SpeciesEnum.Dog;
        var breed = "Labrador";
        var birthDate = new DateOnly(2020, 3, 15);
        var weightKg = 28.5;

        // Act
        var pet = new Pet(name, species, breed, birthDate, weightKg, TutorIdValido);

        // Assert
        Assert.Equal(name, pet.Name);
        Assert.Equal(species, pet.Species);
        Assert.Equal(breed, pet.Breed);
        Assert.Equal(weightKg, pet.WeightKg);
        Assert.Equal(TutorIdValido, pet.TutorId);
    }

    [Fact]
    public void Criar_PesoZeroOuNegativo_DeveLancarExcecao()
    {
        // Arrange
        var birthDate = new DateOnly(2020, 3, 15);

        // Act
        var act = () => new Pet("Rex", SpeciesEnum.Dog, "Labrador", birthDate, 0, TutorIdValido);

        // Assert
        Assert.Throws<Exception>(act);
    }

    [Fact]
    public void Criar_DataNascimentoFutura_DeveLancarExcecao()
    {
        // Arrange
        var dataFutura = DateOnly.FromDateTime(DateTime.Today.AddDays(10));

        // Act
        var act = () => new Pet("Rex", SpeciesEnum.Dog, "Labrador", dataFutura, 28.5, TutorIdValido);

        // Assert
        Assert.Throws<Exception>(act);
    }

    [Fact]
    public void Criar_RacaVazia_DeveDefinirComoIndefinida()
    {
        // Arrange
        var birthDate = new DateOnly(2020, 3, 15);

        // Act
        var pet = new Pet("Rex", SpeciesEnum.Dog, "", birthDate, 28.5, TutorIdValido);

        // Assert
        Assert.Equal("Indefinida", pet.Breed);
    }

    [Fact]
    public void AgeInMonths_PetComUmAnoDeVida_DeveRetornarDozeMeses()
    {
        // Arrange
        var birthDate = DateOnly.FromDateTime(DateTime.Today.AddYears(-1));
        var pet = new Pet("Rex", SpeciesEnum.Dog, "Labrador", birthDate, 28.5, TutorIdValido);

        // Act
        var idade = pet.AgeInMonths;

        // Assert
        Assert.Equal(12, idade);
    }

    [Fact]
    public void Update_DadosValidos_DeveAtualizarCamposDoPet()
    {
        // Arrange
        var birthDate = new DateOnly(2020, 3, 15);
        var pet = new Pet("Rex", SpeciesEnum.Dog, "Labrador", birthDate, 28.5, TutorIdValido);

        // Act
        pet.Update("Rex Atualizado", SpeciesEnum.Dog, "Golden Retriever", birthDate, 32.0);

        // Assert
        Assert.Equal("Rex Atualizado", pet.Name);
        Assert.Equal("Golden Retriever", pet.Breed);
        Assert.Equal(32.0, pet.WeightKg);
    }
}
