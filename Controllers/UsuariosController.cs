using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GestaPME.Data;
using GestaPME.Models;
using GestaPME.Services;
using GestaPME.ViewModels.Usuario;

namespace GestaPME.Controllers;

[Authorize(Roles = "Administrador")]
public class UsuariosController : Controller{
    private readonly AppDbContext _db;
    private readonly IContextoEmpresa _ctx;

    public UsuariosController(AppDbContext db, IContextoEmpresa ctx){
        _db = db;
        _ctx = ctx;
    }

    public async Task<IActionResult> Index(){
        var lista = await _db.Usuarios
            .Where(u => u.EmpresaId == _ctx.EmpresaId)
            .OrderBy(u => u.Nome)
            .ToListAsync();
        return View(lista);
    }

    public IActionResult Create() => View(new UsuarioFormViewModel{ Edicao = false });

    [HttpPost]
    public async Task<IActionResult> Create(UsuarioFormViewModel vm){
        if (string.IsNullOrWhiteSpace(vm.Senha))
            ModelState.AddModelError(nameof(vm.Senha), "Informe uma senha inicial.");
        if (await _db.Usuarios.AnyAsync(u => u.Email == vm.Email))
            ModelState.AddModelError(nameof(vm.Email), "E-mail já cadastrado.");
        if (!ModelState.IsValid) return View(vm);

        var u = new Usuario{
            Nome = vm.Nome, Email = vm.Email, Perfil = vm.Perfil, Ativo = vm.Ativo,
            SenhaHash = AutenticacaoService.GerarHash(vm.Senha!),
            EmpresaId = _ctx.EmpresaId
        };
        _db.Usuarios.Add(u);
        await _db.SaveChangesAsync();
        TempData["Sucesso"] = "Usuário cadastrado.";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(Guid id){
        var u = await BuscarPorId(id);
        if (u == null) return NotFound();
        return View(new UsuarioFormViewModel{
            Id = u.Id, Edicao = true, Nome = u.Nome, Email = u.Email,
            Perfil = u.Perfil, Ativo = u.Ativo
        });
    }

    [HttpPost]
    public async Task<IActionResult> Edit(UsuarioFormViewModel vm){
        vm.Edicao = true;
        ModelState.Remove(nameof(vm.Senha)); // senha não faz parte do Edit
        if (!ModelState.IsValid) return View(vm);

        var u = await BuscarPorId(vm.Id);
        if (u == null) return NotFound();
        // Não permitir o Admin desativar a si mesmo
        if (u.Id == _ctx.UsuarioId && (!vm.Ativo || vm.Perfil != PerfilUsuario.Administrador)){
            ModelState.AddModelError(string.Empty,
                "Você não pode alterar seu próprio perfil ou desativar sua conta aqui.");
            return View(vm);
        }

        u.Nome = vm.Nome;
        u.Perfil = vm.Perfil;
        u.Ativo = vm.Ativo;
        await _db.SaveChangesAsync();
        TempData["Sucesso"] = "Usuário atualizado.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public async Task<IActionResult> AlternarAtivo(Guid id){
        var u = await BuscarPorId(id);
        if (u == null) return NotFound();
        if (u.Id == _ctx.UsuarioId){
            TempData["Erro"] = "Você não pode inativar sua própria conta.";
            return RedirectToAction(nameof(Index));
        }

        u.Ativo = !u.Ativo;
        await _db.SaveChangesAsync();
        TempData["Sucesso"] = u.Ativo ? "Usuário reativado." : "Usuário inativado.";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> ResetarSenha(Guid id){
        var u = await BuscarPorId(id);
        if (u == null) return NotFound();
        return View(new ResetarSenhaViewModel{ Id = u.Id, NomeUsuario = u.Nome });
    }

    [HttpPost]
    public async Task<IActionResult> ResetarSenha(ResetarSenhaViewModel vm){
        if (!ModelState.IsValid) return View(vm);
        var u = await BuscarPorId(vm.Id);
        if (u == null) return NotFound();
        u.SenhaHash = AutenticacaoService.GerarHash(vm.NovaSenha);
        await _db.SaveChangesAsync();
        TempData["Sucesso"] = $"Senha de {u.Nome} redefinida.";
        return RedirectToAction(nameof(Index));
    }

    private Task<Usuario?> BuscarPorId(Guid id) =>
        _db.Usuarios.FirstOrDefaultAsync(u => u.Id == id && u.EmpresaId == _ctx.EmpresaId);
}