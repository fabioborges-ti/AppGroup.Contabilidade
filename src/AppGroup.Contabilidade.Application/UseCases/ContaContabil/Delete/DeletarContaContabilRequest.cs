using AppGroup.Contabilidade.Domain.Dtos.Http;
using MediatR;

namespace AppGroup.Contabilidade.Application.UseCases.ContaContabil.Delete;

public class DeletarContaContabilRequest : RequestBaseDto, IRequest<DeletarContaContabilResponse>
{
    public Guid Id { get; set; }
}
