using System.ComponentModel.DataAnnotations;

namespace GestaPME.ViewModels.Departamento;

public class DepartamentoFormViewModel{
    public Guid Id{ get; set; }

    [Required(ErrorMessage = "Informe o nome.")]
    [StringLength(100, MinimumLength = 2)]
    [Display(Name = "Nome")]
    public string Nome{ get; set; } = string.Empty;

    [StringLength(500)]
    [Display(Name = "Descrição")]
    public string? Descricao{ get; set; }
}