using Microsoft.AspNetCore.Mvc;
using SADVO.Application.Interfaces;
using SADVO.Application.ViewModels;
using SADVO.Application.Dtos;
using SADVO.Application.Utils;

namespace SADVO.Presentation.Controllers
{
    public class CandidatosController : Controller
    {
        private readonly ICandidatoService _service;
        private readonly IEleccionService _eleccionService;
        private readonly IWebHostEnvironment _hostEnvironment;

        public CandidatosController(ICandidatoService service, IEleccionService eleccionService, IWebHostEnvironment hostEnvironment)
        {
            _service = service;
            _eleccionService = eleccionService;
            _hostEnvironment = hostEnvironment;
        }

        public async Task<IActionResult> Index()
        {
            var usuario = HttpContext.Session.Get<CandidatoDto>("Usuario");
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
                return Forbid("No se puede crear candidato durante una elección activa.");
            
            return View(new CandidatoViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> Create(CandidatoViewModel model)
        {
            if (await _eleccionService.HayEleccionActivaAsync())
                return Forbid();

            if (!ModelState.IsValid)
                return View(model);

            if (model.FotoFile != null)
            {
                var folder = Path.Combine(_hostEnvironment.WebRootPath, "uploads", "candidatos");
                Directory.CreateDirectory(folder);

                var filename = Guid.NewGuid() + Path.GetExtension(model.FotoFile.FileName);
                var path = Path.Combine(folder, filename);

                using (var fs = new FileStream(path, FileMode.Create))
                {
                    await model.FotoFile.CopyToAsync(fs);
                }

                model.FotoUrl = $"/uploads/candidatos/{filename}";
            }

            await _service.CreateAsync(model.ToDto());
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int id)
        {
            var dto = await _service.GetByIdAsync(id);
            if (dto == null) return NotFound();
            return View(CandidatoViewModel.FromDto(dto));
        }

        [HttpPost]
        public async Task<IActionResult> Edit(CandidatoViewModel model)
        {
            if (await _eleccionService.HayEleccionActivaAsync())
                return Forbid();

            if (!ModelState.IsValid)
                return View(model);

            if (model.FotoFile != null)
            {
                var folder = Path.Combine(_hostEnvironment.WebRootPath, "uploads", "candidatos");
                Directory.CreateDirectory(folder);

                var filename = Guid.NewGuid() + Path.GetExtension(model.FotoFile.FileName);
                var path = Path.Combine(folder, filename);

                using (var fs = new FileStream(path, FileMode.Create))
                {
                    await model.FotoFile.CopyToAsync(fs);
                }

                model.FotoUrl = $"/uploads/candidatos/{filename}";
            }

            await _service.UpdateAsync(model.ToDto());
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Deactivate(int id)
        {
            if (await _eleccionService.HayEleccionActivaAsync())
                return Forbid();

            await _service.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Activate(int id)
        {
            if (await _eleccionService.HayEleccionActivaAsync())
                return Forbid();

            await _service.ActivateAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
