using AppGroup.Contabilidade.Application.UseCases.ContaContabil.Generator;
using AppGroup.Contabilidade.Application.UseCases.ContaContabil.Generator.Handlers;
using AppGroup.Contabilidade.Domain.Interfaces.Repositories;
using Moq;

public class GerarSugestaoHandlerTests
{
    private readonly Mock<IContaContabilRepository> _repositoryMock;
    private readonly GerarSugestaoHandler _handler;

    public GerarSugestaoHandlerTests()
    {
        _repositoryMock = new Mock<IContaContabilRepository>();
        _handler = new GerarSugestaoHandler(_repositoryMock.Object);
    }

    [Fact]
    public async Task Process_DeveGerarCodigoPai_QuandoIdPaiForNulo()
    {
        // Arrange
        var request = new CriarSugestaoRequest { IdPai = null };
        _repositoryMock.Setup(r => r.GerarCodigoPai()).ReturnsAsync(10);

        // Act
        await _handler.Process(request);

        // Assert
        Assert.Equal("10", request.Codigo);
        Assert.False(request.HasError);
        Assert.Empty(request.ErrorMessage);
    }

    [Fact]
    public async Task Process_DeveGerarCodigoFilho_QuandoIdPaiNaoForNulo_EPrimeiroFilho()
    {
        // Arrange
        var idPai = Guid.NewGuid();
        var request = new CriarSugestaoRequest { IdPai = idPai };
        _repositoryMock.Setup(r => r.PesquisarPaiPorId(idPai)).ReturnsAsync(("1", false));
        _repositoryMock.Setup(r => r.PesquisarFilhosPorId(idPai)).ReturnsAsync(new List<(string, bool)>());

        // Act
        await _handler.Process(request);

        // Assert
        Assert.Equal("1.1", request.Codigo);
        Assert.False(request.HasError);
    }

    [Fact]
    public async Task Process_DeveGerarCodigoFilhoComNivel2()
    {
        var idPai = Guid.NewGuid();
        var filhos = new List<(string, bool)>
        {
            ("1.1", true),
            ("1.2", true)
        };

        var request = new CriarSugestaoRequest { IdPai = idPai };
        _repositoryMock.Setup(r => r.PesquisarPaiPorId(idPai)).ReturnsAsync(("1", false));
        _repositoryMock.Setup(r => r.PesquisarFilhosPorId(idPai)).ReturnsAsync(filhos);

        await _handler.Process(request);

        Assert.Equal("1.3", request.Codigo);
    }

    [Fact]
    public async Task Process_DeveGerarCodigoFilhoComNivel3()
    {
        var idPai = Guid.NewGuid();
        var filhos = new List<(string, bool)>
        {
            ("1.1.998", true),
            ("1.1.999", true)
        };

        var request = new CriarSugestaoRequest { IdPai = idPai };
        _repositoryMock.Setup(r => r.PesquisarPaiPorId(idPai)).ReturnsAsync(("1.1", false));
        _repositoryMock.Setup(r => r.PesquisarFilhosPorId(idPai)).ReturnsAsync(filhos);
        _repositoryMock.Setup(r => r.ExisteCodigo("1.2")).ReturnsAsync(false);

        await _handler.Process(request);

        Assert.Equal("1.2", request.Codigo);
    }

    [Fact]
    public async Task Process_DeveRetornarProximoCodigoPaiQuandoNivel3AtingeLimite()
    {
        var idPai = Guid.NewGuid();
        var filhos = new List<(string, bool)>
        {
            ("1.999.999", true)
        };

        var request = new CriarSugestaoRequest { IdPai = idPai };

        _repositoryMock.Setup(r => r.PesquisarPaiPorId(idPai)).ReturnsAsync(("1.999", false));
        _repositoryMock.Setup(r => r.PesquisarFilhosPorId(idPai)).ReturnsAsync(filhos);
        _repositoryMock.Setup(r => r.ExisteCodigo("1.1000")).ReturnsAsync(false);

        await _handler.Process(request);

        Assert.Equal("1.1000", request.Codigo);
    }

    [Fact]
    public async Task Process_DeveSetarErro_QuandoCodigoPaiNaoForEncontrado()
    {
        var idPai = Guid.NewGuid();
        var request = new CriarSugestaoRequest { IdPai = idPai };

        _repositoryMock.Setup(r => r.PesquisarPaiPorId(idPai))!.ReturnsAsync((null, false));

        await _handler.Process(request);

        Assert.True(request.HasError);
        Assert.Equal("Código-pai não encontrado", request.ErrorMessage);
    }

    [Fact]
    public async Task Process_DeveSetarErro_QuandoCodigoPaiNaoPermiteFilhos()
    {
        var idPai = Guid.NewGuid();
        var request = new CriarSugestaoRequest { IdPai = idPai };

        _repositoryMock.Setup(r => r.PesquisarPaiPorId(idPai)).ReturnsAsync(("1", true));

        await _handler.Process(request);

        Assert.True(request.HasError);
        Assert.Equal("Código-pai não permite criação de filhos", request.ErrorMessage);
    }
}
