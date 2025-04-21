using AppGroup.Contabilidade.Domain.Dtos.Http;
using AppGroup.Contabilidade.Domain.Enums;
using MediatR;
using System.Text.Json.Serialization;

namespace AppGroup.Contabilidade.Application.UseCases.ContaContabil.Create;

public class CriarContaContabilRequest : RequestBaseDto, IRequest<CriarContaContabilResponse>
{
    public string Codigo { get; set; } = string.Empty;
    public string Nome { get; set; } = string.Empty;
    public TipoConta Tipo { get; set; }
    public bool AceitaLancamentos { get; set; }
    public Guid? IdPai { get; set; }

    [JsonIgnore]
    public int Nivel { get; set; }
}
