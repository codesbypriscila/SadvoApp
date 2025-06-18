using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SADVO.Application.Interfaces;
using SADVO.Application.ViewModels;

public class AlianzasPoliticasController : Controller
{
    private readonly IAlianzaPoliticaService _alianzaService;
    private readonly IPartidoPoliticoService _partidoService;
    private readonly IEleccionService _eleccionService;

    private int PartidoLogueadoId => HttpContext.Session.GetInt32("PartidoId") ?? 0;

    public AlianzasPoliticasController(
        IAlianzaPoliticaService alianzaService,
        IPartidoPoliticoService partidoService,
        IEleccionService eleccionService)
    {
        _alianzaService = alianzaService;
        _partidoService = partidoService;
        _eleccionService = eleccionService;
    }

    public async Task<IActionResult> Index()
    {
        ViewBag.PartidoId = PartidoLogueadoId;
        ViewBag.SolicitudesPendientes = await _alianzaService.ObtenerSolicitudesPendientesAsync(PartidoLogueadoId);
        ViewBag.SolicitudesEnviadas = await _alianzaService.ObtenerSolicitudesEnviadasAsync(PartidoLogueadoId);
        ViewBag.AlianzasActivas = await _alianzaService.ObtenerAlianzasActivasAsync(PartidoLogueadoId);
        ViewBag.HayEleccionActiva = await _eleccionService.HayEleccionActivaAsync();

        return View();
    }

    public async Task<IActionResult> Create()
    {
        var partidos = await _partidoService.GetAllAsync();
        var solicitudesPendientes = await _alianzaService.ObtenerSolicitudesEnviadasAsync(PartidoLogueadoId);
        var solicitudesRecibidas = await _alianzaService.ObtenerSolicitudesPendientesAsync(PartidoLogueadoId);

        var partidosNoDisponibles = solicitudesPendientes
            .Where(a => a.Estado == "EnEspera")
            .Select(a => a.PartidoReceptorId)
            .Concat(solicitudesRecibidas.Select(a => a.PartidoSolicitanteId))
            .ToHashSet();

        var disponibles = partidos
            .Where(p => p.Id != PartidoLogueadoId && p.Activo && !partidosNoDisponibles.Contains(p.Id))
            .Select(p => new SelectListItem
            {
                Value = p.Id.ToString(),
                Text = $"{p.Nombre} ({p.Siglas})"
            }).ToList();

        var model = new AlianzaPoliticaViewModel { PartidosDisponibles = disponibles };
        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> Create(AlianzaPoliticaViewModel model)
    {
        if (PartidoLogueadoId == 0)
        {
            ModelState.AddModelError("", "Error: No se pudo determinar el partido logueado.");
            model.PartidosDisponibles = await GetPartidosDisponibles();
            return View(model);
        }

        if (!ModelState.IsValid)
        {
            model.PartidosDisponibles = await GetPartidosDisponibles();
            return View(model);
        }

        try
        {
            await _alianzaService.CrearSolicitudAsync(PartidoLogueadoId, model.PartidoReceptorId);
            TempData["Mensaje"] = "Solicitud de alianza creada correctamente.";
            return RedirectToAction("Index");
        }
        catch (Exception ex)
        {
            var message = ex.InnerException?.InnerException?.Message ?? ex.InnerException?.Message ?? ex.Message;
            ModelState.AddModelError("", message);
            model.PartidosDisponibles = await GetPartidosDisponibles();
            return View(model);
        }
    }

    private async Task<List<SelectListItem>> GetPartidosDisponibles()
    {
        var partidos = await _partidoService.GetAllAsync();
        var solicitudesPendientes = await _alianzaService.ObtenerSolicitudesEnviadasAsync(PartidoLogueadoId);
        var solicitudesRecibidas = await _alianzaService.ObtenerSolicitudesPendientesAsync(PartidoLogueadoId);

        var partidosNoDisponibles = solicitudesPendientes
            .Where(a => a.Estado == "EnEspera")
            .Select(a => a.PartidoReceptorId)
            .Concat(solicitudesRecibidas.Select(a => a.PartidoSolicitanteId))
            .ToHashSet();

        return partidos
            .Where(p => p.Id != PartidoLogueadoId && p.Activo && !partidosNoDisponibles.Contains(p.Id))
            .Select(p => new SelectListItem
            {
                Value = p.Id.ToString(),
                Text = $"{p.Nombre} ({p.Siglas})"
            }).ToList();
    }

    public async Task<IActionResult> ConfirmarAceptar(int id)
    {
        var solicitud = await _alianzaService.ObtenerPorIdAsync(id);
        return View(new AlianzaPoliticaViewModel
        {
            IdSolicitud = solicitud!.Id,
            PartidoReceptorNombre = solicitud.PartidoReceptorNombre,
            PartidoReceptorSiglas = solicitud.PartidoReceptorSiglas
        });
    }

    [HttpPost]
    public async Task<IActionResult> ConfirmarAceptar(AlianzaPoliticaViewModel model)
    {
        if (model.IdSolicitud.HasValue)
            await _alianzaService.AceptarSolicitudAsync(model.IdSolicitud.Value);

        return RedirectToAction("Index");
    }

    public async Task<IActionResult> ConfirmarRechazar(int id)
    {
        var solicitud = await _alianzaService.ObtenerPorIdAsync(id);
        return View(new AlianzaPoliticaViewModel
        {
            IdSolicitud = solicitud!.Id,
            PartidoReceptorNombre = solicitud.PartidoReceptorNombre,
            PartidoReceptorSiglas = solicitud.PartidoReceptorSiglas
        });
    }

    [HttpPost]
    public async Task<IActionResult> ConfirmarRechazar(AlianzaPoliticaViewModel model)
    {
        if (model.IdSolicitud.HasValue)
            await _alianzaService.RechazarSolicitudAsync(model.IdSolicitud.Value);

        return RedirectToAction("Index");
    }

    public async Task<IActionResult> ConfirmarEliminar(int id)
    {
        var solicitud = await _alianzaService.ObtenerPorIdAsync(id);
        return View(new AlianzaPoliticaViewModel
        {
            IdSolicitud = solicitud!.Id,
            PartidoReceptorNombre = solicitud.PartidoReceptorNombre,
            PartidoReceptorSiglas = solicitud.PartidoReceptorSiglas
        });
    }

    [HttpPost]
    public async Task<IActionResult> ConfirmarEliminar(AlianzaPoliticaViewModel model)
    {
        if (model.IdSolicitud.HasValue)
            await _alianzaService.EliminarSolicitudAsync(model.IdSolicitud.Value);

        return RedirectToAction("Index");
    }
}