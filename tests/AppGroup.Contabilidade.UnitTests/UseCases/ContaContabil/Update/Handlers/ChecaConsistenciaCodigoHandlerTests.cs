using AppGroup.Contabilidade.Application.Common.Handlers;
using AppGroup.Contabilidade.Application.UseCases.ContaContabil.Update;
using AppGroup.Contabilidade.Application.UseCases.ContaContabil.Update.Handlers;
using AppGroup.Contabilidade.Domain.Enums;
using AppGroup.Contabilidade.Domain.Interfaces.Repositories;
using AppGroup.Contabilidade.Domain.Models.ContaContabil;
using Moq;

namespace AppGroup.Contabilidade.UnitTests.UseCases.ContaContabil.Update.Handlers;

public class ChecaConsistenciaCodigoHandlerTests
{
    private readonly Mock<IContaContabilRepository> _repositoryMock;
    private readonly ChecaConsistenciaCodigoHandler _handler;
    private readonly Mock<Handler<EditarContaContabilRequest>> _successorMock;

    public ChecaConsistenciaCodigoHandlerTests()
    {
        _repositoryMock = new Mock<IContaContabilRepository>();
        _handler = new ChecaConsistenciaCodigoHandler(_repositoryMock.Object);

        _successorMock = new Mock<Handler<EditarContaContabilRequest>>();
        _handler.SetSuccessor(_successorMock.Object);
    }

    [Fact]
    public async Task DeveRetornarErro_SeNivel1Ou2_TipoAlteradoComFilhos()
    {
        // Arrange
        var request = new EditarContaContabilRequest
        {
            Id = Guid.NewGuid(),
            Nivel = 1,
            Tipo = TipoConta.Despesas
        };

        _repositoryMock
            .Setup(r => r.BuscarContaPorId(request.Id))
            .ReturnsAsync(new ContaContabilModel { Id = request.Id, Tipo = TipoConta.Receitas });

        _repositoryMock
            .Setup(r => r.PesquisarFilhosPorId(request.Id))
            .ReturnsAsync([new("string", true)]);

        // Act
        await _handler.Process(request);

        // Assert
        Assert.True(request.HasError);
        Assert.Equal("Tipo não pode ser alterado, porque há registros filhos cadastrados na base", request.ErrorMessage);

        _successorMock.Verify(s => s.Process(It.IsAny<EditarContaContabilRequest>()), Times.Once);
    }

    [Fact]
    public async Task DeveRetornarErro_SeNivel2_TipoDiferenteDoPai()
    {
        // Arrange
        var request = new EditarContaContabilRequest
        {
            Id = Guid.NewGuid(),
            Nivel = 2,
            Codigo = "1.456",
            Tipo = TipoConta.Receitas
        };

        _repositoryMock
            .Setup(r => r.BuscarContaPorId(request.Id))
            .ReturnsAsync(new ContaContabilModel { Id = request.Id, Tipo = TipoConta.Receitas });

        _repositoryMock
            .Setup(r => r.PesquisarFilhosPorId(request.Id))
            .ReturnsAsync([new("string", true)]);

        _repositoryMock
            .Setup(r => r.PesquisarContaPorCodigo("1"))
            .ReturnsAsync(new ContaContabilModel { Tipo = TipoConta.Despesas });

        // Act
        await _handler.Process(request);

        // Assert
        Assert.True(request.HasError);
        Assert.Equal("Tipo não pode ser alterado porque está diferente da Conta-pai", request.ErrorMessage);
    }

    [Fact]
    public async Task DeveRetornarErro_SeNivel3_EAceitaLancementosFalse()
    {
        var request = new EditarContaContabilRequest
        {
            Nivel = 3,
            AceitaLancementos = false
        };

        // Act
        await _handler.Process(request);

        // Assert
        Assert.True(request.HasError);
        Assert.Equal("Tipo não pode ser alterado, porque tem registros filhos cadastrados", request.ErrorMessage);
    }
}
