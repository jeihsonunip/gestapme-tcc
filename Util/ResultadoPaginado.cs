using Microsoft.EntityFrameworkCore;

namespace GestaPME.Util;

public class ResultadoPaginado<T>{
    public List<T> Itens{ get; set; } = new();
    public int PaginaAtual{ get; set; }
    public int TotalPaginas{ get; set; }
    public int TotalItens{ get; set; }
    public int TamanhoPagina{ get; set; }
    public bool TemAnterior => PaginaAtual > 1;
    public bool TemProxima => PaginaAtual < TotalPaginas;

    public static async Task<ResultadoPaginado<T>> CriarAsync(IQueryable<T> query, int pagina, int tamanhoPagina){
        var total = await query.CountAsync();
        var itens = await query.Skip((pagina - 1) * tamanhoPagina).Take(tamanhoPagina).ToListAsync();
        return new ResultadoPaginado<T>{
            Itens = itens,
            PaginaAtual = pagina,
            TamanhoPagina = tamanhoPagina,
            TotalItens = total,
            TotalPaginas = (int)Math.Ceiling(total / (double)tamanhoPagina)
        };
    }
}