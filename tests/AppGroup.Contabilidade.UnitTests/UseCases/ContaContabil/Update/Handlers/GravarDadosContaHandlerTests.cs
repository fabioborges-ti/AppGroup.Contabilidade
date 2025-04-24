using AppGroup.Contabilidade.Application.UseCases.ContaContabil.Update;
using AppGroup.Contabilidade.Application.UseCases.ContaContabil.Update.Handlers;
using AppGroup.Contabilidade.Domain.Enums;
using AppGroup.Contabilidade.Domain.Interfaces.Repositories;
using AppGroup.Contabilidade.Domain.Models.ContaContabil;
using Moq;

namespace AppGroup.Contabilidade.UnitTests.UseCases.ContaContabil.Update.Handlers;

public class GravarDadosContaHandlerTests
{
    private readonly Mock<IContaContabilRepository> _repositoryMock;
    private readonly GravarDadosContaHandler _handler;

    public GravarDadosContaHandlerTests()
    {
        _repositoryMock = new Mock<IContaContabilRepository>();
        _handler = new GravarDadosContaHandler(_repositoryMock.Object);
    }

    [Fact]
    public async Task DeveGravarConta_QuandoRequestValida()
    {
        // Arrange
        var request = new EditarContaContabilRequest
        {
            Id = Guid.NewGuid(),
            Codigo = "123.456",
            Nome = "Conta Teste",
            Tipo = TipoConta.Receitas,
            AceitaLancementos = true
        };

        _repositoryMock
            .Setup(r => r.EditarContaContabil(It.IsAny<EditarContaContabilModel>()))
            .Returns(Task.CompletedTask);

        // Act
        await _handler.Process(request);

        // Assert
        _repositoryMock.Verify(r => r.EditarContaContabil(It.Is<EditarContaContabilModel>(c =>
            c.Id == request.Id &&
            c.Codigo == request.Codigo &&
            c.Nome == request.Nome &&
            c.Tipo == (int)request.Tipo &&
            c.AceitaLancamentos == 1
        )), Times.Once);

        Assert.False(request.HasError);
    }

    [Fact]
    public async Task NaoDeveGravar_SeJaTemErro()
    {
        // Arrange
        var request = new EditarContaContabilRequest
        {
            HasError = true
        };

        // Act
        await _handler.Process(request);

        // Assert
        _repositoryMock.Verify(r => r.EditarContaContabil(It.IsAny<EditarContaContabilModel>()), Times.Never);
    }

    [Fact]
    public async Task DeveSetarErro_SeLancarExcecao()
    {
        // Arrange
        var request = new EditarContaContabilRequest
        {
            Id = Guid.NewGuid(),
            Codigo = "123.456",
            Nome = "Conta Teste",
            Tipo = TipoConta.Receitas,
            AceitaLancementos = false
        };

        _repositoryMock
            .Setup(r => r.EditarContaContabil(It.IsAny<EditarContaContabilModel>()))
            .ThrowsAsync(new Exception("Erro ao gravar"));

        // Act
        await _handler.Process(request);

        // Assert
        Assert.True(request.HasError);
        Assert.Equal("Erro ao gravar", request.ErrorMessage);
    }
}
