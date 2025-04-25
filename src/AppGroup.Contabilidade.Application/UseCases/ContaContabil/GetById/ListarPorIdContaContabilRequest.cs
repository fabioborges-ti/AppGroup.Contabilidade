using AppGroup.Contabilidade.Domain.Dtos.Http;
using AppGroup.Contabilidade.Domain.Models.ContaContabil;
using MediatR;
using System.Text.Json.Serialization;

namespace AppGroup.Contabilidade.Application.UseCases.ContaContabil.GetById;

public class ListarPorIdContaContabilRequest : RequestBaseDto, IRequest<ListarPorIdContaContabilResponse>
{
    public Guid IdConta { get; set; }

    [JsonIgnore]
    public ContaContabilModel? Conta { get; set; }
}