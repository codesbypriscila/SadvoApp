using Microsoft.AspNetCore.Mvc;
using SADVO.Application.Dtos;
using SADVO.Application.Utils;

namespace SADVO.Presentation.Controllers;

public class AdminController : Controller
{
    public IActionResult Index()
    {
        var usuario = HttpContext.Session.Get<UsuarioDto>("Usuario");

        if (usuario == null)
        {
            return RedirectToAction("Index", "Login");
        }

        return View(usuario);
    }
}
