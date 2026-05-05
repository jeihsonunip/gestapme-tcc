// ViewModels/Usuario/UsuarioFormViewModel.cs

using System.ComponentModel.DataAnnotations;
using GestaPME.Models;

namespace GestaPME.ViewModels.Usuario;

public class UsuarioFormViewModel{
    public Guid Id{ get; set; }
    public bool Edicao{ get; set; }

    [Required, StringLength(100, MinimumLength = 2)]
    [Display(Name = "Nome")]
    public string Nome{ get; set; } = string.Empty;

    [Required, EmailAddress, StringLength(200)]
    [Display(Name = "E-mail")]
    public string Email{ get; set; } = string.Empty;

    [Required] [Display(Name = "Perfil")] public PerfilUsuario Perfil{ get; set; } = PerfilUsuario.Gestor;

    // Senha só obrigatória no Create
    [StringLength(100, MinimumLength = 8)]
    [RegularExpression(@"^(?=.*[A-Za-z])(?=.*\d).+$", ErrorMessage = "A senha deve conter letras e números.")]
    [DataType(DataType.Password)]
    [Display(Name = "Senha")]
    public string? Senha{ get; set; }

    [Display(Name = "Ativo")] public bool Ativo{ get; set; } = true;
}