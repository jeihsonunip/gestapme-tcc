using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using GestaPME.Data;
using GestaPME.Services;
using GestaPME.ViewModels.Empresa;

namespace GestaPME.Controllers;

[Authorize]
public class EmpresaController : Controller{
    private readonly AppDbContext _db;
    private readonly IContextoEmpresa _ctx;

    public EmpresaController(AppDbContext db, IContextoEmpresa ctx){
        _db = db;
        _ctx = ctx;
    }

    public async Task<IActionResult> Index(){
        var e = await _db.Empresas.FindAsync(_ctx.EmpresaId);
        if (e == null) return NotFound();
        return View(e);
    }

    [Authorize(Roles = "Administrador")]
    [HttpGet]
    public async Task<IActionResult> Edit(){
        var e = await _db.Empresas.FindAsync(_ctx.EmpresaId);
        if (e == null) return NotFound();
        return View(new EmpresaEditViewModel{
            Id = e.Id, RazaoSocial = e.RazaoSocial, NomeFantasia = e.NomeFantasia,
            Cnpj = e.CNPJ, Endereco = e.Endereco, Telefone = e.Telefone, Email = e.Email
        });
    }

    [Authorize(Roles = "Administrador")]
    [HttpPost]
    public async Task<IActionResult> Edit(EmpresaEditViewModel vm){
        if (!ModelState.IsValid) return View(vm);
        var e = await _db.Empresas.FindAsync(_ctx.EmpresaId);
        if (e == null) return NotFound();

        e.RazaoSocial = vm.RazaoSocial;
        e.NomeFantasia = vm.NomeFantasia;
        e.Endereco = vm.Endereco;
        e.Telefone = vm.Telefone;
        e.Email = vm.Email;
        await _db.SaveChangesAsync();

        TempData["Sucesso"] = "Dados da empresa atualizados.";
        return RedirectToAction(nameof(Index));
    }
}