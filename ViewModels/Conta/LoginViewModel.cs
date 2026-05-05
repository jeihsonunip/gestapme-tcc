using System.ComponentModel.DataAnnotations;

namespace GestaPME.ViewModels.Conta;

public class LoginViewModel{
    [Required(ErrorMessage = "Informe seu e-mail.")]
    [EmailAddress(ErrorMessage = "E-mail inválido.")]
    [Display(Name = "E-mail")]
    public string Email{ get; set; } = string.Empty;

    [Required(ErrorMessage = "Informe sua senha.")]
    [DataType(DataType.Password)]
    [Display(Name = "Senha")]
    public string Senha{ get; set; } = string.Empty;

    public string? ReturnUrl{ get; set; }
}