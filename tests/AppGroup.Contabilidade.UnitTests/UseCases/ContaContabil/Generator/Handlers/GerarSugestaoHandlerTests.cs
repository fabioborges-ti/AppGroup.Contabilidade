using AppGroup.Contabilidade.Application.UseCases.ContaContabil.Generator;
using AppGroup.Contabilidade.Application.UseCases.ContaContabil.Generator.Handlers;
using AppGroup.Contabilidade.Domain.Interfaces.Repositories;
using Moq;

namespace AppGroup.Contabilidade.UnitTests.UseCases.ContaContabil.Generator.Handlers;

public class GerarSugestaoHandlerTests
{
    private readonly Mock<IContaContabilRepository> _repositoryMock;
    private readonly GerarSugestaoHandler _handler;
    private readonly CriarSugestaoRequest _request;

    public GerarSugestaoHandlerTests()
    {
        _repositoryMock = new Mock<IContaContabilRepository>();
        _handler = new GerarSugestaoHandler(_repositoryMock.Object);
        _request = new CriarSugestaoRequest();
    }

    [Fact]
    public async Task Process_WhenIdPaiIsNull_ShouldGenerateParentCode()
    {
        // Arrange
        _request.IdPai = null;
        _repositoryMock.Setup(r => r.GerarCodigoPai()).ReturnsAsync(1);

        // Act
        await _handler.Process(_request);

        // Assert
        Assert.Equal("1", _request.Codigo);
        Assert.False(_request.HasError);
        Assert.Empty(_request.ErrorMessage);

        _repositoryMock.Verify(r => r.GerarCodigoPai(), Times.Once);
    }

    [Fact]
    public async Task Process_WithValidIdPai_ShouldGenerateChildCode()
    {
        // Arrange
        var parentId = Guid.NewGuid();
        _request.IdPai = parentId;

        _repositoryMock
            .Setup(r => r.PesquisarPaiPorId(parentId))
            .ReturnsAsync(("1", false)); // Parent code, doesn't accept entries

        _repositoryMock
            .Setup(r => r.PesquisarFilhosPorId(parentId))
            .ReturnsAsync(
            [
                    ("1.1", true),
                    ("1.2", true)
            ]);

        // Act
        await _handler.Process(_request);

        // Assert
        Assert.Equal("1.3", _request.Codigo);
        Assert.False(_request.HasError);
        Assert.Empty(_request.ErrorMessage);

        _repositoryMock.Verify(r => r.PesquisarPaiPorId(parentId), Times.Once);
        _repositoryMock.Verify(r => r.PesquisarFilhosPorId(parentId), Times.Once);
    }

    [Fact]
    public async Task Process_WithNoExistingChildren_ShouldGenerateFirstChildCode()
    {
        // Arrange
        var parentId = Guid.NewGuid();
        _request.IdPai = parentId;

        _repositoryMock
            .Setup(r => r.PesquisarPaiPorId(parentId))
            .ReturnsAsync(("1", false)); // Parent code, doesn't accept entries

        _repositoryMock
            .Setup(r => r.PesquisarFilhosPorId(parentId))
            .ReturnsAsync([]);

        // Act
        await _handler.Process(_request);

        // Assert
        Assert.Equal("1.1", _request.Codigo);
        Assert.False(_request.HasError);
        Assert.Empty(_request.ErrorMessage);
    }

    [Fact]
    public async Task Process_WithNonexistentParentId_ShouldSetErrorMessage()
    {
        // Arrange
        var parentId = Guid.NewGuid();
        _request.IdPai = parentId;

        _repositoryMock
            .Setup(r => r.PesquisarPaiPorId(parentId))
            .ReturnsAsync(("", false)); // Empty parent code indicates not found

        // Act
        await _handler.Process(_request);

        // Assert
        Assert.True(_request.HasError);
        Assert.Equal("Código-pai não encontrado", _request.ErrorMessage);
    }

