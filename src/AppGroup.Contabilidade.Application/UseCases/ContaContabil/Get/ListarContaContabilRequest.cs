using AppGroup.Contabilidade.Domain.Dtos.Http;
using AppGroup.Contabilidade.Domain.Models.ContaContabil;
using MediatR;
using System.Text.Json.Serialization;

namespace AppGroup.Contabilidade.Application.UseCases.ContaContabil.Get;

public class ListarContaContabilRequest : RequestBaseDto, IRequest<ListarContaContabilResponse>
{
    [JsonIgnore]
    public List<ContaContabilModel>? Contas { get; set; }
}
