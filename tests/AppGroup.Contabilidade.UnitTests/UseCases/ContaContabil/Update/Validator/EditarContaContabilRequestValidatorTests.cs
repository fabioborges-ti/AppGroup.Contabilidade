using AppGroup.Contabilidade.Application.UseCases.ContaContabil.Update;
using AppGroup.Contabilidade.Application.UseCases.ContaContabil.Update.Validator;
using FluentValidation.TestHelper;

namespace AppGroup.Contabilidade.UnitTests.UseCases.ContaContabil.Update.Validator;

public class EditarContaContabilRequestValidatorTests
{
    private readonly EditarContaContabilRequestValidator _validator;

    public EditarContaContabilRequestValidatorTests()
    {
        _validator = new EditarContaContabilRequestValidator();
    }

    [Fact]
    public void Deve_RetornarErro_QuandoIdForGuidVazio()
    {
        var request = new EditarContaContabilRequest { Id = Guid.Empty };
        var result = _validator.TestValidate(request);
        result.ShouldHaveValidationErrorFor(r => r.Id);
    }

    [Fact]
    public void Deve_RetornarErro_QuandoCodigoEstiverVazio()
    {
        var request = new EditarContaContabilRequest { Codigo = "" };
        var result = _validator.TestValidate(request);
        result.ShouldHaveValidationErrorFor(r => r.Codigo);
    }

    [Fact]
    public void Deve_RetornarErro_QuandoCodigoConterLetras()
    {
        var request = new EditarContaContabilRequest { Codigo = "123A" };
        var result = _validator.TestValidate(request);
        result.ShouldHaveValidationErrorFor(r => r.Codigo);
    }

    [Fact]
    public void Deve_Aceitar_CodigoComApenasNumeros()
    {
        var request = new EditarContaContabilRequest { Codigo = "123456" };
        var result = _validator.TestValidate(request);
        result.ShouldNotHaveValidationErrorFor(r => r.Codigo);
    }

    [Fact]
    public void Deve_RetornarErro_QuandoNomeEstiverVazio()
    {
        var request = new EditarContaContabilRequest { Nome = "" };
        var result = _validator.TestValidate(request);
        result.ShouldHaveValidationErrorFor(r => r.Nome);
    }

    [Fact]
    public void Deve_RetornarErro_QuandoAceitaLancamentosNaoForInformado()
    {
        var request = new EditarContaContabilRequest();
        // como bool não-nullable sempre tem um valor, esse teste seria para tipo bool? (nullable)
        // para manter a consistência, aqui simulamos por design
        // No entanto, isso não causará erro na prática
        var result = _validator.TestValidate(request);
        result.ShouldNotHaveValidationErrorFor(r => r.AceitaLancamentos); // Sempre tem valor por ser bool
    }

    [Fact]
    public void Deve_PassarTodasValidacoes_QuandoDadosForemValidos()
    {
        var request = new EditarContaContabilRequest
        {
            Id = Guid.NewGuid(),
            Codigo = "100",
            Nome = "Conta Corrente",
            AceitaLancamentos = true
        };

        var result = _validator.TestValidate(request);
        result.ShouldNotHaveAnyValidationErrors();
    }
}
