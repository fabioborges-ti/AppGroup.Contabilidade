using AppGroup.Contabilidade.Application.UseCases.ContaContabil.Get;
using AppGroup.Contabilidade.Domain.Interfaces.Repositories;
using AppGroup.Contabilidade.Domain.Models.ContaContabil;
using Moq;

namespace AppGroup.Contabilidade.UnitTests.UseCases.ContaContabil.Get;

public class ListarContaContabilUseCaseTests
{
    private readonly Mock<IContaContabilRepository> _repositoryMock;
    private readonly ListarContaContabilUseCase _useCase;
    private readonly CancellationToken _cancellationToken;

    public ListarContaContabilUseCaseTests()
    {
        _repositoryMock = new Mock<IContaContabilRepository>();
        _useCase = new ListarContaContabilUseCase(_repositoryMock.Object);
        _cancellationToken = CancellationToken.None;
    }

    [Fact]
    public async Task Handle_QuandoProcessoExecutadoComSucesso_DeveRetornarListaDeContasNaResposta()
    {
        // Arrange
        var request = new ListarContaContabilRequest();
        var contasEsperadas = new List<ContaContabilModel>();

        _repositoryMock.Setup(repo => repo.ListarContas()).ReturnsAsync(contasEsperadas);

        // Act
        var response = await _useCase.Handle(request, _cancellationToken);

        // Assert
        Assert.NotNull(response);
        Assert.Equal(contasEsperadas, response.Data);
        Assert.False(request.HasError);
    }

    [Fact]
    public async Task Handle_QuandoProcessoOcorreErro_DeveRetornarMensagemDeErroNaResposta()
    {
        // Arrange
        var request = new ListarContaContabilRequest();
        var mensagemErroEsperada = "Erro ao listar contas contábeis";

        _repositoryMock.Setup(repo => repo.ListarContas()).ThrowsAsync(new Exception(mensagemErroEsperada));

        // Act
        var response = await _useCase.Handle(request, _cancellationToken);

        // Assert
        Assert.NotNull(response);
        Assert.Equal(mensagemErroEsperada, response.Data);
        Assert.True(request.HasError);
        Assert.Equal(mensagemErroEsperada, request.ErrorMessage);
    }

    [Fact]
    public async Task Handle_DeveUtilizarCarregarBaseContabilHandler_ParaProcessarRequest()
    {
        // Arrange
        var request = new ListarContaContabilRequest();
        var contas = new List<ContaContabilModel>();

        _repositoryMock.Setup(repo => repo.ListarContas()).ReturnsAsync(contas);

        // Act
        var response = await _useCase.Handle(request, _cancellationToken);

        // Assert
        _repositoryMock.Verify(repo => repo.ListarContas(), Times.Once);

        Assert.Equal(contas, request.Contas);
    }
}