using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace GestaPME.ViewModels.Ferias;

public class FeriasSolicitacaoViewModel : IValidatableObject{
    public Guid Id{ get; set; }

    [Required]
    [Display(Name = "Funcionário")]
    public Guid FuncionarioId{ get; set; }

    [Required, DataType(DataType.Date)]
    [Display(Name = "Data de início")]
    public DateTime DataInicio{ get; set; } = DateTime.Today;

    [Required, DataType(DataType.Date)]
    [Display(Name = "Data de fim")]
    public DateTime DataFim{ get; set; } = DateTime.Today.AddDays(14);

    [StringLength(500)]
    [Display(Name = "Observações")]
    public string? Observacoes{ get; set; }

    public IEnumerable<SelectListItem> Funcionarios{ get; set; } = new List<SelectListItem>();

    public IEnumerable<ValidationResult> Validate(ValidationContext ctx){
        if (DataFim < DataInicio)
            yield return new ValidationResult("A data final deve ser maior ou igual à inicial.",
                new[]{ nameof(DataFim) });
    }
}