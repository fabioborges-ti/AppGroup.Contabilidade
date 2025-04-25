using AppGroup.Contabilidade.Domain.Enums;
using FluentValidation;

namespace AppGroup.Contabilidade.Application.UseCases.ContaContabil.Create.Validator;

public class CriarContaContabilRequestValidator : AbstractValidator<CriarContaContabilRequest>
{
    public CriarContaContabilRequestValidator()
    {
        RuleFor(c => c.Codigo)
            .NotEmpty().WithMessage("O código da conta contábil deve ser informado.");

        RuleFor(c => c.Nome)
            .NotEmpty().WithMessage("O nome da conta contábil deve ser informado.");

        RuleFor(c => c.Tipo)
            .IsInEnum().WithMessage("O tipo da conta contábil deve ser válido.")
            .Must(tipo => tipo == TipoConta.Receitas || tipo == TipoConta.Despesas)
            .WithMessage("O tipo da conta contábil deve ser 1 (Receita) ou 2 (Despesa).");

        RuleFor(c => c.AceitaLancamentos)
            .NotNull().WithMessage("O campo AceitaLancamentos deve ser informado.");

        RuleFor(c => c.IdPai)
            .Must(idPai => idPai == null || Guid.TryParse(idPai.ToString(), out _))
            .WithMessage("O IdPai deve ser nulo ou um GUID válido.");
    }
}
