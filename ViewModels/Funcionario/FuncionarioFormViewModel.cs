// ViewModels/Funcionario/FuncionarioFormViewModel.cs

using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using GestaPME.Validators;

namespace GestaPME.ViewModels.Funcionario;

public class FuncionarioFormViewModel{
    public Guid Id{ get; set; }

    [Required, StringLength(200, MinimumLength = 3)]
    [Display(Name = "Nome completo")]
    public string NomeCompleto{ get; set; } = string.Empty;

    [Required, ValidadorCpf]
    [Display(Name = "CPF")]
    public string Cpf{ get; set; } = string.Empty;

    [EmailAddress, StringLength(200)]
    [Display(Name = "E-mail")]
    public string? Email{ get; set; }

    [StringLength(20)]
    [Display(Name = "Telefone")]
    public string? Telefone{ get; set; }

    [DataType(DataType.Date)]
    [Display(Name = "Data de nascimento")]
    public DateTime? DataNascimento{ get; set; }

    [Required]
    [DataType(DataType.Date)]
    [Display(Name = "Data de admissão")]
    public DateTime DataAdmissao{ get; set; } = DateTime.Today;

    [Required]
    [Display(Name = "Departamento")]
    public Guid DepartamentoId{ get; set; }

    [Required] [Display(Name = "Cargo")] public Guid CargoId{ get; set; }

    [Display(Name = "Ativo")] public bool Ativo{ get; set; } = true;

    public IEnumerable<SelectListItem> Departamentos{ get; set; } = new List<SelectListItem>();
    public IEnumerable<SelectListItem> Cargos{ get; set; } = new List<SelectListItem>();
}