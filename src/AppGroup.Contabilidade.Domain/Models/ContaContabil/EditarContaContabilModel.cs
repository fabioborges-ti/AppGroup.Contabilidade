namespace AppGroup.Contabilidade.Domain.Models.ContaContabil;

public class EditarContaContabilModel
{
    public Guid Id { get; set; }
    public string Codigo { get; set; } = string.Empty;
    public string Nome { get; set; } = string.Empty;
    public int Tipo { get; set; }
    public int AceitaLancamentos { get; set; }
}