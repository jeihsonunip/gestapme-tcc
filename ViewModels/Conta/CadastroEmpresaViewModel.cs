using System.ComponentModel.DataAnnotations;
using GestaPME.Validators;

namespace GestaPME.ViewModels.Conta;

public class CadastroEmpresaViewModel{
    // Empresa
    [Required(ErrorMessage = "Informe a razão social.")]
    [StringLength(200, MinimumLength = 2)]
    [Display(Name = "Razão social")]
    public string RazaoSocial{ get; set; } = string.Empty;

    [StringLength(200)]
    [Display(Name = "Nome fantasia")]
    public string? NomeFantasia{ get; set; }

    [Required(ErrorMessage = "Informe o CNPJ.")]
    [ValidadorCnpj]
    [Display(Name = "CNPJ")]
    public string Cnpj{ get; set; } = string.Empty;

    // Admin
    [Required(ErrorMessage = "Informe seu nome.")]
    [StringLength(100, MinimumLength = 2)]
    [Display(Name = "Seu nome")]
    public string NomeAdmin{ get; set; } = string.Empty;

    [Required(ErrorMessage = "Informe o e-mail.")]
    [EmailAddress(ErrorMessage = "E-mail inválido.")]
    [Display(Name = "E-mail")]
    public string EmailAdmin{ get; set; } = string.Empty;

    [Required(ErrorMessage = "Informe a senha.")]
    [StringLength(100, MinimumLength = 8, ErrorMessage = "A senha deve ter ao menos 8 caracteres.")]
    [RegularExpression(@"^(?=.*[A-Za-z])(?=.*\d).+$",
        ErrorMessage = "A senha deve conter letras e números.")]
    [DataType(DataType.Password)]
    [Display(Name = "Senha")]
    public string Senha{ get; set; } = string.Empty;

    [Required(ErrorMessage = "Confirme a senha.")]
    [Compare(nameof(Senha), ErrorMessage = "As senhas não conferem.")]
    [DataType(DataType.Password)]
    [Display(Name = "Confirmar senha")]
    public string ConfirmarSenha{ get; set; } = string.Empty;
}