using Microsoft.AspNetCore.Mvc;
using SADVO.Application.Dtos;
using SADVO.Application.Interfaces;
using SADVO.Application.Utils;

public class DirigenteController : Controller
{
    private readonly IAsignacionDirigenteService _asignacionService;

    public DirigenteController(IAsignacionDirigenteService asignacionService)
    {
        _asignacionService = asignacionService;
    }

    public async Task<IActionResult> Index()
    {
        var usuario = HttpContext.Session.Get<UsuarioDto>("Usuario");

        if (usuario == null)
        {
            return RedirectToAction("Index", "Login");
        }

        var asignaciones = await _asignacionService.GetAllAsync();
        var asignacionUsuario = asignaciones.FirstOrDefault(a => a.UsuarioId == usuario.Id);
        ViewBag.PartidoSiglas = asignacionUsuario?.PartidoPoliticoSiglas ?? "Sin asignación";

        return View(usuario);
    }
}
