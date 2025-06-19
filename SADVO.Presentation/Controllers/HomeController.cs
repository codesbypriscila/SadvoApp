using Microsoft.AspNetCore.Mvc;
using SADVO.Application.Interfaces;
using SADVO.Application.ViewModels;
using SADVO.Application.Dtos;
using SADVO.Domain.Entities.Administrador;
using SADVO.Shared.Interfaces;
using System.Diagnostics;
using SADVO.Presentation.Models;
using SADVO.Domain.Interfaces;

namespace SADVO.Presentation.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IVotoService _votoService;
        private readonly IGenericRepository<Ciudadano> _ciudadanoRepo;
        private readonly IServicioCorreo _servicioCorreo;

        public HomeController(ILogger<HomeController> logger, IVotoService votoService, IGenericRepository<Ciudadano> ciudadanoRepo, IServicioCorreo servicioCorreo)
        {
            _logger = logger;
            _votoService = votoService;
            _ciudadanoRepo = ciudadanoRepo;
            _servicioCorreo = servicioCorreo;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View(new CedulaViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> Vote(CedulaViewModel model)
        {
            if (!ModelState.IsValid)
                return View("Index", model);

            var resultado = await _votoService.ValidarCedulaInicialAsync(model.Cedula);
            if (resultado != "OK")
            {
                TempData["Error"] = resultado;
                return RedirectToAction("Index");
            }

            return RedirectToAction("ValidarIdentidad", new { cedula = model.Cedula });
        }

        [HttpGet]
        public IActionResult ValidarIdentidad(string cedula)
        {
            return View(new ValidarIdentidadViewModel { CedulaDigitada = cedula });
        }

        [HttpPost]
        public async Task<IActionResult> ValidarIdentidad(ValidarIdentidadViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var texto = await _votoService.ProcesarOCRAsync(model.ImagenCedula);

            var textoLimpio = new string(texto.Where(char.IsDigit).ToArray());

            var cedulaSinGuiones = model.CedulaDigitada.Replace("-", "");

            if (!textoLimpio.Contains(cedulaSinGuiones))
            {
                TempData["Error"] = "La cédula no coincide con la imagen";
                return RedirectToAction("ValidarIdentidad", new { cedula = model.CedulaDigitada });
            }

            var ciudadano = (await _ciudadanoRepo.FindAsync(c => c.Cedula == model.CedulaDigitada)).FirstOrDefault();
            return RedirectToAction("SeleccionarCandidatos", new { ciudadanoId = ciudadano!.Id });
        }

        [HttpGet]
        public async Task<IActionResult> SeleccionarCandidatos(int ciudadanoId)
        {
            var viewModels = await _votoService.ObtenerPuestosYOpcionesAsync(ciudadanoId);
            ViewBag.CiudadanoId = ciudadanoId;
            return View(viewModels);
        }

        [HttpPost]
        public async Task<IActionResult> FinalizarVotacion(List<VotoDto> votos, int ciudadanoId)
        {
            if (votos == null || votos.Count == 0 || votos.Any(v => v.CandidatoId == 0))
            {
                TempData["Error"] = "Debes seleccionar un candidato por cada puesto.";
                return RedirectToAction("SeleccionarCandidatos", new { ciudadanoId });
            }

            var resultado = await _votoService.RegistrarVotoAsync(votos, ciudadanoId);
            if (resultado != "OK")
            {
                TempData["Error"] = resultado;
                return RedirectToAction("SeleccionarCandidatos", new { ciudadanoId });
            }

            var ciudadano = (await _ciudadanoRepo.FindAsync(c => c.Id == ciudadanoId)).FirstOrDefault();
            if (ciudadano != null && !string.IsNullOrWhiteSpace(ciudadano.Email))
            {
                var cuerpo = "<p>Gracias por tu voto. El proceso se ha completado correctamente.</p>";
                await _servicioCorreo.EnviarCorreoAsync(ciudadano.Email, "Resumen de tu votación", cuerpo);
            }

            return RedirectToAction("Gracias");
        }

        [HttpGet]
        public IActionResult Gracias()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}