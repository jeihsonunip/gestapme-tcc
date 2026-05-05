using System.ComponentModel.DataAnnotations;

namespace GestaPME.Models;

public enum PerfilUsuario{
    Administrador,
    Gestor
}

public class Usuario : BaseEntity{
    [Required, MaxLength(100)] public string Nome{ get; set; } = string.Empty;

    [Required, MaxLength(200)] public string Email{ get; set; } = string.Empty;

    [Required] public string SenhaHash{ get; set; } = string.Empty;

    public PerfilUsuario Perfil{ get; set; } = PerfilUsuario.Gestor;

    public bool Ativo{ get; set; } = true;

    public Guid EmpresaId{ get; set; }
    public Empresa Empresa{ get; set; } = null!;
}