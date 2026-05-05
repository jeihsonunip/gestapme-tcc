using System.ClientModel;
using OpenAI;
using OpenAI.Chat;
using Microsoft.EntityFrameworkCore;
using GestaPME.Data;

namespace GestaPME.Services;

public class AssistenteService{
    private readonly AppDbContext _context;
    private readonly IConfiguration _config;

    public AssistenteService(AppDbContext context, IConfiguration config){
        _context = context;
        _config = config;
    }

    public async Task<string> ProcessarPerguntaAsync(string pergunta, Guid empresaId){
        // 1. Coletar dados relevantes da empresa
        var contextoEmpresa = await ColetarContextoAsync(empresaId);

        // 2. Montar o prompt com contexto
        var systemPrompt = $"""
                            Você é um assistente inteligente de gestão empresarial. 
                            Responda perguntas sobre a empresa e seus funcionários com base APENAS nos dados fornecidos abaixo.
                            Se a informação não estiver disponível nos dados, informe que não possui essa informação.
                            Responda sempre em português brasileiro, de forma clara e objetiva.

                            === DADOS DA EMPRESA ===
                            {contextoEmpresa}
                            """;

        // 3. Chamar provedor de IA (OpenAI-compatible — GitHub Models, Azure OpenAI ou OpenAI direto)
        var endpoint = _config["AssistenteIA:Endpoint"]!;
        var apiKey = _config["AssistenteIA:ApiKey"]!;
        var modelo = _config["AssistenteIA:Modelo"]!;

        var client = new OpenAIClient(
            new ApiKeyCredential(apiKey),
            new OpenAIClientOptions{ Endpoint = new Uri(endpoint) });

        var chatClient = client.GetChatClient(modelo);

        var messages = new List<ChatMessage>{
            new SystemChatMessage(systemPrompt),
            new UserChatMessage(pergunta)
        };

        var options = new ChatCompletionOptions{
            MaxOutputTokenCount = 1000,
            Temperature = 0.3f
        };

        var response = await chatClient.CompleteChatAsync(messages, options);

        return response.Value.Content[0].Text;
    }

    private async Task<string> ColetarContextoAsync(Guid empresaId){
        var empresa = await _context.Empresas
            .FirstOrDefaultAsync(e => e.Id == empresaId);

        if (empresa == null) return "Empresa não encontrada.";

        var departamentos = await _context.Departamentos
            .Where(d => d.EmpresaId == empresaId)
            .Select(d => new{ d.Id, d.Nome })
            .ToListAsync();

        var cargos = await _context.Cargos
            .Where(c => c.EmpresaId == empresaId)
            .Select(c => new{ c.Id, c.Nome })
            .ToListAsync();

        var funcionarios = await _context.Funcionarios
            .Where(f => f.EmpresaId == empresaId)
            .Include(f => f.Departamento)
            .Include(f => f.Cargo)
            .Select(f => new{
                f.NomeCompleto,
                f.Email,
                f.DataAdmissao,
                f.DataDesligamento,
                f.Ativo,
                Departamento = f.Departamento.Nome,
                Cargo = f.Cargo.Nome
            })
            .ToListAsync();

        var ferias = await _context.Ferias
            .Where(f => f.Funcionario.EmpresaId == empresaId)
            .Include(f => f.Funcionario)
            .Select(f => new{
                Funcionario = f.Funcionario.NomeCompleto,
                f.DataInicio,
                f.DataFim,
                f.Status
            })
            .ToListAsync();

        var totalAtivos = funcionarios.Count(f => f.Ativo);
        var totalInativos = funcionarios.Count(f => !f.Ativo);

        var texto = $"""
                     Empresa: {empresa.RazaoSocial} ({empresa.NomeFantasia ?? "sem nome fantasia"})
                     CNPJ: {empresa.CNPJ}
                     Total de funcionários ativos: {totalAtivos}
                     Total de funcionários inativos: {totalInativos}

                     Departamentos ({departamentos.Count}):
                     {string.Join("\n", departamentos.Select(d => $"- {d.Nome}"))}

                     Cargos ({cargos.Count}):
                     {string.Join("\n", cargos.Select(c => $"- {c.Nome}"))}

                     Funcionários:
                     {string.Join("\n", funcionarios.Select(f =>
                         $"- {f.NomeCompleto} | Cargo: {f.Cargo} | Depto: {f.Departamento} | " +
                         $"Admissão: {f.DataAdmissao:dd/MM/yyyy} | Status: {(f.Ativo ? "Ativo" : "Inativo")}"))}

                     Férias registradas:
                     {string.Join("\n", ferias.Select(f =>
                         $"- {f.Funcionario}: {f.DataInicio:dd/MM/yyyy} a {f.DataFim:dd/MM/yyyy} | Status: {f.Status}"))}
                     """;

        return texto;
    }
}