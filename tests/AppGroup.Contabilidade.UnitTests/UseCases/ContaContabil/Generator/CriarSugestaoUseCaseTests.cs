using AppGroup.Contabilidade.Application.UseCases.ContaContabil.Generator;
using AppGroup.Contabilidade.Domain.Interfaces.Repositories;
using Moq;

public class CriarSugestaoUseCaseTests
{
    private readonly Mock<IContaContabilRepository> _repositoryMock;
    private readonly CriarSugestaoUseCase _useCase;

    public CriarSugestaoUseCaseTests()
    {
        _repositoryMock = new Mock<IContaContabilRepository>();
        _useCase = new CriarSugestaoUseCase(_repositoryMock.Object);
    }

    [Fact]
    public async Task Handle_DeveGerarCodigoPai_QuandoIdPaiForNull()
    {
        // Arrange
        var request = new CriarSugestaoRequest
        {
            IdPai = null
        };

        _repositoryMock.Setup(r => r.GerarCodigoPai()).ReturnsAsync(99);

        // Act
        var response = await _useCase.Handle(request, default);

        // Assert
        Assert.False(request.HasError);
        Assert.Equal("99", response.Data);
    }

    [Fact]
    public async Task Handle_DeveGerarCodigoFilho_QuandoIdPaiNaoForNull()
    {
        // Arrange
        var idPai = Guid.NewGuid();
        var request = new CriarSugestaoRequest
        {
            IdPai = idPai
        };

        _repositoryMock.Setup(r => r.PesquisarPaiPorId(idPai)).ReturnsAsync(("1", false));
        _repositoryMock.Setup(r => r.PesquisarFilhosPorId(idPai)).ReturnsAsync(new List<(string, bool)>());

        // Act
        var response = await _useCase.Handle(request, default);

        // Assert
        Assert.False(request.HasError);
        Assert.Equal("1.1", response.Data);
    }

    [Fact]
    public async Task Handle_DeveRetornarErro_QuandoCodigoPaiInvalido()
    {
        // Arrange
        var idPai = Guid.NewGuid();
        var request = new CriarSugestaoRequest
        {
            IdPai = idPai
        };

        _repositoryMock.Setup(r => r.PesquisarPaiPorId(idPai))!.ReturnsAsync((null, false));

        // Act
        var response = await _useCase.Handle(request, default);

        // Assert
        Assert.True(request.HasError);
        Assert.Equal("Código-pai não encontrado", response.Data);
    }
}
