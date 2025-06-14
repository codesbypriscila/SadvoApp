using Microsoft.AspNetCore.Mvc;
using SADVO.Application.Interfaces;
using SADVO.Application.ViewModels;

namespace SADVO.Presentation.Controllers
{
    public class EleccionesController : Controller
    {
        private readonly IEleccionService _service;

        public EleccionesController(IEleccionService service)
        {
            _service = service;
        }

        public async Task<IActionResult> Index()
        {
            var list = await _service.GetAllAsync();
            var modelList = list.Select(EleccionViewModel.FromDto);
            return View(modelList);
        }

        public async Task<IActionResult> Details(int id)
        {
            var dto = await _service.GetByIdAsync(id);
            if (dto == null) return NotFound();
            return View(EleccionViewModel.FromDto(dto));
        }

        public async Task<IActionResult> Create()
        {
            if (await _service.HayEleccionActivaAsync())
                return Forbid("Ya existe una elección activa. Finalice la actual antes de crear una nueva.");

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(EleccionViewModel model)
        {
            if (await _service.HayEleccionActivaAsync())
                return Forbid("Ya existe una elección activa. Finalice la actual antes de crear una nueva.");

            if (!ModelState.IsValid) return View(model);

            await _service.CreateAsync(model.ToDto());
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int id)
        {
            var dto = await _service.GetByIdAsync(id);
            if (dto == null) return NotFound();
            return View(EleccionViewModel.FromDto(dto));
        }

        [HttpPost]
        public async Task<IActionResult> Edit(EleccionViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            await _service.UpdateAsync(model.ToDto());
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Finalizar(int id)
        {
            await _service.FinalizarAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
