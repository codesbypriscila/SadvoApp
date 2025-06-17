using Microsoft.AspNetCore.Mvc;
using SADVO.Application.Interfaces;
using SADVO.Application.ViewModels;
using SADVO.Application.Dtos;
using SADVO.Application.Utils;

namespace SADVO.Presentation.Controllers
{
    public class AsignacionDirigentesController : Controller
    {
        private readonly IAsignacionDirigenteService _service;
        private readonly IUsuarioService _usuarioService; 
        private readonly IPartidoPoliticoService _partidoService;
        private readonly IEleccionService _eleccionService;

        public AsignacionDirigentesController(
            IAsignacionDirigenteService service,
            IUsuarioService usuarioService,
            IPartidoPoliticoService partidoService,
            IEleccionService eleccionService)
        {
            _service = service;
            _usuarioService = usuarioService;
            _partidoService = partidoService;
            _eleccionService = eleccionService;
        }

        public async Task<IActionResult> Index()
        {
            var usuario = HttpContext.Session.Get<UsuarioDto>("Usuario");
            if (usuario == null)
                return RedirectToAction("Index", "Login");

            var list = await _service.GetAllAsync();
            return View(list);
        }

        public async Task<IActionResult> Create()
        {
            if (await _eleccionService.HayEleccionActivaAsync())
                return Forbid("No se permite asignar dirigentes mientras hay una elección activa.");

            var usuarios = await _usuarioService.GetUsuariosDirigentesDisponiblesAsync();
            var partidos = await _partidoService.GetAllAsync();

            ViewBag.Usuarios = usuarios;
            ViewBag.Partidos = partidos;

            return View(new AsignacionDirigenteViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> Create(AsignacionDirigenteViewModel model)
        {
            if (await _eleccionService.HayEleccionActivaAsync())
                return Forbid("No se permite asignar dirigentes mientras hay una elección activa.");

            if (!ModelState.IsValid)
            {
                ViewBag.Usuarios = await _usuarioService.GetUsuariosDirigentesDisponiblesAsync();
                ViewBag.Partidos = await _partidoService.GetAllAsync();
                return View(model);
            }

            try
            {
                var dto = new AsignacionDirigenteCreateDto
                {
                    UsuarioId = model.UsuarioId,
                    PartidoPoliticoId = model.PartidoPoliticoId
                };
                await _service.CreateAsync(dto);
                return RedirectToAction(nameof(Index));
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                ViewBag.Usuarios = await _usuarioService.GetUsuariosDirigentesDisponiblesAsync();
                ViewBag.Partidos = await _partidoService.GetAllAsync();
                return View(model);
            }
        }

        public async Task<IActionResult> Delete(int id)
        {
            var asignacion = await _service.GetByIdAsync(id);
            if (asignacion == null)
                return NotFound();

            return View(asignacion);
        }

        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (await _eleccionService.HayEleccionActivaAsync())
                return Forbid("No se permite eliminar asignaciones mientras hay una elección activa.");

            await _service.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Activate(int id)
        {
            if (await _eleccionService.HayEleccionActivaAsync())
                return Forbid("No se permite activar asignaciones mientras hay una elección activa.");

            await _service.ActivateAsync(id);
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Deactivate(int id)
        {
            if (await _eleccionService.HayEleccionActivaAsync())
                return Forbid("No se permite desactivar asignaciones mientras hay una elección activa.");

            await _service.DeactivateAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
