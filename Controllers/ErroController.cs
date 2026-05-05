using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GestaPME.Controllers;

[AllowAnonymous]
[Route("Erro")]
public class ErroController : Controller{
    [Route("404")]
    public IActionResult Erro404() => View("NaoEncontrado");

    [Route("500")]
    public IActionResult Erro500() => View("Error");

    [Route("{codigo:int}")]
    public IActionResult PorCodigo(int codigo) => codigo switch{
        404 => View("NaoEncontrado"),
        500 => View("Error"),
        _ => View("Error")
    };
}