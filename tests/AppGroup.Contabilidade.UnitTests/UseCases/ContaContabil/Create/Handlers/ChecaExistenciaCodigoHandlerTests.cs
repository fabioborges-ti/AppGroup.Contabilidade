using AppGroup.Contabilidade.Application.Common.Handlers;
using AppGroup.Contabilidade.Application.UseCases.ContaContabil.Create;
using AppGroup.Contabilidade.Application.UseCases.ContaContabil.Create.Handlers;
using AppGroup.Contabilidade.Domain.Interfaces.Repositories;
using Moq;

namespace AppGroup.Contabilidade.UnitTests.UseCases.ContaContabil.Create.Handlers;

public class ChecaExistenciaCodigoHandlerTests
{
    private readonly Mock<IContaContabilRepository> _repositoryMock;
    private readonly ChecaExistenciaCodigoHandler _handler;
    private readonly Mock<Handler<CriarContaContabilRequest>> _successorMock;

    public ChecaExistenciaCodigoHandlerTests()
    {
        _repositoryMock = new Mock<IContaContabilRepository>();
        _handler = new ChecaExistenciaCodigoHandler(_repositoryMock.Object);

        _successorMock = new Mock<Handler<CriarContaContabilRequest>>();
        _handler.SetSuccessor(_successorMock.Object);
    }

    [Fact]
    public async Task Process_DeveSetarErro_QuandoCodigoJaExiste()
    {
        // Arrange
        var request = new CriarContaContabilRequest { Codigo = "1.1" };
        _repositoryMock.Setup(r => r.ExisteCodigo("1.1")).ReturnsAsync(true);

        // Act
        await _handler.Process(request);

        // Assert
        Assert.True(request.HasError);
        Assert.Equal("Código já cadastrado", request.ErrorMessage);
        _successorMock.Verify(s => s.Process(It.IsAny<CriarContaContabilRequest>()), Times.Once);
    }

    [Fact]
    public async Task Process_NaoDeveSetarErro_QuandoCodigoNaoExiste()
    {
        // Arrange
        var request = new CriarContaContabilRequest { Codigo = "1.1" };
        _repositoryMock.Setup(r => r.ExisteCodigo("1.1")).ReturnsAsync(false);

        // Act
        await _handler.Process(request);

        // Assert
        Assert.False(request.HasError);
        Assert.Empty(request.ErrorMessage);
        _successorMock.Verify(s => s.Process(It.IsAny<CriarContaContabilRequest>()), Times.Once);
    }

    [Fact]
    public async Task Process_DeveSetarErro_QuandoOcorreExcecao()
    {
        // Arrange
        var request = new CriarContaContabilRequest { Codigo = "erro" };
        _repositoryMock.Setup(r => r.ExisteCodigo("erro")).ThrowsAsync(new Exception("Falha ao acessar repositório"));

        // Act
        await _handler.Process(request);

        // Assert
        Assert.True(request.HasError);
        Assert.Equal("Falha ao acessar repositório", request.ErrorMessage);
        _successorMock.Verify(s => s.Process(It.IsAny<CriarContaContabilRequest>()), Times.Once);
    }
}