using Microsoft.AspNetCore.Mvc;
using SADVO.Application.Dtos;
using SADVO.Application.Interfaces;
using SADVO.Application.ViewModels;
using SADVO.Application.Utils;

namespace SADVO.Presentation.Controllers
{
    public class UsuariosController : Controller
    {
        private readonly IUsuarioService _service;
        private readonly IEleccionService _eleccionService;
        private readonly IRolService _rolService;

        public UsuariosController(
            IUsuarioService service, 
            IEleccionService eleccionService,
            IRolService rolService)
        {
            _service = service;
            _eleccionService = eleccionService;
            _rolService = rolService;
        }

        public async Task<IActionResult> Index()
        {
            var usuario = HttpContext.Session.Get<UsuarioDto>("Usuario");
            if (usuario == null)
            {
                return RedirectToAction("Index", "Login");
            }
            var usuarios = await _service.GetAllAsync();
            return View(usuarios);
        }

        public async Task<IActionResult> Create()
        {
            if (await _eleccionService.HayEleccionActivaAsync())
                return RedirectToAction("Index");

            var model = new UsuarioViewModel
            {
                RolesDisponibles = await _rolService.GetRolesDisponiblesAsync()
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(UsuarioViewModel model)
        {
            if (await _eleccionService.HayEleccionActivaAsync())
                return RedirectToAction("Index");

            if (!ModelState.IsValid)
            {
                model.RolesDisponibles = await _rolService.GetRolesDisponiblesAsync();
                return View(model);
            }

            if (await _service.IsUsernameTakenAsync(model.Username))
            {
                ModelState.AddModelError("Username", "El nombre de usuario ya existe.");
                model.RolesDisponibles = await _rolService.GetRolesDisponiblesAsync();
                return View(model);
            }

            try
            {
                await _service.CreateAsync(model.ToDto(), model.Password!);
                return RedirectToAction("Index");
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError("", ex.Message);
                model.RolesDisponibles = await _rolService.GetRolesDisponiblesAsync();
                return View(model);
            }
        }

        public async Task<IActionResult> Edit(int id)
        {
            if (await _eleccionService.HayEleccionActivaAsync())
                return RedirectToAction("Index");

            var usuarioDto = await _service.GetByIdAsync(id);
            if (usuarioDto == null)
                return NotFound();

            var model = UsuarioViewModel.FromDto(usuarioDto);
            model.RolesDisponibles = await _rolService.GetRolesDisponiblesAsync();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, UsuarioViewModel model)
        {
            if (await _eleccionService.HayEleccionActivaAsync())
                return RedirectToAction("Index");

            if (!ModelState.IsValid)
            {
                model.RolesDisponibles = await _rolService.GetRolesDisponiblesAsync();
                return View(model);
            }

            if (await _service.IsUsernameTakenAsync(model.Username, excludeUserId: id))
            {
                ModelState.AddModelError("Username", "El nombre de usuario ya está en uso.");
                model.RolesDisponibles = await _rolService.GetRolesDisponiblesAsync();
                return View(model);
            }

            try
            {
                await _service.UpdateAsync(id, model.ToDto(), model.Password);
                return RedirectToAction("Index");
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError("", ex.Message);
                model.RolesDisponibles = await _rolService.GetRolesDisponiblesAsync();
                return View(model);
            }
        }

        public async Task<IActionResult> Deactivate(int id)
        {
            if (await _eleccionService.HayEleccionActivaAsync())
                return Forbid("No se permite desactivar un usuario mientras hay una elección activa.");

            await _service.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Activate(int id)
        {
            if (await _eleccionService.HayEleccionActivaAsync())
                return Forbid("No se permite activar un usuario mientras hay una elección activa.");

            await _service.ActivateAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}