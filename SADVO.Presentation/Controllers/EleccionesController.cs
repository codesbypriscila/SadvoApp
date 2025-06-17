using Microsoft.AspNetCore.Mvc;
using SADVO.Application.Dtos;
using SADVO.Application.Interfaces;
using SADVO.Application.ViewModels;
using SADVO.Application.Utils;

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
            var usuario = HttpContext.Session.Get<UsuarioDto>("Usuario");
            if (usuario == null)
                return RedirectToAction("Index", "Login");

            var list = await _service.GetAllAsync();
            var modelList = list.Select(EleccionViewModel.FromDto);
            return View(modelList);
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
            {
                ModelState.AddModelError("", "Ya existe una elección activa. Finalice la actual antes de crear una nueva.");
                return View(model);
            }

            if (!ModelState.IsValid) return View(model);

            try
            {
                await _service.CreateAsync(model.ToDto());
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View(model);
            }
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

        public async Task<IActionResult> ConfirmarFinalizacion(int id)
        {
            var eleccion = await _service.GetByIdAsync(id);
            if (eleccion == null || !eleccion.Activa) return NotFound();

            var model = EleccionViewModel.FromDto(eleccion);
            return View("ConfirmarFinalizacion", model);
        }

        [HttpPost]
        public async Task<IActionResult> Finalizar(int id)
        {
            await _service.FinalizarAsync(id);
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Resultados(int id)
        {
            var resultado = await _service.ObtenerResultadosAsync(id);
            if (resultado == null)
                return NotFound("La elección no está finalizada o no existe.");

            return View("Resultados", resultado);
        }

        public async Task<IActionResult> Details(int id)
        {
            var dto = await _service.GetByIdAsync(id);
            if (dto == null) return NotFound();
            return View(EleccionViewModel.FromDto(dto));
        }
    }
}
