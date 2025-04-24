using AppGroup.Contabilidade.Application.UseCases.ContaContabil.Update;
using AppGroup.Contabilidade.Application.UseCases.ContaContabil.Update.Handlers;
using AppGroup.Contabilidade.Domain.Interfaces.Repositories;
using AppGroup.Contabilidade.Domain.Models.ContaContabil;
using Moq;

namespace AppGroup.Contabilidade.UnitTests.UseCases.ContaContabil.Update.Handlers;

public class ChecaExistenciaCodigoHandlerTests
{
    private readonly Mock<IContaContabilRepository> _repositoryMock;
    private readonly ChecaExistenciaCodigoHandler _handler;

    public ChecaExistenciaCodigoHandlerTests()
    {
        _repositoryMock = new Mock<IContaContabilRepository>();
        _handler = new ChecaExistenciaCodigoHandler(_repositoryMock.Object);
    }

    [Fact]
    public async Task DeveSetarErro_SeContaNaoForEncontrada()
    {
        // Arrange
        var request = new EditarContaContabilRequest
        {
            Id = Guid.NewGuid()
        };

        _repositoryMock
            .Setup(r => r.BuscarContaPorId(request.Id))
            .ReturnsAsync((ContaContabilModel)null!);

        // Act
        await _handler.Process(request);

        // Assert
        Assert.True(request.HasError);
        Assert.Equal("Registro de conta não encontrado", request.ErrorMessage);
    }

    [Fact]
    public async Task NaoDeveSetarErro_SeContaForEncontrada()
    {
        // Arrange
        var request = new EditarContaContabilRequest
        {
            Id = Guid.NewGuid()
        };

        _repositoryMock
            .Setup(r => r.BuscarContaPorId(request.Id))
            .ReturnsAsync(new ContaContabilModel { Id = request.Id });

        // Act
        await _handler.Process(request);

        // Assert
        Assert.False(request.HasError);
        Assert.Empty(request.ErrorMessage);
    }

    [Fact]
    public async Task NaoDeveExecutar_SeRequestJaTemErro()
    {
        // Arrange
        var request = new EditarContaContabilRequest
        {
            HasError = true
        };

        // Act
        await _handler.Process(request);

        // Assert
        _repositoryMock.Verify(r => r.BuscarContaPorId(It.IsAny<Guid>()), Times.Never);
    }
}
