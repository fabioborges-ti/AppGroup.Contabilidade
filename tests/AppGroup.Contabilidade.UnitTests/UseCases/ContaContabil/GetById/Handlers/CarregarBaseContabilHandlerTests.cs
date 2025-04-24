using AppGroup.Contabilidade.Application.UseCases.ContaContabil.GetById;
using AppGroup.Contabilidade.Application.UseCases.ContaContabil.GetById.Handlers;
using AppGroup.Contabilidade.Domain.Interfaces.Repositories;
using AppGroup.Contabilidade.Domain.Models.ContaContabil;
using Moq;

namespace AppGroup.Contabilidade.UnitTests.UseCases.ContaContabil.GetById.Handlers;

public class CarregarBaseContabilHandlerTests
{
    private readonly Mock<IContaContabilRepository> _repositoryMock;
    private readonly CarregarBaseContabilHandler _handler;

    public CarregarBaseContabilHandlerTests()
    {
        // Configuração inicial para cada teste
        _repositoryMock = new Mock<IContaContabilRepository>();
        _handler = new CarregarBaseContabilHandler(_repositoryMock.Object);
    }

    [Fact]
    public async Task Process_QuandoContaExiste_DevePreencherContaNoRequest()
    {
        // Arrange
        var idConta = Guid.NewGuid();
        var contaEsperada = new ContaContabilModel();

        var request = new ListarPorIdContaContabilRequest { IdConta = idConta };

        _repositoryMock
            .Setup(repo => repo.BuscarContaPorId(idConta))
            .ReturnsAsync(contaEsperada);

        // Act
        await _handler.Process(request);

        // Assert
        Assert.Equal(contaEsperada, request.Conta);
        Assert.False(request.HasError);
        Assert.Empty(request.ErrorMessage);
    }

    [Fact]
    public async Task Process_QuandoRepositorioLancaExcecao_DeveDefinirErroNoRequest()
    {
        // Arrange
        var idConta = Guid.NewGuid();
        var mensagemErroEsperada = "Erro ao buscar conta contábil";

        var request = new ListarPorIdContaContabilRequest { IdConta = idConta };

        _repositoryMock.Setup(repo => repo.BuscarContaPorId(idConta)).ThrowsAsync(new Exception(mensagemErroEsperada));

        // Act
        await _handler.Process(request);

        // Assert
        Assert.True(request.HasError);
        Assert.Equal(mensagemErroEsperada, request.ErrorMessage);
        Assert.Null(request.Conta);
    }

    [Fact]
    public async Task Process_DevePassarIdCorretoParaRepositorio()
    {
        // Arrange
        var idConta = Guid.NewGuid();
        var contaEsperada = new ContaContabilModel();

        var request = new ListarPorIdContaContabilRequest { IdConta = idConta };

        _repositoryMock
            .Setup(repo => repo.BuscarContaPorId(It.IsAny<Guid>()))
            .ReturnsAsync(contaEsperada);

        // Act
        await _handler.Process(request);

        // Assert
        _repositoryMock.Verify(repo => repo.BuscarContaPorId(idConta), Times.Once);
    }
}
