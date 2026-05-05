namespace GestaPME.ViewModels.Home;

public class PainelInicialViewModel{
    public string NomeUsuario{ get; set; } = string.Empty;
    public int TotalFuncionarios{ get; set; }
    public int FuncionariosAtivos{ get; set; }
    public int FuncionariosInativos{ get; set; }
    public int TotalDepartamentos{ get; set; }
    public List<ResumoFeriasItem> FeriasEmCurso{ get; set; } = new();
    public List<ResumoFeriasItem> FeriasPendentes{ get; set; } = new();
}

public class ResumoFeriasItem{
    public Guid Id{ get; set; }
    public string Funcionario{ get; set; } = string.Empty;
    public DateTime DataInicio{ get; set; }
    public DateTime DataFim{ get; set; }
    public int Dias{ get; set; }
}