using AppGroup.Contabilidade.Application.UseCases.ContaContabil.Create;
using AppGroup.Contabilidade.Application.UseCases.ContaContabil.Create.Handlers;
using AppGroup.Contabilidade.Domain.Enums;
using AppGroup.Contabilidade.Domain.Interfaces.Repositories;
using AppGroup.Contabilidade.Domain.Models.ContaContabil;
using Moq;

namespace AppGroup.Contabilidade.UnitTests.UseCases.ContaContabil.Create.Handlers;

public class GravaDadosContaHandlerTests
{
    private readonly Mock<IContaContabilRepository> _repositoryMock;
    private readonly GravaDadosContaHandler _handler;

    public GravaDadosContaHandlerTests()
    {
        _repositoryMock = new Mock<IContaContabilRepository>();
        _handler = new GravaDadosContaHandler(_repositoryMock.Object);
    }

    [Fact]
    public async Task Process_DeveGravarConta_QuandoRequestValido()
    {
        // Arrange
        var request = new CriarContaContabilRequest
        {
            Codigo = "1.1",
            Nome = "Receita de Vendas",
            Tipo = TipoConta.Receitas,
            AceitaLancamentos = true,
            IdPai = Guid.NewGuid()
        };

        // Act
        await _handler.Process(request);

        // Assert
        _repositoryMock.Verify(repo => repo.CriarContaContabil(It.Is<CriarContaContabilModel>(
            c => c.Codigo == request.Codigo &&
                 c.Nome == request.Nome &&
                 c.Tipo == 1 &&
                 c.AceitaLancamentos == 1 &&
                 c.IdPai == request.IdPai
        )), Times.Once);

        Assert.False(request.HasError);
        Assert.Empty(request.ErrorMessage);
    }

    [Fact]
    public async Task Process_DeveSetarErro_QuandoLancamentoLancaExcecao()
    {
        // Arrange
        var request = new CriarContaContabilRequest
        {
            Codigo = "1.1",
            Nome = "Conta com Erro",
            Tipo = TipoConta.Despesas,
            AceitaLancamentos = false
        };

        _repositoryMock
            .Setup(repo => repo.CriarContaContabil(It.IsAny<CriarContaContabilModel>()))
            .ThrowsAsync(new Exception("Erro ao gravar"));

        // Act
        await _handler.Process(request);

        // Assert
        Assert.True(request.HasError);
        Assert.Equal("Erro ao gravar", request.ErrorMessage);
    }
}