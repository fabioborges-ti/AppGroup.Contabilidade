using AppGroup.Contabilidade.Application.Common.Handlers;
using AppGroup.Contabilidade.Application.UseCases.ContaContabil.Create;
using AppGroup.Contabilidade.Application.UseCases.ContaContabil.Create.Handlers;

namespace AppGroup.Contabilidade.UnitTests.UseCases.ContaContabil.Create.Handlers;

public class ChecaConsistenciaNivelHandlerTests
{
    private readonly ChecaConsistenciaNivelHandler _handler;

    public ChecaConsistenciaNivelHandlerTests()
    {
        _handler = new ChecaConsistenciaNivelHandler();
    }

    [Fact]
    public async Task Deve_RetornarErro_QuandoNivelFor3_EAceitaLancamentosForFalse()
    {
        var request = new CriarContaContabilRequest
        {
            Nivel = 3,
            AceitaLancamentos = false
        };

        await _handler.Process(request);

        Assert.True(request.HasError);
        Assert.Equal("Este nível de conta-contábil deve receber lançamentos", request.ErrorMessage);
    }

    [Theory]
    [InlineData(1, true)]
    [InlineData(2, false)]
    [InlineData(3, true)]
    [InlineData(4, false)]
    public async Task NaoDeve_RetornarErro_ParaOutrosCenarios(int nivel, bool aceitaLancamentos)
    {
        var request = new CriarContaContabilRequest
        {
            Nivel = nivel,
            AceitaLancamentos = aceitaLancamentos
        };

        await _handler.Process(request);

        Assert.False(request.HasError);
        Assert.Empty(request.ErrorMessage);
    }

    [Fact]
    public async Task Deve_ExecutarSuccessor_QuandoNaoHouverErro()
    {
        var chamado = false;

        var mockSuccessor = new TestHandler(req =>
        {
            chamado = true;
            return Task.CompletedTask;
        });

        _handler.SetSuccessor(mockSuccessor);

        var request = new CriarContaContabilRequest
        {
            Nivel = 2,
            AceitaLancamentos = true
        };

        await _handler.Process(request);

        Assert.True(chamado);
    }

    private class TestHandler : Handler<CriarContaContabilRequest>
    {
        private readonly Func<CriarContaContabilRequest, Task> _processFunc;

        public TestHandler(Func<CriarContaContabilRequest, Task> processFunc)
        {
            _processFunc = processFunc;
        }

        public override Task Process(CriarContaContabilRequest request)
        {
            return _processFunc(request);
        }
    }
}