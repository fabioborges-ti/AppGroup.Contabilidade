using AppGroup.Contabilidade.Domain.Dtos.Http;
using MediatR;
using System.Text.Json.Serialization;

namespace AppGroup.Contabilidade.Application.UseCases.ContaContabil.Generator;

public class CriarSugestaoRequest : RequestBaseDto, IRequest<CriarSugestaoResponse>
{
    public Guid? IdPai { get; set; }

    [JsonIgnore]
    public string Codigo { get; set; } = string.Empty;
}