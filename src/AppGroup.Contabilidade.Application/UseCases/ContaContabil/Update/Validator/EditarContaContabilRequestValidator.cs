using FluentValidation;

namespace AppGroup.Contabilidade.Application.UseCases.ContaContabil.Update.Validator;

public class EditarContaContabilRequestValidator : AbstractValidator<EditarContaContabilRequest>
{
    public EditarContaContabilRequestValidator()
    {
        RuleFor(c => c.Id)
            .Must(id => id != Guid.Empty).WithMessage("O ID da conta contábil não pode ser um GUID vazio.");

        RuleFor(c => c.Codigo)
            .NotEmpty().WithMessage("O código da conta contábil deve ser informado.");

        RuleFor(c => c.Nome)
            .NotEmpty().WithMessage("O nome da conta contábil deve ser informado.");

        RuleFor(c => c.AceitaLancamentos)
            .NotNull().WithMessage("O campo AceitaLancamentos deve ser informado.");
    }
}