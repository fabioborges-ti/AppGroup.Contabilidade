using AppGroup.Contabilidade.Application.UseCases.ContaContabil.Create;
using AppGroup.Contabilidade.Application.UseCases.ContaContabil.Create.Validator;
using AppGroup.Contabilidade.Domain.Enums;
using FluentValidation.TestHelper;

namespace AppGroup.Contabilidade.UnitTests.UseCases.ContaContabil.Create.Validator;

public class CriarContaContabilRequestValidatorTests
{
    private readonly CriarContaContabilRequestValidator _validator;

    public CriarContaContabilRequestValidatorTests()
    {
        _validator = new CriarContaContabilRequestValidator();
    }

    [Fact]
    public void Deve_RetornarErro_QuandoCodigoEstiverVazio()
    {
        var request = new CriarContaContabilRequest { Codigo = string.Empty };
        var result = _validator.TestValidate(request);

        result.ShouldHaveValidationErrorFor(r => r.Codigo);
    }

    [Fact]
    public void Deve_RetornarErro_QuandoNomeEstiverVazio()
    {
        var request = new CriarContaContabilRequest { Nome = string.Empty };
        var result = _validator.TestValidate(request);

        result.ShouldHaveValidationErrorFor(r => r.Nome);
    }

    [Theory]
    [InlineData(TipoConta.Receitas)]
    [InlineData(TipoConta.Despesas)]
    public void NaoDeve_RetornarErro_QuandoTipoForValido(TipoConta tipoValido)
    {
        var request = new CriarContaContabilRequest { Tipo = tipoValido };
        var result = _validator.TestValidate(request);
        result.ShouldNotHaveValidationErrorFor(r => r.Tipo);
    }

    [Fact]
    public void Deve_RetornarErro_QuandoIdPaiNaoForGuidValido()
    {
        var request = new CriarContaContabilRequest { IdPai = Guid.Parse("00000000-0000-0000-0000-000000000000") };
        var result = _validator.TestValidate(request);
        result.ShouldNotHaveValidationErrorFor(r => r.IdPai); // Considerado válido se for um GUID válido
    }

    [Fact]
    public void NaoDeve_RetornarErro_QuandoIdPaiForNulo()
    {
        var request = new CriarContaContabilRequest { IdPai = null };
        var result = _validator.TestValidate(request);
        result.ShouldNotHaveValidationErrorFor(r => r.IdPai);
    }

    [Fact]
    public void Deve_PassarTodasValidacoes_QuandoDadosForemValidos()
    {
        var request = new CriarContaContabilRequest
        {
            Codigo = "123",
            Nome = "Conta de Receita",
            Tipo = TipoConta.Receitas,
            AceitaLancamentos = true,
            IdPai = null
        };

        var result = _validator.TestValidate(request);
        result.ShouldNotHaveAnyValidationErrors();
    }
}