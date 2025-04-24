using AppGroup.Contabilidade.Application.UseCases.ContaContabil.Get;
using AppGroup.Contabilidade.Application.UseCases.ContaContabil.Get.Handlers;
using AppGroup.Contabilidade.Domain.Interfaces.Repositories;
using AppGroup.Contabilidade.Domain.Models.ContaContabil;
using Moq;

namespace AppGroup.Contabilidade.UnitTests.UseCases.ContaContabil.Get.Handlers;

public class CarregarBaseContabilHandlerTests
{
    private readonly Mock<IContaContabilRepository> _repositoryMock;
    private readonly CarregarBaseContabilHandler _handler;
    private readonly ListarContaContabilRequest _request;

    public CarregarBaseContabilHandlerTests()
    {
        // Configuração inicial para cada teste
        _repositoryMock = new Mock<IContaContabilRepository>();
        _handler = new CarregarBaseContabilHandler(_repositoryMock.Object);
        _request = new ListarContaContabilRequest();
    }

    [Fact]
    public async Task Process_QuandoRepositorioRetornaContas_DevePreencherContasNoRequest()
    {
        // Arrange
        var contasEsperadas = new List<ContaContabilModel>();

        _repositoryMock.Setup(repo => repo.ListarContas()).ReturnsAsync(contasEsperadas);

        // Act
        await _handler.Process(_request);

        // Assert
        Assert.Equal(contasEsperadas, _request.Contas);
        Assert.False(_request.HasError);
        Assert.Empty(_request.ErrorMessage);
    }

    [Fact]
    public async Task Process_QuandoRepositorioLancaExcecao_DeveDefinirErroNoRequest()
    {
        // Arrange
        var mensagemErroEsperada = "Erro ao listar contas contábeis";

        _repositoryMock.Setup(repo => repo.ListarContas()).ThrowsAsync(new Exception(mensagemErroEsperada));

        // Act
        await _handler.Process(_request);

        // Assert
        Assert.True(_request.HasError);
        Assert.Equal(mensagemErroEsperada, _request.ErrorMessage);
        Assert.Null(_request.Contas);
    }
}
