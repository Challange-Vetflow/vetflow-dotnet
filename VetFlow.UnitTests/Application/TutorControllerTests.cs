using Microsoft.AspNetCore.Mvc;
using Moq;
using VetFlow.API.Controllers;
using VetFlow.Application.DTOs;
using VetFlow.Application.Repositories;
using VetFlow.Domain.Entities;
using Xunit;

namespace VetFlow.UnitTests.Application;

public class TutorControllerTests
{
    private readonly Mock<ITutorRepository> _repositoryMock;
    private readonly TutorController _controller;

    public TutorControllerTests()
    {
        _repositoryMock = new Mock<ITutorRepository>();
        _controller = new TutorController(_repositoryMock.Object);
    }

    [Fact]
    public void GetById_TutorExistente_DeveRetornarOkComTutor()
    {
        // Arrange
        var tutor = new Tutor("Carlos Mendes", "carlos@email.com", "11999999999");
        _repositoryMock.Setup(r => r.GetById(tutor.Id)).Returns(tutor);

        // Act
        var resultado = _controller.GetById(tutor.Id);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(resultado);
        var response = Assert.IsType<TutorResponse>(okResult.Value);
        Assert.Equal(tutor.Name, response.Name);
    }

    [Fact]
    public void GetById_TutorInexistente_DeveRetornarNotFound()
    {
        // Arrange
        var idInexistente = Guid.NewGuid();
        _repositoryMock.Setup(r => r.GetById(idInexistente)).Returns((Tutor?)null);

        // Act
        var resultado = _controller.GetById(idInexistente);

        // Assert
        Assert.IsType<NotFoundResult>(resultado);
    }

    [Fact]
    public void Create_EmailJaCadastrado_DeveRetornarBadRequest()
    {
        // Arrange
        var request = new TutorRequest("Carlos Mendes", "carlos@email.com", "11999999999");
        _repositoryMock.Setup(r => r.ExistsByEmail(request.Email)).Returns(true);

        // Act
        var resultado = _controller.Create(request);

        // Assert
        Assert.IsType<BadRequestObjectResult>(resultado);
        _repositoryMock.Verify(r => r.Add(It.IsAny<Tutor>()), Times.Never);
    }

    [Fact]
    public void Create_DadosValidos_DeveRetornarCreatedComTutor()
    {
        // Arrange
        var request = new TutorRequest("Carlos Mendes", "carlos@email.com", "11999999999");
        _repositoryMock.Setup(r => r.ExistsByEmail(request.Email)).Returns(false);
        _repositoryMock.Setup(r => r.Add(It.IsAny<Tutor>())).Returns<Tutor>(t => t);

        // Act
        var resultado = _controller.Create(request);

        // Assert
        var createdResult = Assert.IsType<CreatedAtActionResult>(resultado);
        var response = Assert.IsType<TutorResponse>(createdResult.Value);
        Assert.Equal(request.Name, response.Name);
        _repositoryMock.Verify(r => r.Add(It.IsAny<Tutor>()), Times.Once);
    }

    [Fact]
    public void Delete_TutorExistente_DeveRetornarNoContent()
    {
        // Arrange
        var id = Guid.NewGuid();
        _repositoryMock.Setup(r => r.Delete(id)).Returns(true);

        // Act
        var resultado = _controller.Delete(id);

        // Assert
        Assert.IsType<NoContentResult>(resultado);
    }

    [Fact]
    public void Delete_TutorInexistente_DeveRetornarNotFound()
    {
        // Arrange
        var id = Guid.NewGuid();
        _repositoryMock.Setup(r => r.Delete(id)).Returns(false);

        // Act
        var resultado = _controller.Delete(id);

        // Assert
        Assert.IsType<NotFoundResult>(resultado);
    }
}
