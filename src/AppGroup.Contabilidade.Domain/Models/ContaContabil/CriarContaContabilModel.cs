namespace AppGroup.Contabilidade.Domain.Models.ContaContabil;

public class CriarContaContabilModel
{
    public string Codigo { get; set; } = string.Empty;
    public string Nome { get; set; } = string.Empty;
    public int Tipo { get; set; }
    public int AceitaLancamentos { get; set; }
    public Guid? IdPai { get; set; }
}
