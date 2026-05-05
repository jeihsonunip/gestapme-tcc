using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GestaPME.Data;
using GestaPME.Models;
using GestaPME.Services;
using GestaPME.Util;
using GestaPME.ViewModels.Cargo;

namespace GestaPME.Controllers;

[Authorize]
public class CargosController : Controller{
    private readonly AppDbContext _db;
    private readonly IContextoEmpresa _ctx;

    public CargosController(AppDbContext db, IContextoEmpresa ctx){
        _db = db;
        _ctx = ctx;
    }

    public async Task<IActionResult> Index(string? busca, int pagina = 1){
        var q = _db.Cargos.Where(c => c.EmpresaId == _ctx.EmpresaId);
        if (!string.IsNullOrWhiteSpace(busca))
            q = q.Where(c => c.Nome.Contains(busca));
        q = q.OrderBy(c => c.Nome);
        var resultado = await ResultadoPaginado<Cargo>.CriarAsync(q, pagina, 20);
        ViewBag.Busca = busca;
        return View(resultado);
    }

    [Authorize(Roles = "Administrador")]
    public IActionResult Create() => View(new CargoFormViewModel());

    [Authorize(Roles = "Administrador")]
    [HttpPost]
    public async Task<IActionResult> Create(CargoFormViewModel vm){
        if (!ModelState.IsValid) return View(vm);
        var c = new Cargo{ Nome = vm.Nome, Descricao = vm.Descricao, EmpresaId = _ctx.EmpresaId };
        _db.Cargos.Add(c);
        await _db.SaveChangesAsync();
        TempData["Sucesso"] = "Cargo cadastrado.";
        return RedirectToAction(nameof(Index));
    }

    [Authorize(Roles = "Administrador")]
    public async Task<IActionResult> Edit(Guid id){
        var c = await _db.Cargos.FirstOrDefaultAsync(x => x.Id == id && x.EmpresaId == _ctx.EmpresaId);
        if (c == null) return NotFound();
        return View(new CargoFormViewModel{ Id = c.Id, Nome = c.Nome, Descricao = c.Descricao });
    }

    [Authorize(Roles = "Administrador")]
    [HttpPost]
    public async Task<IActionResult> Edit(CargoFormViewModel vm){
        if (!ModelState.IsValid) return View(vm);
        var c = await _db.Cargos.FirstOrDefaultAsync(x => x.Id == vm.Id && x.EmpresaId == _ctx.EmpresaId);
        if (c == null) return NotFound();
        c.Nome = vm.Nome;
        c.Descricao = vm.Descricao;
        await _db.SaveChangesAsync();
        TempData["Sucesso"] = "Cargo atualizado.";
        return RedirectToAction(nameof(Index));
    }

    [Authorize(Roles = "Administrador")]
    [HttpPost]
    public async Task<IActionResult> Delete(Guid id){
        var c = await _db.Cargos.FirstOrDefaultAsync(x => x.Id == id && x.EmpresaId == _ctx.EmpresaId);
        if (c == null) return NotFound();
        try{
            _db.Cargos.Remove(c);
            await _db.SaveChangesAsync();
            TempData["Sucesso"] = "Cargo excluído.";
        }
        catch (DbUpdateException){
            TempData["Erro"] = "Não é possível excluir: há funcionários vinculados a este cargo.";
        }

        return RedirectToAction(nameof(Index));
    }
}