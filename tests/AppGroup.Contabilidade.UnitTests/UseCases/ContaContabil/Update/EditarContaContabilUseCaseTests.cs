using AppGroup.Contabilidade.Application.UseCases.ContaContabil.Update;
using AppGroup.Contabilidade.Domain.Enums;
using AppGroup.Contabilidade.Domain.Interfaces.Repositories;
using AppGroup.Contabilidade.Domain.Models.ContaContabil;
using Moq;

namespace AppGroup.Contabilidade.UnitTests.UseCases.ContaContabil.Update;

public class EditarContaContabilUseCaseTests
{
    private readonly Mock<IContaContabilRepository> _repositoryMock;
    private readonly EditarContaContabilUseCase _useCase;

    public EditarContaContabilUseCaseTests()
    {
        _repositoryMock = new Mock<IContaContabilRepository>();
        _useCase = new EditarContaContabilUseCase(_repositoryMock.Object);
    }

    [Fact]
    public async Task DeveRetornarTrue_QuandoEdicaoForBemSucedida()
    {
        // Arrange
        var request = new EditarContaContabilRequest
        {
            Id = Guid.NewGuid(),
            Codigo = "1.01",
            Nome = "Conta Teste",
            Tipo = TipoConta.Receitas,
            AceitaLancamentos = true
        };

        _repositoryMock
            .Setup(r => r.BuscarContaPorId(It.IsAny<Guid>()))
            .ReturnsAsync(new ContaContabilModel { Id = request.Id, Tipo = TipoConta.Receitas });

        _repositoryMock
            .Setup(r => r.PesquisarFilhosPorId(It.IsAny<Guid>()))
            .ReturnsAsync(new List<(string, bool)>());

        _repositoryMock
            .Setup(r => r.PesquisarContaPorCodigo("1"))
            .ReturnsAsync(new ContaContabilModel { Tipo = TipoConta.Receitas });

        _repositoryMock
            .Setup(r => r.EditarContaContabil(It.IsAny<EditarContaContabilModel>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _useCase.Handle(request, CancellationToken.None);

        // Assert
        Assert.IsType<EditarContaContabilResponse>(result);
        Assert.False(request.HasError);
        Assert.Equal(true, result.Data);
    }

    [Fact]
    public async Task DeveRetornarErro_QuandoContaNaoExiste()
    {
        // Arrange
        var request = new EditarContaContabilRequest
        {
            Id = Guid.NewGuid(),
            Codigo = "1.01",
            Nome = "Conta Teste",
            Tipo = TipoConta.Receitas,
            AceitaLancamentos = true
        };

        _repositoryMock
            .Setup(r => r.BuscarContaPorId(It.IsAny<Guid>()))
            .ReturnsAsync((ContaContabilModel)null!);

        // Act
        var result = await _useCase.Handle(request, CancellationToken.None);

        // Assert
        Assert.True(request.HasError);
        Assert.Equal("Registro de conta não encontrado", result.Data);
    }
}