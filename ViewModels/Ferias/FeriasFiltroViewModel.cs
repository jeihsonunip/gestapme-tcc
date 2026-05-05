using Microsoft.AspNetCore.Mvc.Rendering;
using GestaPME.Models;

namespace GestaPME.ViewModels.Ferias;

public class FeriasFiltroViewModel{
    public Guid? FuncionarioId{ get; set; }
    public StatusFerias? Status{ get; set; }
    public IEnumerable<SelectListItem> Funcionarios{ get; set; } = new List<SelectListItem>();
}