using Microsoft.AspNetCore.Mvc;
using SADVO.Application.Dtos;
using SADVO.Application.Interfaces;
using SADVO.Application.ViewModels;
using SADVO.Application.Utils;

namespace SADVO.Presentation.Controllers
{
    public class PuestoElectivoController : Controller
    {
        private readonly IPuestoElectivoService _service;
        private readonly IEleccionService _eleccionService;

        public PuestoElectivoController(IPuestoElectivoService service, IEleccionService eleccionService)
        {
            _service = service;
            _eleccionService = eleccionService;
        }

        public async Task<IActionResult> Index()
        {
            var usuario = HttpContext.Session.Get<UsuarioDto>("Usuario");
            if (usuario == null)
            {
                return RedirectToAction("Index", "Login");
            }
            var list = await _service.GetAllAsync();
            return View(list);
        }

        public async Task<IActionResult> Create()
        {
            if (await _eleccionService.HayEleccionActivaAsync())
                return Forbid("No se permite crear un puesto mientras hay una elección activa.");
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(PuestoElectivoViewModel model)
        {
            if (await _eleccionService.HayEleccionActivaAsync())
                return Forbid("No se permite crear un puesto mientras hay una elección activa.");

            if (!ModelState.IsValid) return View(model);
            await _service.CreateAsync(model.ToDto());
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int id)
        {
            if (await _eleccionService.HayEleccionActivaAsync())
                return Forbid("No se permite editar un puesto mientras hay una elección activa.");

            var dto = await _service.GetByIdAsync(id);
            if (dto == null) return NotFound();
            return View(PuestoElectivoViewModel.FromDto(dto));
        }

        [HttpPost]
        public async Task<IActionResult> Edit(PuestoElectivoViewModel model)
        {
            if (await _eleccionService.HayEleccionActivaAsync())
                return Forbid("No se permite editar un puesto mientras hay una elección activa.");

            if (!ModelState.IsValid) return View(model);
            await _service.UpdateAsync(model.ToDto());
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Deactivate(int id)
        {
            if (await _eleccionService.HayEleccionActivaAsync())
                return Forbid("No se permite desactivar un puesto mientras hay una elección activa.");

            await _service.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Activate(int id)
        {
            if (await _eleccionService.HayEleccionActivaAsync())
                return Forbid("No se permite activar un puesto mientras hay una elección activa.");

            await _service.ActivateAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
