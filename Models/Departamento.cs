using System.ComponentModel.DataAnnotations;

namespace GestaPME.Models;

public class Departamento : BaseEntity{
    [Required, MaxLength(150)] public string Nome{ get; set; } = string.Empty;

    [MaxLength(500)] public string? Descricao{ get; set; }

    public Guid EmpresaId{ get; set; }
    public Empresa Empresa{ get; set; } = null!;

    public ICollection<Funcionario> Funcionarios{ get; set; } = new List<Funcionario>();
}