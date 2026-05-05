using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using GestaPME.Data;
using GestaPME.Models;
using GestaPME.Services;
using GestaPME.Util;
using GestaPME.ViewModels.Funcionario;

namespace GestaPME.Controllers;

[Authorize]
public class FuncionariosController : Controller{
    private readonly AppDbContext _db;
    private readonly IContextoEmpresa _ctx;

    public FuncionariosController(AppDbContext db, IContextoEmpresa ctx){
        _db = db;
        _ctx = ctx;
    }

    public async Task<IActionResult> Index(FuncionarioFiltroViewModel filtro, int pagina = 1){
        var q = _db.Funcionarios
            .Where(f => f.EmpresaId == _ctx.EmpresaId)
            .Include(f => f.Departamento).Include(f => f.Cargo)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(filtro.Busca)){
            var busca = filtro.Busca.Trim();
            q = q.Where(f => f.NomeCompleto.Contains(busca) || f.CPF.Contains(busca));
        }

        if (filtro.DepartamentoId.HasValue)
            q = q.Where(f => f.DepartamentoId == filtro.DepartamentoId);
        if (filtro.CargoId.HasValue)
            q = q.Where(f => f.CargoId == filtro.CargoId);
        if (filtro.Status == "ativo") q = q.Where(f => f.Ativo);
        if (filtro.Status == "inativo") q = q.Where(f => !f.Ativo);

        q = q.OrderBy(f => f.NomeCompleto);
        var resultado = await ResultadoPaginado<Funcionario>.CriarAsync(q, pagina, 20);

        filtro.Departamentos = await SelectDepartamentos();
        filtro.Cargos = await SelectCargos();
        ViewBag.Filtro = filtro;
        return View(resultado);
    }

    public async Task<IActionResult> Details(Guid id){
        var f = await _db.Funcionarios
            .Include(x => x.Departamento).Include(x => x.Cargo)
            .Include(x => x.Ferias.OrderByDescending(z => z.DataInicio).Take(5))
            .FirstOrDefaultAsync(x => x.Id == id && x.EmpresaId == _ctx.EmpresaId);
        if (f == null) return NotFound();
        return View(f);
    }

    [Authorize(Roles = "Administrador")]
    public async Task<IActionResult> Create(){
        var vm = new FuncionarioFormViewModel{
            Departamentos = await SelectDepartamentos(),
            Cargos = await SelectCargos()
        };
        return View(vm);
    }

    [Authorize(Roles = "Administrador")]
    [HttpPost]
    public async Task<IActionResult> Create(FuncionarioFormViewModel vm){
        vm.Cpf = (vm.Cpf ?? "").Replace(".", "").Replace("-", "").Trim();
        if (ModelState.IsValid
            && await _db.Funcionarios.AnyAsync(f => f.CPF == vm.Cpf && f.EmpresaId == _ctx.EmpresaId)){
            ModelState.AddModelError(nameof(vm.Cpf), "CPF já cadastrado nesta empresa.");
        }

        if (!ModelState.IsValid){
            vm.Departamentos = await SelectDepartamentos();
            vm.Cargos = await SelectCargos();
            return View(vm);
        }

        var f = new Funcionario{
            NomeCompleto = vm.NomeCompleto,
            CPF = vm.Cpf,
            Email = vm.Email,
            Telefone = vm.Telefone,
            DataNascimento = vm.DataNascimento,
            DataAdmissao = vm.DataAdmissao,
            Ativo = vm.Ativo,
            DepartamentoId = vm.DepartamentoId,
            CargoId = vm.CargoId,
            EmpresaId = _ctx.EmpresaId
        };
        _db.Funcionarios.Add(f);
        await _db.SaveChangesAsync();
        TempData["Sucesso"] = "Funcionário cadastrado.";
        return RedirectToAction(nameof(Index));
    }

    [Authorize(Roles = "Administrador")]
    public async Task<IActionResult> Edit(Guid id){
        var f = await _db.Funcionarios.FirstOrDefaultAsync(x => x.Id == id && x.EmpresaId == _ctx.EmpresaId);
        if (f == null) return NotFound();
        var vm = new FuncionarioFormViewModel{
            Id = f.Id, NomeCompleto = f.NomeCompleto, Cpf = f.CPF,
            Email = f.Email, Telefone = f.Telefone,
            DataNascimento = f.DataNascimento, DataAdmissao = f.DataAdmissao,
            Ativo = f.Ativo, DepartamentoId = f.DepartamentoId, CargoId = f.CargoId,
            Departamentos = await SelectDepartamentos(),
            Cargos = await SelectCargos()
        };
        return View(vm);
    }

    [Authorize(Roles = "Administrador")]
    [HttpPost]
    public async Task<IActionResult> Edit(FuncionarioFormViewModel vm){
        if (!ModelState.IsValid){
            vm.Departamentos = await SelectDepartamentos();
            vm.Cargos = await SelectCargos();
            return View(vm);
        }

        var f = await _db.Funcionarios.FirstOrDefaultAsync(x => x.Id == vm.Id && x.EmpresaId == _ctx.EmpresaId);
        if (f == null) return NotFound();

        f.NomeCompleto = vm.NomeCompleto;
        f.Email = vm.Email;
        f.Telefone = vm.Telefone;
        f.DataNascimento = vm.DataNascimento;
        f.DataAdmissao = vm.DataAdmissao;
        f.Ativo = vm.Ativo;
        f.DepartamentoId = vm.DepartamentoId;
        f.CargoId = vm.CargoId;
        await _db.SaveChangesAsync();
        TempData["Sucesso"] = "Funcionário atualizado.";
        return RedirectToAction(nameof(Index));
    }

    [Authorize(Roles = "Administrador")]
    [HttpPost]
    public async Task<IActionResult> AlternarAtivo(Guid id){
        var f = await _db.Funcionarios.FirstOrDefaultAsync(x => x.Id == id && x.EmpresaId == _ctx.EmpresaId);
        if (f == null) return NotFound();
        f.Ativo = !f.Ativo;
        if (!f.Ativo) f.DataDesligamento ??= DateTime.UtcNow.Date;
        else f.DataDesligamento = null;
        await _db.SaveChangesAsync();
        TempData["Sucesso"] = f.Ativo ? "Funcionário reativado." : "Funcionário inativado.";
        return RedirectToAction(nameof(Index));
    }

    private async Task<List<SelectListItem>> SelectDepartamentos() =>
        await _db.Departamentos.Where(d => d.EmpresaId == _ctx.EmpresaId)
            .OrderBy(d => d.Nome)
            .Select(d => new SelectListItem(d.Nome, d.Id.ToString())).ToListAsync();

    private async Task<List<SelectListItem>> SelectCargos() =>
        await _db.Cargos.Where(c => c.EmpresaId == _ctx.EmpresaId)
            .OrderBy(c => c.Nome)
            .Select(c => new SelectListItem(c.Nome, c.Id.ToString())).ToListAsync();
}