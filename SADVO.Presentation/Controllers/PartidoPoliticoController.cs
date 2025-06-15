using Microsoft.AspNetCore.Mvc;
using SADVO.Application.Interfaces;
using SADVO.Application.ViewModels;


namespace SADVO.Presentation.Controllers
{
    public class PartidosPoliticosController : Controller
    {
        private readonly IPartidoPoliticoService _service;
        private readonly IEleccionService _eleccionService;
        private readonly IWebHostEnvironment _hostEnvironment;

        public PartidosPoliticosController(
            IPartidoPoliticoService service,
            IEleccionService eleccionService,
            IWebHostEnvironment hostEnvironment)
        {
            _service = service;
            _eleccionService = eleccionService;
            _hostEnvironment = hostEnvironment;
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
            return View(new PartidoPoliticoViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> Create(PartidoPoliticoViewModel model)
        {
            if (await _eleccionService.HayEleccionActivaAsync())
                return Forbid("No se permite crear un partido político mientras hay una elección activa.");

            if (!ModelState.IsValid)
                return View(model);

            if (model.LogoFile != null && model.LogoFile.Length > 0)
            {
                var uploadsFolder = Path.Combine(_hostEnvironment.WebRootPath, "uploads", "partidos");
                Directory.CreateDirectory(uploadsFolder);

                var safeFileName = Guid.NewGuid().ToString() + Path.GetExtension(model.LogoFile.FileName);
                var filePath = Path.Combine(uploadsFolder, safeFileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await model.LogoFile.CopyToAsync(fileStream);
                }

                model.LogoUrl = $"/uploads/partidos/{safeFileName}";
            }
            else
            {
                model.LogoUrl = null;
            }

            await _service.CreateAsync(model.ToDto());
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int id)
        {
            var dto = await _service.GetByIdAsync(id);
            if (dto == null)
                return NotFound();

            return View(PartidoPoliticoViewModel.FromDto(dto));
        }

        [HttpPost]
        public async Task<IActionResult> Edit(PartidoPoliticoViewModel model)
        {
            if (await _eleccionService.HayEleccionActivaAsync())
                return Forbid("No se permite editar un partido político mientras hay una elección activa.");

            if (!ModelState.IsValid)
                return View(model);

            var partidoActual = await _service.GetByIdAsync(model.Id);

            if (model.LogoFile != null && model.LogoFile.Length > 0)
            {
                var uploadsFolder = Path.Combine(_hostEnvironment.WebRootPath, "uploads", "partidos");
                Directory.CreateDirectory(uploadsFolder);

                var safeFileName = Guid.NewGuid().ToString() + Path.GetExtension(model.LogoFile.FileName);
                var filePath = Path.Combine(uploadsFolder, safeFileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await model.LogoFile.CopyToAsync(fileStream);
                }

                model.LogoUrl = $"/uploads/partidos/{safeFileName}";
            }
            else
            {
                model.LogoUrl = partidoActual?.LogoUrl;
            }

            await _service.UpdateAsync(model.ToDto());
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Deactivate(int id)
        {
            if (await _eleccionService.HayEleccionActivaAsync())
                return Forbid("No se permite desactivar un partido político mientras hay una elección activa.");

            await _service.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Activate(int id)
        {
            if (await _eleccionService.HayEleccionActivaAsync())
                return Forbid("No se permite activar un partido político mientras hay una elección activa.");

            await _service.ActivateAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}