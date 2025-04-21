using AppGroup.Contabilidade.Domain.Dtos.Http;
using AppGroup.Contabilidade.Domain.Enums;
using MediatR;
using System.Text.Json.Serialization;

namespace AppGroup.Contabilidade.Application.UseCases.ContaContabil.Update;

public class EditarContaContabilRequest : RequestBaseDto, IRequest<EditarContaContabilResponse>
{
    public Guid Id { get; set; }
    public string Codigo { get; set; } = string.Empty;
    public string Nome { get; set; } = string.Empty;
    public TipoConta Tipo { get; set; }
    public bool AceitaLancementos { get; set; }

    [JsonIgnore]
    public int Nivel { get; set; }
}
