namespace GestaPME.Services;

public interface IContextoEmpresa{
    Guid EmpresaId{ get; }
    Guid UsuarioId{ get; }
    string NomeUsuario{ get; }
    string EmailUsuario{ get; }
    bool EhAdministrador{ get; }
    bool EstaAutenticado{ get; }
}