    [Fact]
    public async Task Process_WithParentThatAcceptsEntries_ShouldSetErrorMessage()
    {
        // Arrange
        var parentId = Guid.NewGuid();

        _request.IdPai = parentId;

        _repositoryMock
            .Setup(r => r.PesquisarPaiPorId(parentId))
            .ReturnsAsync(("1", true)); // Parent code that accepts entries

        // Act
        await _handler.Process(_request);

        // Assert
        Assert.True(_request.HasError);
        Assert.Equal("Código-pai não permite criação de filhos", _request.ErrorMessage);
    }

    [Fact]
    public async Task Process_WithLevel3CodeReachingLimit_ShouldFindNextAvailableParentCode()
    {
        // Arrange
        var parentId = Guid.NewGuid();
        _request.IdPai = parentId;

        _repositoryMock
            .Setup(r => r.PesquisarPaiPorId(parentId))
            .ReturnsAsync(("1.2", false));

        var childCodes = new List<(string, bool)>();

        for (int i = 1; i < 1000; i++)
        {
            childCodes.Add(($"1.2.{i}", true));
        }

        _repositoryMock
            .Setup(r => r.PesquisarFilhosPorId(parentId))
            .ReturnsAsync(childCodes);

        _repositoryMock
            .Setup(r => r.ExisteCodigo("1.3"))
            .ReturnsAsync(true);

        _repositoryMock
            .Setup(r => r.ExisteCodigo("1.4"))
            .ReturnsAsync(false);

        // Act
        await _handler.Process(_request);

        // Assert
        Assert.Equal("1.4", _request.Codigo);
        Assert.False(_request.HasError);

        _repositoryMock.Verify(r => r.ExisteCodigo("1.3"), Times.Once);
        _repositoryMock.Verify(r => r.ExisteCodigo("1.4"), Times.Once);
    }

    [Fact]
    public async Task Process_WithGenericException_ShouldHandleError()
    {
        // Arrange
        _request.IdPai = null;
        _repositoryMock.Setup(r => r.GerarCodigoPai())
            .ThrowsAsync(new Exception("Database connection error"));

        // Act
        await _handler.Process(_request);

        // Assert
        Assert.True(_request.HasError);
        Assert.Equal("Database connection error", _request.ErrorMessage);
    }

    [Fact]
    public async Task Process_WithChildCodesAtLevel2_ShouldGenerateCorrectNextCode()
    {
        // Arrange
        var parentId = Guid.NewGuid();
        _request.IdPai = parentId;

        _repositoryMock
            .Setup(r => r.PesquisarPaiPorId(parentId))
            .ReturnsAsync(("1", false));

        _repositoryMock
            .Setup(r => r.PesquisarFilhosPorId(parentId))
            .ReturnsAsync(
            [
                ("1.5", true),
                ("1.7", true)
            ]);

        // Act
        await _handler.Process(_request);

        // Assert
        Assert.Equal("1.8", _request.Codigo);
    }

    [Fact]
    public async Task Process_WithChildCodesAtLevel3_ShouldGenerateCorrectNextCode()
    {
        // Arrange
        var parentId = Guid.NewGuid();
        _request.IdPai = parentId;

        _repositoryMock
            .Setup(r => r.PesquisarPaiPorId(parentId))
            .ReturnsAsync(("1.2", false));

        _repositoryMock
            .Setup(r => r.PesquisarFilhosPorId(parentId))
            .ReturnsAsync(
            [
                ("1.2.5", true),
                ("1.2.8", true)
            ]);

        // Act
        await _handler.Process(_request);

        // Assert
        Assert.Equal("1.2.9", _request.Codigo);
    }

    [Fact]
    public async Task Process_WithMixedLevelChildCodes_ShouldConsiderOnlyCorrectLevel()
    {
        // Arrange
        var parentId = Guid.NewGuid();

        _request.IdPai = parentId;

        _repositoryMock
            .Setup(r => r.PesquisarPaiPorId(parentId))
            .ReturnsAsync(("1.2", false));

        // Mix of level 2 and level 3 codes
        _repositoryMock
            .Setup(r => r.PesquisarFilhosPorId(parentId))
            .ReturnsAsync(
            [
                ("1.2.1", true),
                ("1.2.2", true),
                ("1.2.3", true),
                ("1.2.4", true),
            ]);

        // Act
        await _handler.Process(_request);

        // Assert
        Assert.Equal("1.2.5", _request.Codigo);
    }
}