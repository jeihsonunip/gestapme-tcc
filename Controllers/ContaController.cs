using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using GestaPME.Services;
using GestaPME.ViewModels.Conta;

namespace GestaPME.Controllers;

[Authorize]
public class ContaController : Controller{
    private readonly AutenticacaoService _auth;
    private readonly IContextoEmpresa _contexto;

    public ContaController(AutenticacaoService auth, IContextoEmpresa contexto){
        _auth = auth;
        _contexto = contexto;
    }

    [AllowAnonymous]
    [HttpGet]
    public IActionResult Login(string? returnUrl = null){
        if (User.Identity?.IsAuthenticated == true)
            return RedirectToAction("Index", "Home");
        return View(new LoginViewModel{ ReturnUrl = returnUrl });
    }

    [AllowAnonymous]
    [HttpPost]
    public async Task<IActionResult> Login(LoginViewModel vm){
        if (!ModelState.IsValid) return View(vm);

        var usuario = await _auth.ValidarCredenciaisAsync(vm.Email, vm.Senha);
        if (usuario == null){
            ModelState.AddModelError(string.Empty, "E-mail ou senha inválidos.");
            return View(vm);
        }

        await _auth.LogarAsync(HttpContext, usuario);
        TempData["Sucesso"] = $"Bem-vindo, {usuario.Nome}.";
        if (!string.IsNullOrEmpty(vm.ReturnUrl) && Url.IsLocalUrl(vm.ReturnUrl))
            return Redirect(vm.ReturnUrl);
        return RedirectToAction("Index", "Home");
    }

    [AllowAnonymous]
    [HttpGet]
    public IActionResult Cadastro(){
        if (User.Identity?.IsAuthenticated == true)
            return RedirectToAction("Index", "Home");
        return View(new CadastroEmpresaViewModel());
    }

    [AllowAnonymous]
    [HttpPost]
    public async Task<IActionResult> Cadastro(CadastroEmpresaViewModel vm){
        if (!ModelState.IsValid) return View(vm);

        try{
            var cnpj = vm.Cnpj.Replace(".", "").Replace("/", "").Replace("-", "").Trim();
            var (_, admin) = await _auth.CadastrarEmpresaAsync(
                vm.RazaoSocial, vm.NomeFantasia, cnpj,
                vm.NomeAdmin, vm.EmailAdmin, vm.Senha);
            await _auth.LogarAsync(HttpContext, admin);
            TempData["Sucesso"] = "Empresa cadastrada. Bem-vindo ao GestaPME!";
            return RedirectToAction("Index", "Home");
        }
        catch (InvalidOperationException ex){
            ModelState.AddModelError(string.Empty, ex.Message);
            return View(vm);
        }
    }

    [HttpPost]
    public async Task<IActionResult> Sair(){
        await _auth.DeslogarAsync(HttpContext);
        TempData["Info"] = "Você saiu.";
        return RedirectToAction(nameof(Login));
    }

    [AllowAnonymous]
    [HttpGet]
    public IActionResult AcessoNegado() => View();

    [HttpGet]
    public IActionResult MinhaConta() => View();

    [HttpGet]
    public IActionResult AlterarSenha() => View(new AlterarSenhaViewModel());

    [HttpPost]
    public async Task<IActionResult> AlterarSenha(AlterarSenhaViewModel vm){
        if (!ModelState.IsValid) return View(vm);
        try{
            await _auth.AlterarSenhaAsync(_contexto.UsuarioId, vm.SenhaAtual, vm.NovaSenha);
            TempData["Sucesso"] = "Senha alterada com sucesso.";
            return RedirectToAction(nameof(MinhaConta));
        }
        catch (InvalidOperationException ex){
            ModelState.AddModelError(nameof(vm.SenhaAtual), ex.Message);
            return View(vm);
        }
    }
}