using AppGroup.Contabilidade.Application.UseCases.ContaContabil.GetById;
using AppGroup.Contabilidade.Domain.Interfaces.Repositories;
using AppGroup.Contabilidade.Domain.Models.ContaContabil;
using Moq;

namespace AppGroup.Contabilidade.UnitTests.UseCases.ContaContabil.GetById;

public class ListarPorIdContaContabilUseCaseTests
{
    private readonly Mock<IContaContabilRepository> _repositoryMock;
    private readonly ListarPorIdContaContabilUseCase _useCase;
    private readonly CancellationToken _cancellationToken;

    public ListarPorIdContaContabilUseCaseTests()
    {
        _repositoryMock = new Mock<IContaContabilRepository>();
        _useCase = new ListarPorIdContaContabilUseCase(_repositoryMock.Object);
        _cancellationToken = CancellationToken.None;
    }

    [Fact]
    public async Task Handle_QuandoContaExiste_DeveRetornarContaNaResposta()
    {
        // Arrange
        var idConta = Guid.NewGuid();
        var contaEsperada = new ContaContabilModel();

        var request = new ListarPorIdContaContabilRequest { IdConta = idConta };

        _repositoryMock
            .Setup(repo => repo.BuscarContaPorId(idConta))
            .ReturnsAsync(contaEsperada);

        // Act
        var response = await _useCase.Handle(request, _cancellationToken);

        // Assert
        Assert.NotNull(response);
        Assert.Equal(contaEsperada, response.Data);
    }

    [Fact]
    public async Task Handle_QuandoOcorreErro_DeveRetornarMensagemDeErroNaResposta()
    {
        // Arrange
        var idConta = Guid.NewGuid();
        var mensagemErroEsperada = "Erro ao buscar conta contábil";

        var request = new ListarPorIdContaContabilRequest { IdConta = idConta };

        _repositoryMock
            .Setup(repo => repo.BuscarContaPorId(idConta))
            .ThrowsAsync(new Exception(mensagemErroEsperada));

        // Act
        var response = await _useCase.Handle(request, _cancellationToken);

        // Assert
        Assert.NotNull(response);
        Assert.Equal(mensagemErroEsperada, response.Data);
        Assert.True(request.HasError);
    }

    [Fact]
    public async Task Handle_QuandoContaNaoExiste_DeveProcessarCorretamente()
    {
        // Arrange
        var idConta = Guid.NewGuid(); // ID inexistente
        object contaNula = new ContaContabilModel();

        var request = new ListarPorIdContaContabilRequest { IdConta = idConta };

        _repositoryMock
            .Setup(repo => repo.BuscarContaPorId(idConta))
            .ReturnsAsync((ContaContabilModel)null!);

        // Act
        var response = await _useCase.Handle(request, _cancellationToken);

        // Assert
        Assert.NotNull(response);
        Assert.Null(response.Data);
        Assert.False(request.HasError);
    }

    [Fact]
    public async Task Handle_DeveUtilizarCarregarBaseContabilHandler_ParaProcessarRequest()
    {
        // Arrange
        var idConta = Guid.NewGuid();
        var contaEsperada = new ContaContabilModel(); // Substitua pelo tipo correto

        var request = new ListarPorIdContaContabilRequest { IdConta = idConta };

        _repositoryMock
            .Setup(repo => repo.BuscarContaPorId(idConta))
            .ReturnsAsync(contaEsperada);

        // Act
        var response = await _useCase.Handle(request, _cancellationToken);

        // Assert
        _repositoryMock.Verify(repo => repo.BuscarContaPorId(idConta), Times.Once);

        Assert.Equal(contaEsperada, request.Conta);
    }
}