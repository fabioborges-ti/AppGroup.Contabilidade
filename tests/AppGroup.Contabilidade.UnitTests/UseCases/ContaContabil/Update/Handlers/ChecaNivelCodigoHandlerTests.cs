using AppGroup.Contabilidade.Application.Common.Handlers;
using AppGroup.Contabilidade.Application.UseCases.ContaContabil.Update;
using AppGroup.Contabilidade.Application.UseCases.ContaContabil.Update.Handlers;

namespace AppGroup.Contabilidade.UnitTests.UseCases.ContaContabil.Update.Handlers;

public class ChecaNivelCodigoHandlerTests
{
    private readonly ChecaNivelCodigoHandler _handler;

    public ChecaNivelCodigoHandlerTests()
    {
        _handler = new ChecaNivelCodigoHandler();
    }

    [Theory]
    [InlineData("1", 1)]
    [InlineData("1.01", 2)]
    [InlineData("1.01.03", 3)]
    [InlineData("1.01.03.05", 4)]
    public async Task Deve_Definir_Corretamente_Nivel_ComBaseNoCodigo(string codigo, int nivelEsperado)
    {
        var request = new EditarContaContabilRequest { Codigo = codigo };

        await _handler.Process(request);

        Assert.False(request.HasError);
        Assert.Equal(nivelEsperado, request.Nivel);
    }

    [Fact]
    public async Task Deve_DefinirNivelComo1_QuandoCodigoNaoTemPonto()
    {
        var request = new EditarContaContabilRequest { Codigo = "10" };

        await _handler.Process(request);

        Assert.Equal(1, request.Nivel);
        Assert.False(request.HasError);
    }

    [Fact]
    public async Task Deve_DefinirErro_QuandoCodigoForNuloOuVazio()
    {
        var request = new EditarContaContabilRequest { Codigo = "" };

        await _handler.Process(request);

        Assert.True(request.HasError);
        Assert.Equal("Codigo deve ser informado", request.ErrorMessage);
    }

    [Fact]
    public async Task Deve_Permitir_Encadeamento_QuandoSuccessorForDefinido()
    {
        var chamado = false;
        var mockHandler = new TestHandler(req =>
        {
            chamado = true;
            return Task.CompletedTask;
        });

        _handler.SetSuccessor(mockHandler);

        var request = new EditarContaContabilRequest { Codigo = "1.01" };

        await _handler.Process(request);

        Assert.True(chamado);
    }

    private class TestHandler : Handler<EditarContaContabilRequest>
    {
        private readonly Func<EditarContaContabilRequest, Task> _processFunc;

        public TestHandler(Func<EditarContaContabilRequest, Task> processFunc)
        {
            _processFunc = processFunc;
        }

        public override Task Process(EditarContaContabilRequest request)
        {
            return _processFunc(request);
        }
    }
}