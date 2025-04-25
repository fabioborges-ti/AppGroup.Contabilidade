using AppGroup.Contabilidade.Application.Common.Handlers;
using AppGroup.Contabilidade.Application.UseCases.ContaContabil.Create;
using AppGroup.Contabilidade.Application.UseCases.ContaContabil.Create.Handlers;
using AppGroup.Contabilidade.Domain.Enums;
using AppGroup.Contabilidade.Domain.Interfaces.Repositories;
using AppGroup.Contabilidade.Domain.Models.ContaContabil;
using Moq;

namespace AppGroup.Contabilidade.UnitTests.UseCases.ContaContabil.Create.Handlers;

public class ChecaContaPaiHandlerTests
{
    private readonly Mock<IContaContabilRepository> _repositoryMock;
    private readonly ChecaContaPaiHandler _handler;

    public ChecaContaPaiHandlerTests()
    {
        _repositoryMock = new Mock<IContaContabilRepository>();
        _handler = new ChecaContaPaiHandler(_repositoryMock.Object);

        // Sucessor fictício para evitar null reference
        _handler.SetSuccessor(new DummyHandler());
    }

    [Fact]
    public async Task DeveIgnorarProcessamentoSeRequestJaPossuiErro()
    {
        var request = new CriarContaContabilRequest { HasError = true };

        await _handler.Process(request);

        _repositoryMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task DeveAceitarContaNivel1SemVerificarContaPai()
    {
        var request = new CriarContaContabilRequest
        {
            Codigo = "1",
            Nivel = 1,
            Tipo = TipoConta.Receitas
        };

        await _handler.Process(request);

        Assert.False(request.HasError);
    }

    [Fact]
    public async Task DeveValidarContaPaiNivel2_SeTudoValido()
    {
        var request = new CriarContaContabilRequest
        {
            Codigo = "1.1",
            Nivel = 2,
            Tipo = TipoConta.Receitas
        };

        _repositoryMock.Setup(r => r.ExisteCodigo("1")).ReturnsAsync(true);
        _repositoryMock.Setup(r => r.PesquisarContaPorCodigo("1"))
            .ReturnsAsync(new ContaContabilModel
            {
                Codigo = "1",
                Tipo = TipoConta.Receitas,
                AceitaLancamentos = false
            });

        await _handler.Process(request);

        Assert.False(request.HasError);
    }

    [Fact]
    public async Task DeveRetornarErro_SeContaPaiNaoExistir()
    {
        var request = new CriarContaContabilRequest
        {
            Codigo = "1.1",
            Nivel = 2,
            Tipo = TipoConta.Receitas
        };

        _repositoryMock.Setup(r => r.ExisteCodigo("1")).ReturnsAsync(false);

        await _handler.Process(request);

        Assert.True(request.HasError);
        Assert.Equal("Conta-pai não localizada", request.ErrorMessage);
    }

    [Fact]
    public async Task DeveRetornarErro_SeTipoDiferenteDoPai()
    {
        var request = new CriarContaContabilRequest
        {
            Codigo = "1.1",
            Nivel = 2,
            Tipo = TipoConta.Receitas
        };

        _repositoryMock.Setup(r => r.ExisteCodigo("1")).ReturnsAsync(true);
        _repositoryMock.Setup(r => r.PesquisarContaPorCodigo("1"))
            .ReturnsAsync(new ContaContabilModel
            {
                Codigo = "1",
                Tipo = TipoConta.Despesas,
                AceitaLancamentos = false
            });

        await _handler.Process(request);

        Assert.True(request.HasError);
        Assert.Equal("Tipo da conta deve ser igual ao da Conta-pai", request.ErrorMessage);
    }

    [Fact]
    public async Task DeveRetornarErro_SeContaPaiAceitaLancamento()
    {
        var request = new CriarContaContabilRequest
        {
            Codigo = "1.1",
            Nivel = 2,
            Tipo = TipoConta.Receitas
        };

        _repositoryMock.Setup(r => r.ExisteCodigo("1")).ReturnsAsync(true);
        _repositoryMock.Setup(r => r.PesquisarContaPorCodigo("1"))
            .ReturnsAsync(new ContaContabilModel
            {
                Codigo = "1",
                Tipo = TipoConta.Receitas,
                AceitaLancamentos = true
            });

        await _handler.Process(request);

        Assert.True(request.HasError);
        Assert.Equal("Conta-pai informada não aceita cadastro de contas-filhas", request.ErrorMessage);
    }

    [Fact]
    public async Task DeveRetornarErro_SeNivelForInvalido()
    {
        var request = new CriarContaContabilRequest
        {
            Codigo = "1.2.3.4",
            Nivel = 4,
            Tipo = TipoConta.Receitas
        };

        await _handler.Process(request);

        Assert.True(request.HasError);
        Assert.Equal("Formato inválido.", request.ErrorMessage);
    }

    // Handler Dummy para encadear chamadas
    private class DummyHandler : Handler<CriarContaContabilRequest>
    {
        public override Task Process(CriarContaContabilRequest request) => Task.CompletedTask;
    }
}