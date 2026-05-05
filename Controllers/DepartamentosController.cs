using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GestaPME.Data;
using GestaPME.Models;
using GestaPME.Services;
using GestaPME.Util;
using GestaPME.ViewModels.Departamento;

namespace GestaPME.Controllers;

[Authorize]
public class DepartamentosController : Controller{
    private readonly AppDbContext _db;
    private readonly IContextoEmpresa _ctx;

    public DepartamentosController(AppDbContext db, IContextoEmpresa ctx){
        _db = db;
        _ctx = ctx;
    }

    public async Task<IActionResult> Index(string? busca, int pagina = 1){
        var q = _db.Departamentos.Where(d => d.EmpresaId == _ctx.EmpresaId);
        if (!string.IsNullOrWhiteSpace(busca))
            q = q.Where(d => d.Nome.Contains(busca));
        q = q.OrderBy(d => d.Nome);
        var resultado = await ResultadoPaginado<Departamento>.CriarAsync(q, pagina, 20);
        ViewBag.Busca = busca;
        return View(resultado);
    }

    [Authorize(Roles = "Administrador")]
    public IActionResult Create() => View(new DepartamentoFormViewModel());

    [Authorize(Roles = "Administrador")]
    [HttpPost]
    public async Task<IActionResult> Create(DepartamentoFormViewModel vm){
        if (!ModelState.IsValid) return View(vm);
        var d = new Departamento{ Nome = vm.Nome, Descricao = vm.Descricao, EmpresaId = _ctx.EmpresaId };
        _db.Departamentos.Add(d);
        await _db.SaveChangesAsync();
        TempData["Sucesso"] = "Departamento cadastrado.";
        return RedirectToAction(nameof(Index));
    }

    [Authorize(Roles = "Administrador")]
    public async Task<IActionResult> Edit(Guid id){
        var d = await _db.Departamentos.FirstOrDefaultAsync(x => x.Id == id && x.EmpresaId == _ctx.EmpresaId);
        if (d == null) return NotFound();
        return View(new DepartamentoFormViewModel{ Id = d.Id, Nome = d.Nome, Descricao = d.Descricao });
    }

    [Authorize(Roles = "Administrador")]
    [HttpPost]
    public async Task<IActionResult> Edit(DepartamentoFormViewModel vm){
        if (!ModelState.IsValid) return View(vm);
        var d = await _db.Departamentos.FirstOrDefaultAsync(x => x.Id == vm.Id && x.EmpresaId == _ctx.EmpresaId);
        if (d == null) return NotFound();
        d.Nome = vm.Nome;
        d.Descricao = vm.Descricao;
        await _db.SaveChangesAsync();
        TempData["Sucesso"] = "Departamento atualizado.";
        return RedirectToAction(nameof(Index));
    }

    [Authorize(Roles = "Administrador")]
    [HttpPost]
    public async Task<IActionResult> Delete(Guid id){
        var d = await _db.Departamentos.FirstOrDefaultAsync(x => x.Id == id && x.EmpresaId == _ctx.EmpresaId);
        if (d == null) return NotFound();
        try{
            _db.Departamentos.Remove(d);
            await _db.SaveChangesAsync();
            TempData["Sucesso"] = "Departamento excluído.";
        }
        catch (DbUpdateException){
            TempData["Erro"] = "Não é possível excluir: há funcionários vinculados a este departamento.";
        }

        return RedirectToAction(nameof(Index));
    }
}