using Microsoft.AspNetCore.Mvc;
using SADVO.Application.Interfaces;
using SADVO.Application.ViewModels;
using SADVO.Application.Dtos;
using SADVO.Application.Utils;
using SADVO.Application.Services;

namespace SADVO.Presentation.Controllers
{
    public class CandidatosController : Controller
    {
        private readonly ICandidatoService _service;
        private readonly IEleccionService _eleccionService;
        private readonly IAsignacionDirigenteService _asignacionService;
        private readonly IWebHostEnvironment _hostEnvironment;

        public CandidatosController(ICandidatoService service, IAsignacionDirigenteService asignacionService, IEleccionService eleccionService, IWebHostEnvironment hostEnvironment)
        {
            _service = service;
            _eleccionService = eleccionService;
            _asignacionService = asignacionService;
            _hostEnvironment = hostEnvironment;
        }

        public async Task<IActionResult> Index()
        {
            var usuario = HttpContext.Session.Get<UsuarioDto>("Usuario");
            if (usuario == null)
            {
                return RedirectToAction("Index", "Login");
            }

            var asignaciones = await _asignacionService.GetAllAsync();
            var asignacion = asignaciones.FirstOrDefault(a => a.UsuarioId == usuario.Id);

            if (asignacion == null)
            {
                TempData["ErrorMessage"] = "No tienes un partido asignado. Contacta al administrador.";
                return RedirectToAction("Index", "Home");
            }

            var candidatos = await _service.GetAllAsync();
            var filtrados = candidatos
                .Where(c => c.PartidoPoliticoId == asignacion.PartidoPoliticoId)
                .ToList();

            return View(filtrados);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View(new CandidatoViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> Create(CandidatoViewModel model)
        {
            if (await _eleccionService.HayEleccionActivaAsync())
                return Forbid();

            if (model.FotoFile == null)
            {
                ModelState.AddModelError("FotoFile", "La foto del candidato es obligatoria.");
                return View(model);
            }

            if (!ModelState.IsValid)
                return View(model);

            var usuario = HttpContext.Session.Get<UsuarioDto>("Usuario");
            var asignaciones = await _asignacionService.GetAllAsync();
            var asignacion = asignaciones.FirstOrDefault(a => a.UsuarioId == usuario!.Id);

            if (asignacion == null)
            {
                TempData["ErrorMessage"] = "No tienes un partido asignado. Contacta al administrador.";
                return RedirectToAction("Index");
            }

            model.PartidoPoliticoId = asignacion.PartidoPoliticoId;

            var folder = Path.Combine(_hostEnvironment.WebRootPath, "uploads", "candidatos");
            Directory.CreateDirectory(folder);

            var filename = Guid.NewGuid() + Path.GetExtension(model.FotoFile.FileName);
            var path = Path.Combine(folder, filename);

            using (var fs = new FileStream(path, FileMode.Create))
            {
                await model.FotoFile.CopyToAsync(fs);
            }

            model.FotoUrl = $"/uploads/candidatos/{filename}";

            await _service.CreateAsync(model.ToDto());
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var dto = await _service.GetByIdAsync(id);
            if (dto == null)
                return NotFound();

            return View(CandidatoViewModel.FromDto(dto));
        }

        [HttpPost]
        public async Task<IActionResult> Edit(CandidatoViewModel model)
        {
            if (await _eleccionService.HayEleccionActivaAsync())
                return Forbid();

            if (!ModelState.IsValid)
                return View(model);

            var actual = await _service.GetByIdAsync(model.Id);

            if (model.FotoFile != null && model.FotoFile.Length > 0)
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
            else
            {
                model.FotoUrl = actual?.FotoUrl;
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
