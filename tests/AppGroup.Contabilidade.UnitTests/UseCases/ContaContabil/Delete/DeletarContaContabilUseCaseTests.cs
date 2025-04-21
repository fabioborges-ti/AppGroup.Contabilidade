using AppGroup.Contabilidade.Application.UseCases.ContaContabil.Delete;
using AppGroup.Contabilidade.Domain.Interfaces.Repositories;
using AppGroup.Contabilidade.Domain.Models.ContaContabil;
using Moq;

namespace AppGroup.Contabilidade.Application.Tests.UseCases.ContaContabil.Delete;

public class DeletarContaContabilUseCaseTests
{
    private readonly Mock<IContaContabilRepository> _repositoryMock;
    private readonly DeletarContaContabilUseCase _useCase;
    private readonly CancellationToken _cancellationToken;

    public DeletarContaContabilUseCaseTests()
    {
        _repositoryMock = new Mock<IContaContabilRepository>();
        _useCase = new DeletarContaContabilUseCase(_repositoryMock.Object);
        _cancellationToken = CancellationToken.None;
    }

    [Fact]
    public async Task Handle_QuandoOperacaoSucesso_DeveRetornarTrue()
    {
        // Arrange
        var idConta = Guid.NewGuid();

        var request = new DeletarContaContabilRequest { Id = idConta };
        var contaContabil = new ContaContabilModel { Id = idConta };

        _repositoryMock
            .Setup(r => r.BuscarContaPorId(request.Id))
            .ReturnsAsync(contaContabil);

        _repositoryMock
            .Setup(r => r.PesquisarFilhosPorId(request.Id))
            .ReturnsAsync([]);

        // Act
        var response = await _useCase.Handle(request, _cancellationToken);

        // Assert
        Assert.True(response.Data is bool);
        Assert.True((bool)response.Data);

        _repositoryMock.Verify(r => r.DeletarContaContabil(request.Id), Times.Once);
    }

    [Fact]
    public async Task Handle_QuandoContaNaoEncontrada_DeveRetornarMensagemErro()
    {
        // Arrange
        var request = new DeletarContaContabilRequest { Id = Guid.NewGuid() };

        _repositoryMock
            .Setup(r => r.BuscarContaPorId(request.Id))
            .ReturnsAsync((ContaContabilModel)null!);

        // Act
        var response = await _useCase.Handle(request, _cancellationToken);

        // Assert
        Assert.True(response.Data is string);
        Assert.Equal("Conta contábil não encontrada", response.Data);

        _repositoryMock.Verify(r => r.DeletarContaContabil(request.Id), Times.Never);
    }

    [Fact]
    public async Task Handle_QuandoContaPossuiFilhos_DeveRetornarMensagemErro()
    {
        // Arrange
        var idConta = Guid.NewGuid();
        var request = new DeletarContaContabilRequest { Id = idConta };
        var contaContabil = new ContaContabilModel { Id = idConta };
        var contasFilhas = new List<(string, bool)>
        {
            new ("string", true)
        };

        _repositoryMock
            .Setup(r => r.BuscarContaPorId(request.Id))
            .ReturnsAsync(contaContabil);

        _repositoryMock
            .Setup(r => r.PesquisarFilhosPorId(request.Id))
            .ReturnsAsync(contasFilhas);

        // Act
        var response = await _useCase.Handle(request, _cancellationToken);

        // Assert
        Assert.True(response.Data is string);
        Assert.Equal("Não é possível excluir uma conta que possui contas filhas associadas", response.Data);

        _repositoryMock.Verify(r => r.DeletarContaContabil(request.Id), Times.Never);
    }

    [Fact]
    public async Task Handle_QuandoOcorreErroNaExclusao_DeveRetornarMensagemErro()
    {
        // Arrange
        var idConta = Guid.NewGuid();
        var request = new DeletarContaContabilRequest { Id = idConta };
        var contaContabil = new ContaContabilModel { Id = idConta };
        var mensagemErro = "Erro ao excluir conta contábil";

        _repositoryMock
            .Setup(r => r.BuscarContaPorId(request.Id))
            .ReturnsAsync(contaContabil);

        _repositoryMock
            .Setup(r => r.PesquisarFilhosPorId(request.Id))
            .ReturnsAsync(new List<(string, bool)>());

        _repositoryMock
            .Setup(r => r.DeletarContaContabil(request.Id))
            .ThrowsAsync(new Exception(mensagemErro));

        // Act
        var response = await _useCase.Handle(request, _cancellationToken);

        // Assert
        Assert.True(response.Data is string);
        Assert.Equal(mensagemErro, response.Data);
    }

    [Fact]
    public async Task Handle_DeveUsarCorretamenteCadeiaDeHandlers()
    {
        // Este teste verifica se a cadeia de handlers está sendo configurada corretamente
        // É mais um teste de integração do que unitário, mas é importante verificar o fluxo

        // Arrange
        var idConta = Guid.NewGuid();
        var request = new DeletarContaContabilRequest { Id = idConta };
        var contaContabil = new ContaContabilModel { Id = idConta };

        _repositoryMock
            .Setup(r => r.BuscarContaPorId(request.Id))
            .ReturnsAsync(contaContabil);

        _repositoryMock
            .Setup(r => r.PesquisarFilhosPorId(request.Id))
            .ReturnsAsync(new List<(string, bool)>());

        // Act
        var response = await _useCase.Handle(request, _cancellationToken);

        // Assert
        Assert.True(response.Data is bool);
        Assert.True((bool)response.Data);

        // Verifica se ambos os handlers foram chamados na ordem correta
        _repositoryMock.Verify(r => r.BuscarContaPorId(request.Id), Times.Once);
        _repositoryMock.Verify(r => r.PesquisarFilhosPorId(request.Id), Times.Once);
        _repositoryMock.Verify(r => r.DeletarContaContabil(request.Id), Times.Once);

        // A verificação da ordem é implícita pelo fato de que o segundo handler
        // só é chamado se o primeiro handler não encontrar erros
    }
}
