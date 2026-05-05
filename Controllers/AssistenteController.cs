using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GestaPME.Controllers;

[Authorize]
public class AssistenteController : Controller{
    public IActionResult Index() => View();
}