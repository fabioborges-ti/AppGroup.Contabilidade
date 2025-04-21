using AppGroup.Contabilidade.Domain.Enums;

namespace AppGroup.Contabilidade.Domain.Entities;

public class ContaContabil
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Codigo { get; set; } = string.Empty;
    public string Nome { get; set; } = string.Empty;
    public TipoConta Tipo { get; set; }
    public bool AceitaLancamentos { get; set; }
    public Guid? IdPai { get; set; }

    public ContaContabil? ContaPai { get; set; }
    public ICollection<ContaContabil>? SubContas { get; set; }
}
