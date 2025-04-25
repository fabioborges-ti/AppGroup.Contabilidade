using AppGroup.Contabilidade.Application.Common.Handlers;
using AppGroup.Contabilidade.Application.UseCases.ContaContabil.Delete;
using AppGroup.Contabilidade.Application.UseCases.ContaContabil.Delete.Handlers;
using AppGroup.Contabilidade.Domain.Interfaces.Repositories;
using AppGroup.Contabilidade.Domain.Models.ContaContabil;
using Moq;

namespace AppGroup.Contabilidade.UnitTests.UseCases.ContaContabil.Delete.Handlers;

public class ChecaDadosContaHandlerTests
{
    private readonly Mock<IContaContabilRepository> _repositoryMock;
    private readonly ChecaDadosContaHandler _handler;
    private readonly Mock<Handler<DeletarContaContabilRequest>> _successorMock;

    public ChecaDadosContaHandlerTests()
    {
        _repositoryMock = new Mock<IContaContabilRepository>();
        _handler = new ChecaDadosContaHandler(_repositoryMock.Object);
        _successorMock = new Mock<Handler<DeletarContaContabilRequest>>();
        _handler.SetSuccessor(_successorMock.Object);
    }

    [Fact]
    public async Task Process_ContaNaoEncontrada_DeveDefinirErro()
    {
        // Arrange
        var request = new DeletarContaContabilRequest { Id = Guid.NewGuid() };

        _repositoryMock
            .Setup(r => r.BuscarContaPorId(request.Id))
            .ReturnsAsync((ContaContabilModel)null!);

        // Act
        await _handler.Process(request);

        // Assert
        Assert.True(request.HasError);
        Assert.Equal("Conta contábil não encontrada", request.ErrorMessage);

        _successorMock.Verify(s => s.Process(request), Times.Once);
    }

    [Fact]
    public async Task Process_ContaPossuiFilhos_DeveDefinirErro()
    {
        // Arrange
        var idConta = Guid.NewGuid();

        var request = new DeletarContaContabilRequest { Id = idConta };
        var conta = new ContaContabilModel { Id = idConta };
        var contasFilhas = new List<(string, bool)> { new("string", true) };

        _repositoryMock
            .Setup(r => r.BuscarContaPorId(request.Id))
            .ReturnsAsync(conta);

        _repositoryMock
            .Setup(r => r.PesquisarFilhosPorId(conta.Id))
            .ReturnsAsync(contasFilhas);

        // Act
        await _handler.Process(request);

        // Assert
        Assert.True(request.HasError);
        Assert.Equal("Não é possível excluir uma conta que possui contas filhas associadas", request.ErrorMessage);

        _successorMock.Verify(s => s.Process(request), Times.Once);
    }

    [Fact]
    public async Task Process_ContaValidaSemFilhos_DeveContinuarProcessamento()
    {
        // Arrange
        var idConta = Guid.NewGuid();

        var request = new DeletarContaContabilRequest { Id = idConta };
        var conta = new ContaContabilModel { Id = idConta };
        var contasFilhasVazias = new List<(string, bool)>();

        _repositoryMock
            .Setup(r => r.BuscarContaPorId(request.Id))
            .ReturnsAsync(conta);

        _repositoryMock
            .Setup(r => r.PesquisarFilhosPorId(conta.Id))
            .ReturnsAsync(contasFilhasVazias);

        // Act
        await _handler.Process(request);

        // Assert
        Assert.False(request.HasError);
        Assert.Empty(request.ErrorMessage);

        _successorMock.Verify(s => s.Process(request), Times.Once);
    }

    [Fact]
    public async Task Process_ErroGenerico_DeveDefinirMensagemDeErro()
    {
        // Arrange
        var request = new DeletarContaContabilRequest { Id = Guid.NewGuid() };
        var exceptionMessage = "Erro no banco de dados";

        _repositoryMock
            .Setup(r => r.BuscarContaPorId(request.Id))
            .ThrowsAsync(new Exception(exceptionMessage));

        // Act
        await _handler.Process(request);

        // Assert
        Assert.True(request.HasError);
        Assert.Equal(exceptionMessage, request.ErrorMessage);

        _successorMock.Verify(s => s.Process(request), Times.Once);
    }
}