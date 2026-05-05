using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using GestaPME.Data;
using GestaPME.Models;
using BC = BCrypt.Net.BCrypt;

namespace GestaPME.Services;

public class AutenticacaoService{
    private readonly AppDbContext _db;

    public AutenticacaoService(AppDbContext db) => _db = db;

    public static string GerarHash(string senha) => BC.HashPassword(senha);

    public static bool VerificarHash(string senha, string hash) => BC.Verify(senha, hash);

    public async Task<Usuario?> ValidarCredenciaisAsync(string email, string senha){
        var usuario = await _db.Usuarios
            .FirstOrDefaultAsync(u => u.Email == email && u.Ativo);
        if (usuario == null) return null;
        if (!VerificarHash(senha, usuario.SenhaHash)) return null;
        return usuario;
    }

    public async Task LogarAsync(HttpContext httpContext, Usuario usuario){
        var claims = new List<Claim>{
            new(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
            new(ClaimTypes.Name, usuario.Nome),
            new(ClaimTypes.Email, usuario.Email),
            new(ClaimTypes.Role, usuario.Perfil.ToString()),
            new("EmpresaId", usuario.EmpresaId.ToString())
        };

        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        var principal = new ClaimsPrincipal(identity);

        await httpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            principal,
            new AuthenticationProperties{ IsPersistent = false });
    }

    public async Task DeslogarAsync(HttpContext httpContext){
        await httpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
    }

    public async Task<(Empresa empresa, Usuario admin)> CadastrarEmpresaAsync(
        string razaoSocial, string? nomeFantasia, string cnpj,
        string nomeAdmin, string emailAdmin, string senhaAdmin){
        if (await _db.Empresas.AnyAsync(e => e.CNPJ == cnpj))
            throw new InvalidOperationException("CNPJ já cadastrado.");

        if (await _db.Usuarios.AnyAsync(u => u.Email == emailAdmin))
            throw new InvalidOperationException("E-mail já cadastrado.");

        await using var tx = await _db.Database.BeginTransactionAsync();

        var empresa = new Empresa{
            RazaoSocial = razaoSocial,
            NomeFantasia = nomeFantasia,
            CNPJ = cnpj
        };
        _db.Empresas.Add(empresa);
        await _db.SaveChangesAsync();

        var admin = new Usuario{
            Nome = nomeAdmin,
            Email = emailAdmin,
            SenhaHash = GerarHash(senhaAdmin),
            Perfil = PerfilUsuario.Administrador,
            EmpresaId = empresa.Id
        };
        _db.Usuarios.Add(admin);
        await _db.SaveChangesAsync();

        await tx.CommitAsync();
        return (empresa, admin);
    }

    public async Task AlterarSenhaAsync(Guid usuarioId, string senhaAtual, string novaSenha){
        var usuario = await _db.Usuarios.FindAsync(usuarioId)
                      ?? throw new InvalidOperationException("Usuário não encontrado.");
        if (!VerificarHash(senhaAtual, usuario.SenhaHash))
            throw new InvalidOperationException("Senha atual inválida.");
        usuario.SenhaHash = GerarHash(novaSenha);
        await _db.SaveChangesAsync();
    }
}