using System.ComponentModel.DataAnnotations;

namespace GestaPME.Models;

public class Empresa : BaseEntity{
    [Required, MaxLength(200)] public string RazaoSocial{ get; set; } = string.Empty;

    [MaxLength(200)] public string? NomeFantasia{ get; set; }

    [Required, MaxLength(18)] public string CNPJ{ get; set; } = string.Empty;

    [MaxLength(500)] public string? Endereco{ get; set; }

    [MaxLength(20)] public string? Telefone{ get; set; }

    [MaxLength(200)] public string? Email{ get; set; }

    // Navigation
    public ICollection<Departamento> Departamentos{ get; set; } = new List<Departamento>();
    public ICollection<Cargo> Cargos{ get; set; } = new List<Cargo>();
    public ICollection<Funcionario> Funcionarios{ get; set; } = new List<Funcionario>();
    public ICollection<Usuario> Usuarios{ get; set; } = new List<Usuario>();
}