using System.Security.Claims;

namespace GestaPME.Services;

public class ContextoEmpresa : IContextoEmpresa{
    private readonly IHttpContextAccessor _accessor;

    public ContextoEmpresa(IHttpContextAccessor accessor) => _accessor = accessor;

    private ClaimsPrincipal? User => _accessor.HttpContext?.User;

    public bool EstaAutenticado => User?.Identity?.IsAuthenticated == true;

    public Guid EmpresaId =>
        Guid.TryParse(User?.FindFirstValue("EmpresaId"), out var id)
            ? id
            : throw new InvalidOperationException("Usuário não possui EmpresaId na sessão.");

    public Guid UsuarioId =>
        Guid.TryParse(User?.FindFirstValue(ClaimTypes.NameIdentifier), out var id)
            ? id
            : throw new InvalidOperationException("Usuário não possui NameIdentifier na sessão.");

    public string NomeUsuario => User?.FindFirstValue(ClaimTypes.Name) ?? string.Empty;

    public string EmailUsuario => User?.FindFirstValue(ClaimTypes.Email) ?? string.Empty;

    public bool EhAdministrador => User?.IsInRole("Administrador") == true;
}