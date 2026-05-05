using System.ComponentModel.DataAnnotations;

namespace GestaPME.Models;

public enum StatusFerias{
    Solicitada,
    Aprovada,
    Rejeitada,
    Concluida
}

public class Ferias : BaseEntity{
    [Required] public DateTime DataInicio{ get; set; }

    [Required] public DateTime DataFim{ get; set; }

    public int QuantidadeDias => (DataFim - DataInicio).Days + 1;

    public StatusFerias Status{ get; set; } = StatusFerias.Solicitada;

    [MaxLength(500)] public string? Observacoes{ get; set; }

    public Guid FuncionarioId{ get; set; }
    public Funcionario Funcionario{ get; set; } = null!;
}