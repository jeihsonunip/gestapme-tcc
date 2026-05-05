// ViewModels/Funcionario/FuncionarioFiltroViewModel.cs

using Microsoft.AspNetCore.Mvc.Rendering;

namespace GestaPME.ViewModels.Funcionario;

public class FuncionarioFiltroViewModel{
    public string? Busca{ get; set; }
    public Guid? DepartamentoId{ get; set; }
    public Guid? CargoId{ get; set; }
    public string? Status{ get; set; } // "ativo", "inativo", null

    public IEnumerable<SelectListItem> Departamentos{ get; set; } = new List<SelectListItem>();
    public IEnumerable<SelectListItem> Cargos{ get; set; } = new List<SelectListItem>();
}