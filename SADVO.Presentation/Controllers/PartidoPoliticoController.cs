using Microsoft.AspNetCore.Mvc;
using SADVO.Application.Interfaces;
using SADVO.Application.ViewModels;

namespace SADVO.Presentation.Controllers
{
    public class PartidoPoliticoController : Controller
    {
        private readonly IPartidoPoliticoService _service;
        private readonly IEleccionService _eleccionService;

        public PartidoPoliticoController(IPartidoPoliticoService service, IEleccionService eleccionService)
        {
            _service = service;
            _eleccionService = eleccionService;
        }

        public async Task<IActionResult> Index()
        {
            var list = await _service.GetAllAsync();
            return View(list);
        }

        public async Task<IActionResult> Create()
        {
            if (await _eleccionService.HayEleccionActivaAsync())
            return Forbid("No se permite crear un partido político mientras hay una elección activa.");
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(PartidoPoliticoViewModel model)
        {
            if (await _eleccionService.HayEleccionActivaAsync())
                return Forbid("No se permite crear un partido político mientras hay una elección activa.");

            if (!ModelState.IsValid) return View(model);
            await _service.CreateAsync(model.ToDto());
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int id)
        {
            if (await _eleccionService.HayEleccionActivaAsync())
                return Forbid("No se permite editar un partido político mientras hay una elección activa.");

            var dto = await _service.GetByIdAsync(id);
            if (dto == null) return NotFound();
            return View(PartidoPoliticoViewModel.FromDto(dto));
        }

        [HttpPost]
        public async Task<IActionResult> Edit(PartidoPoliticoViewModel model)
        {
            if (await _eleccionService.HayEleccionActivaAsync())
                return Forbid("No se permite editar un partido político mientras hay una elección activa.");

            if (!ModelState.IsValid) return View(model);
            await _service.UpdateAsync(model.ToDto());
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Desactivar(int id)
        {
            if (await _eleccionService.HayEleccionActivaAsync())
                return Forbid("No se permite desactivar un partido político mientras hay una elección activa.");

            await _service.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Activar(int id)
        {
            if (await _eleccionService.HayEleccionActivaAsync())
                return Forbid("No se permite activar un partido político mientras hay una elección activa.");

            await _service.ActivateAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}