// ViewModels/Usuario/ResetarSenhaViewModel.cs

using System.ComponentModel.DataAnnotations;

namespace GestaPME.ViewModels.Usuario;

public class ResetarSenhaViewModel{
    public Guid Id{ get; set; }
    public string NomeUsuario{ get; set; } = string.Empty;

    [Required, StringLength(100, MinimumLength = 8)]
    [RegularExpression(@"^(?=.*[A-Za-z])(?=.*\d).+$", ErrorMessage = "A senha deve conter letras e números.")]
    [DataType(DataType.Password)]
    [Display(Name = "Nova senha")]
    public string NovaSenha{ get; set; } = string.Empty;

    [Required, Compare(nameof(NovaSenha), ErrorMessage = "As senhas não conferem.")]
    [DataType(DataType.Password)]
    [Display(Name = "Confirmar")]
    public string ConfirmarSenha{ get; set; } = string.Empty;
}