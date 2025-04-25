using AppGroup.Contabilidade.Application.Common.Handlers;
using AppGroup.Contabilidade.Application.UseCases.ContaContabil.Create;
using AppGroup.Contabilidade.Application.UseCases.ContaContabil.Create.Handlers;
using Moq;

namespace AppGroup.Contabilidade.UnitTests.UseCases.ContaContabil.Create.Handlers;

public class ChecaNivelCodigoHandlerTests
{
    private readonly ChecaNivelCodigoHandler _handler;
    private readonly Mock<Handler<CriarContaContabilRequest>> _successorMock;

    public ChecaNivelCodigoHandlerTests()
    {
        _handler = new ChecaNivelCodigoHandler();
        _successorMock = new Mock<Handler<CriarContaContabilRequest>>();
        _handler.SetSuccessor(_successorMock.Object);
    }

    [Fact]
    public async Task Process_DeveSetarNivel1_QuandoIdPaiForNull_ECodigoSemPonto()
    {
        var request = new CriarContaContabilRequest
        {
            Codigo = "1",
            IdPai = null,
            AceitaLancamentos = false
        };

        await _handler.Process(request);

        Assert.Equal(1, request.Nivel);
        Assert.False(request.HasError);
        _successorMock.Verify(s => s.Process(request), Times.Once);
    }

    [Fact]
    public async Task Process_DeveSetarErro_QuandoIdPaiForNull_ECodigoContiverPonto()
    {
        var request = new CriarContaContabilRequest
        {
            Codigo = "1.1",
            IdPai = null,
            AceitaLancamentos = false
        };

        await _handler.Process(request);

        Assert.True(request.HasError);
        Assert.Equal("Para cadastro de Conta-pai não deve informar sub-niveis.", request.ErrorMessage);
        _successorMock.Verify(s => s.Process(request), Times.Once);
    }

    [Fact]
    public async Task Process_DeveSetarErro_QuandoContaPaiAceitaLancamentos()
    {
        var request = new CriarContaContabilRequest
        {
            Codigo = "1",
            IdPai = null,
            AceitaLancamentos = true
        };

        await _handler.Process(request);

        Assert.True(request.HasError);
        Assert.Equal("Conta-pai não deve aceitar lançamentos.", request.ErrorMessage);
        _successorMock.Verify(s => s.Process(request), Times.Once);
    }

    [Theory]
    [InlineData("1.1", 2)]
    [InlineData("1.1.1", 3)]
    [InlineData("1.1.1.1", 4)]
    public async Task Process_DeveCalcularNivelCorretamente_QuandoIdPaiNaoForNull(string codigo, int nivelEsperado)
    {
        var request = new CriarContaContabilRequest
        {
            Codigo = codigo,
            IdPai = Guid.NewGuid()
        };

        await _handler.Process(request);

        Assert.Equal(nivelEsperado, request.Nivel);
        Assert.False(request.HasError);
        _successorMock.Verify(s => s.Process(request), Times.Once);
    }

    [Fact]
    public async Task Process_NaoDeveProcessar_QuandoRequestJaTemErro()
    {
        var request = new CriarContaContabilRequest
        {
            HasError = true,
            Codigo = "1.1"
        };

        await _handler.Process(request);

        _successorMock.Verify(s => s.Process(It.IsAny<CriarContaContabilRequest>()), Times.Never);
    }
}