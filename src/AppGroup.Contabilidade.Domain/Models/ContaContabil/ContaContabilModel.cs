using AppGroup.Contabilidade.Domain.Enums;

namespace AppGroup.Contabilidade.Domain.Models.ContaContabil;

public class ContaContabilModel
{
    public Guid Id { get; set; }
    public string Codigo { get; set; }
    public string Nome { get; set; }
    public TipoConta Tipo { get; set; }
    public bool AceitaLancamentos { get; set; }
}
