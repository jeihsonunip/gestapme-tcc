using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GestaPME.Data;
using GestaPME.Models;
using GestaPME.Services;
using GestaPME.ViewModels.Home;

namespace GestaPME.Controllers;

[Authorize]
public class HomeController : Controller{
    private readonly AppDbContext _db;
    private readonly IContextoEmpresa _ctx;

    public HomeController(AppDbContext db, IContextoEmpresa ctx){
        _db = db;
        _ctx = ctx;
    }

    public async Task<IActionResult> Index(){
        var empresaId = _ctx.EmpresaId;
        var hoje = DateTime.UtcNow.Date;

        var total = await _db.Funcionarios.CountAsync(f => f.EmpresaId == empresaId);
        var ativos = await _db.Funcionarios.CountAsync(f => f.EmpresaId == empresaId && f.Ativo);
        var deptos = await _db.Departamentos.CountAsync(d => d.EmpresaId == empresaId);

        var emCurso = await _db.Ferias
            .Where(f => f.Funcionario.EmpresaId == empresaId
                        && f.Status == StatusFerias.Aprovada
                        && f.DataInicio <= hoje && f.DataFim >= hoje)
            .OrderBy(f => f.DataFim)
            .Take(5)
            .Select(f => new ResumoFeriasItem{
                Id = f.Id,
                Funcionario = f.Funcionario.NomeCompleto,
                DataInicio = f.DataInicio,
                DataFim = f.DataFim,
                Dias = EF.Functions.DateDiffDay(f.DataInicio, f.DataFim) + 1
            })
            .ToListAsync();

        var pendentes = _ctx.EhAdministrador
            ? await _db.Ferias
                .Where(f => f.Funcionario.EmpresaId == empresaId && f.Status == StatusFerias.Solicitada)
                .OrderBy(f => f.CriadoEm)
                .Take(5)
                .Select(f => new ResumoFeriasItem{
                    Id = f.Id,
                    Funcionario = f.Funcionario.NomeCompleto,
                    DataInicio = f.DataInicio,
                    DataFim = f.DataFim,
                    Dias = EF.Functions.DateDiffDay(f.DataInicio, f.DataFim) + 1
                })
                .ToListAsync()
            : new List<ResumoFeriasItem>();

        var vm = new PainelInicialViewModel{
            NomeUsuario = _ctx.NomeUsuario,
            TotalFuncionarios = total,
            FuncionariosAtivos = ativos,
            FuncionariosInativos = total - ativos,
            TotalDepartamentos = deptos,
            FeriasEmCurso = emCurso,
            FeriasPendentes = pendentes
        };
        return View(vm);
    }
}