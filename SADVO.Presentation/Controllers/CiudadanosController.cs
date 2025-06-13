using Microsoft.AspNetCore.Mvc;
using SADVO.Application.Dtos;
using SADVO.Application.Interfaces;
using SADVO.Web.ViewModels;

namespace SADVO.Web.Controllers
{
    public class CiudadanosController : Controller
    {
        private readonly ICiudadanoService _service;

        public CiudadanosController(ICiudadanoService service)
        {
            _service = service;
        }

        public async Task<IActionResult> Index()
        {
            var list = await _service.GetAllAsync();
            return View(list);
        }

        public IActionResult Create() => View();

        [HttpPost]
        public async Task<IActionResult> Create(CuidadanoViewModel model)
        {
            if (!ModelState.IsValid) return View(model);
            await _service.CreateAsync(model.ToDto());
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int id)
        {
            var dto = await _service.GetByIdAsync(id);
            if (dto == null) return NotFound();
            return View(CuidadanoViewModel.FromDto(dto));
        }

        [HttpPost]
        public async Task<IActionResult> Edit(CuidadanoViewModel model)
        {
            if (!ModelState.IsValid) return View(model);
            await _service.UpdateAsync(model.ToDto());
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Deactivate(int id)
        {
            await _service.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Activate(int id)
        {
            await _service.ActivateAsync(id);
            return RedirectToAction(nameof(Index));
        }

    }
}
