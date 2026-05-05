using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using GestaPME.Services;

namespace GestaPME.Controllers.Api;

[ApiController]
[Authorize]
[IgnoreAntiforgeryToken]
[Route("api/Assistente")]
public class AssistenteApiController : ControllerBase{
    private readonly AssistenteService _assistente;
    private readonly IContextoEmpresa _ctx;

    public AssistenteApiController(AssistenteService assistente, IContextoEmpresa ctx){
        _assistente = assistente;
        _ctx = ctx;
    }

    [HttpPost]
    public async Task<IActionResult> Perguntar([FromBody] PerguntaRequest req){
        if (string.IsNullOrWhiteSpace(req.Pergunta))
            return BadRequest(new{ message = "A pergunta não pode ser vazia." });
        try{
            var resposta = await _assistente.ProcessarPerguntaAsync(req.Pergunta, _ctx.EmpresaId);
            return Ok(new{ pergunta = req.Pergunta, resposta });
        }
        catch (Exception ex){
            return StatusCode(500, new{ message = "Erro ao processar a pergunta.", error = ex.Message });
        }
    }
}

public record PerguntaRequest(string Pergunta);