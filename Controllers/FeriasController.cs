using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using GestaPME.Data;
using GestaPME.Models;
using GestaPME.Services;
using GestaPME.Util;
using GestaPME.ViewModels.Ferias;

namespace GestaPME.Controllers;

[Authorize]
public class FeriasController : Controller{
    private readonly AppDbContext _db;
    private readonly IContextoEmpresa _ctx;

    public FeriasController(AppDbContext db, IContextoEmpresa ctx){
        _db = db;
        _ctx = ctx;
    }

    public async Task<IActionResult> Index(FeriasFiltroViewModel filtro, int pagina = 1){
        var q = _db.Ferias
            .Where(f => f.Funcionario.EmpresaId == _ctx.EmpresaId)
            .Include(f => f.Funcionario)
            .AsQueryable();

        if (filtro.FuncionarioId.HasValue)
            q = q.Where(f => f.FuncionarioId == filtro.FuncionarioId);
        if (filtro.Status.HasValue)
            q = q.Where(f => f.Status == filtro.Status);

        q = q.OrderByDescending(f => f.DataInicio);
        var resultado = await ResultadoPaginado<Ferias>.CriarAsync(q, pagina, 20);

        filtro.Funcionarios = await SelectFuncionarios();
        ViewBag.Filtro = filtro;
        return View(resultado);
    }

    public async Task<IActionResult> Details(Guid id){
        var f = await _db.Ferias
            .Include(x => x.Funcionario)
            .FirstOrDefaultAsync(x => x.Id == id && x.Funcionario.EmpresaId == _ctx.EmpresaId);
        if (f == null) return NotFound();
        return View(f);
    }

    public async Task<IActionResult> Solicitar(){
        var vm = new FeriasSolicitacaoViewModel{
            Funcionarios = await SelectFuncionarios(apenasAtivos: true)
        };
        return View(vm);
    }

    [HttpPost]
    public async Task<IActionResult> Solicitar(FeriasSolicitacaoViewModel vm){
        if (ModelState.IsValid && await HaSobreposicao(vm.FuncionarioId, vm.DataInicio, vm.DataFim, null))
            ModelState.AddModelError(string.Empty,
                "Já existe um período de férias aprovado/solicitado que se sobrepõe a estas datas.");

        if (!ModelState.IsValid){
            vm.Funcionarios = await SelectFuncionarios(apenasAtivos: true);
            return View(vm);
        }

        // Garantir que o funcionário é da empresa do usuário
        var pertence = await _db.Funcionarios
            .AnyAsync(f => f.Id == vm.FuncionarioId && f.EmpresaId == _ctx.EmpresaId);
        if (!pertence) return NotFound();

        var ferias = new Ferias{
            FuncionarioId = vm.FuncionarioId,
            DataInicio = vm.DataInicio,
            DataFim = vm.DataFim,
            Observacoes = vm.Observacoes,
            Status = StatusFerias.Solicitada
        };
        _db.Ferias.Add(ferias);
        await _db.SaveChangesAsync();
        TempData["Sucesso"] = "Solicitação de férias registrada.";
        return RedirectToAction(nameof(Index));
    }

    [Authorize(Roles = "Administrador")]
    [HttpPost]
    public async Task<IActionResult> Aprovar(Guid id){
        var f = await BuscarPorIdMinhaEmpresa(id);
        if (f == null) return NotFound();
        if (f.Status != StatusFerias.Solicitada){
            TempData["Erro"] = "Apenas solicitações pendentes podem ser aprovadas.";
            return RedirectToAction(nameof(Details), new{ id });
        }

        f.Status = StatusFerias.Aprovada;
        await _db.SaveChangesAsync();
        TempData["Sucesso"] = "Férias aprovadas.";
        return RedirectToAction(nameof(Details), new{ id });
    }

    [Authorize(Roles = "Administrador")]
    [HttpPost]
    public async Task<IActionResult> Rejeitar(Guid id){
        var f = await BuscarPorIdMinhaEmpresa(id);
        if (f == null) return NotFound();
        if (f.Status != StatusFerias.Solicitada){
            TempData["Erro"] = "Apenas solicitações pendentes podem ser rejeitadas.";
            return RedirectToAction(nameof(Details), new{ id });
        }

        f.Status = StatusFerias.Rejeitada;
        await _db.SaveChangesAsync();
        TempData["Sucesso"] = "Solicitação rejeitada.";
        return RedirectToAction(nameof(Details), new{ id });
    }

    private async Task<Ferias?> BuscarPorIdMinhaEmpresa(Guid id) =>
        await _db.Ferias.Include(f => f.Funcionario)
            .FirstOrDefaultAsync(f => f.Id == id && f.Funcionario.EmpresaId == _ctx.EmpresaId);

    private async Task<bool> HaSobreposicao(Guid funcionarioId, DateTime inicio, DateTime fim, Guid? ignorarId) =>
        await _db.Ferias.AnyAsync(f =>
            f.FuncionarioId == funcionarioId
            && f.Status != StatusFerias.Rejeitada
            && (ignorarId == null || f.Id != ignorarId)
            && f.DataInicio <= fim && f.DataFim >= inicio);

    private async Task<List<SelectListItem>> SelectFuncionarios(bool apenasAtivos = false){
        var q = _db.Funcionarios.Where(f => f.EmpresaId == _ctx.EmpresaId);
        if (apenasAtivos) q = q.Where(f => f.Ativo);
        return await q.OrderBy(f => f.NomeCompleto)
            .Select(f => new SelectListItem(f.NomeCompleto, f.Id.ToString()))
            .ToListAsync();
    }
}