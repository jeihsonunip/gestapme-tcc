using System.ComponentModel.DataAnnotations;

namespace GestaPME.Models;

public class Funcionario : BaseEntity{
    [Required, MaxLength(200)] public string NomeCompleto{ get; set; } = string.Empty;

    [Required, MaxLength(14)] public string CPF{ get; set; } = string.Empty;

    [MaxLength(200)] public string? Email{ get; set; }

    [MaxLength(20)] public string? Telefone{ get; set; }

    public DateTime? DataNascimento{ get; set; }

    [Required] public DateTime DataAdmissao{ get; set; }

    public DateTime? DataDesligamento{ get; set; }

    public bool Ativo{ get; set; } = true;

    public Guid DepartamentoId{ get; set; }
    public Departamento Departamento{ get; set; } = null!;

    public Guid CargoId{ get; set; }
    public Cargo Cargo{ get; set; } = null!;

    public Guid EmpresaId{ get; set; }
    public Empresa Empresa{ get; set; } = null!;

    public ICollection<Ferias> Ferias{ get; set; } = new List<Ferias>();
}