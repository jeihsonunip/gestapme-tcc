using System.ComponentModel.DataAnnotations;

namespace GestaPME.ViewModels.Empresa;

public class EmpresaEditViewModel{
    public Guid Id{ get; set; }

    [Required, StringLength(200, MinimumLength = 2)]
    [Display(Name = "Razão social")]
    public string RazaoSocial{ get; set; } = string.Empty;

    [StringLength(200)]
    [Display(Name = "Nome fantasia")]
    public string? NomeFantasia{ get; set; }

    [Display(Name = "CNPJ")] public string Cnpj{ get; set; } = string.Empty; // somente leitura na tela

    [StringLength(500)]
    [Display(Name = "Endereço")]
    public string? Endereco{ get; set; }

    [StringLength(20)]
    [Display(Name = "Telefone")]
    public string? Telefone{ get; set; }

    [StringLength(200), EmailAddress]
    [Display(Name = "E-mail de contato")]
    public string? Email{ get; set; }
}