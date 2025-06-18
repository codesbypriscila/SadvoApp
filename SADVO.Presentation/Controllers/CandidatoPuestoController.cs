using Microsoft.AspNetCore.Mvc;
using SADVO.Application.Dtos;
using SADVO.Application.Interfaces;
using SADVO.Application.ViewModels;
using SADVO.Application.Utils;

namespace SADVO.Presentation.Controllers
{
    public class CandidatoPuestoController : Controller
    {
        private readonly ICandidatoPuestoService _service;
        private readonly IAsignacionDirigenteService _asignacionService;
        private readonly IEleccionService _eleccionService;

        public CandidatoPuestoController(
            ICandidatoPuestoService service,
            IAsignacionDirigenteService asignacionService,
            IEleccionService eleccionService)
        {
            _service = service;
            _asignacionService = asignacionService;
            _eleccionService = eleccionService;
        }

        public async Task<IActionResult> Index()
        {
            var usuario = HttpContext.Session.Get<UsuarioDto>("Usuario");
            if (usuario == null)
                return RedirectToAction("Index", "Login");

            var asignacion = (await _asignacionService.GetAllAsync())
                .FirstOrDefault(x => x.UsuarioId == usuario.Id);

            if (asignacion == null)
            {
                TempData["ErrorMessage"] = "No tienes un partido asignado.";
                return RedirectToAction("Index", "Home");
            }

            var relaciones = await _service.GetAllAsync(asignacion.PartidoPoliticoId);
            return View(relaciones);
        }

        public async Task<IActionResult> Create()
        {
            if (await _eleccionService.HayEleccionActivaAsync())
                return Forbid("No se permite asignar candidatos mientras hay una elección activa.");

            var usuario = HttpContext.Session.Get<UsuarioDto>("Usuario");
            if (usuario == null)
                return RedirectToAction("Index", "Login");

            var asignacion = (await _asignacionService.GetAllAsync())
                .FirstOrDefault(x => x.UsuarioId == usuario.Id);

            if (asignacion == null)
            {
                TempData["ErrorMessage"] = "No tienes un partido asignado.";
                return RedirectToAction("Index", "Home");
            }

            var puestos = await _service.ObtenerPuestosDisponiblesAsync(asignacion.PartidoPoliticoId);
            var primerPuestoId = puestos.FirstOrDefault()?.Id ?? 0;

            var model = new CandidatoPuestoViewModel
            {
                PartidoPoliticoId = asignacion.PartidoPoliticoId,
                PuestosDisponibles = puestos,
                CandidatosDisponibles = await _service.ObtenerCandidatosDisponiblesAsync(asignacion.PartidoPoliticoId, primerPuestoId)
            };

            return View(model);
        }


        [HttpPost]
        public async Task<IActionResult> Create(CandidatoPuestoViewModel model)
        {
            if (await _eleccionService.HayEleccionActivaAsync())
                return Forbid("No se permite asignar candidatos mientras hay una elección activa.");

            if (!ModelState.IsValid)
            {
                model.CandidatosDisponibles = await _service.ObtenerCandidatosDisponiblesAsync(model.PartidoPoliticoId, model.PuestoElectivoId);
                model.PuestosDisponibles = await _service.ObtenerPuestosDisponiblesAsync(model.PartidoPoliticoId);
                return View(model);
            }

            try
            {
                await _service.CreateAsync(model.ToDto());
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                model.CandidatosDisponibles = await _service.ObtenerCandidatosDisponiblesAsync(model.PartidoPoliticoId, model.PuestoElectivoId);
                model.PuestosDisponibles = await _service.ObtenerPuestosDisponiblesAsync(model.PartidoPoliticoId);
                return View(model);
            }
        }

        public async Task<IActionResult> Delete(int id)
        {
            var usuario = HttpContext.Session.Get<UsuarioDto>("Usuario");
            if (usuario == null)
                return RedirectToAction("Index", "Login");

            var asignacion = (await _asignacionService.GetAllAsync())
                .FirstOrDefault(x => x.UsuarioId == usuario.Id);

            if (asignacion == null)
            {
                TempData["ErrorMessage"] = "No tienes un partido asignado.";
                return RedirectToAction("Index", "Home");
            }

            var relaciones = await _service.GetAllAsync(asignacion.PartidoPoliticoId);
            var dto = relaciones.FirstOrDefault(x => x.Id == id);
            if (dto == null)
                return NotFound();

            return View(dto);
        }

        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (await _eleccionService.HayEleccionActivaAsync())
                return Forbid("No se permite eliminar asignaciones mientras hay una elección activa.");

            await _service.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
