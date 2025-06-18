using Microsoft.AspNetCore.Mvc;
using SADVO.Application.Interfaces;
using SADVO.Application.ViewModels;
using SADVO.Application.Utils;

namespace SADVO.Presentation.Controllers
{
    public class LoginController : Controller
    {
        private readonly ILoginService _loginService;
        private readonly IDirigenteService _dirigenteService;

        public LoginController(ILoginService loginService, IDirigenteService dirigenteService)
        {
            _loginService = loginService;
            _dirigenteService = dirigenteService;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View(new LoginViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> Index(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                var usuario = await _loginService.LoginAsync(model);
                if (usuario == null)
                {
                    model.ErrorLogin = "Credenciales inválidas";
                    return View(model);
                }

                if (usuario.Rol == "Dirigente")
                {
                    var dirigente = await _dirigenteService.ObtenerDirigentePorUsuarioIdAsync(usuario.Id);
                    if (dirigente == null || dirigente.PartidoPoliticoId == null)
                    {
                        model.ErrorLogin = "Este dirigente no está asignado a ningún partido político.";
                        return View(model);
                    }

                    usuario.PartidoPoliticoId = dirigente.PartidoPoliticoId;
                    HttpContext.Session.SetInt32("PartidoId", dirigente.PartidoPoliticoId.Value);
                }

                HttpContext.Session.Set("Usuario", usuario);

                return usuario.Rol switch
                {
                    "Administrador" => RedirectToAction("Index", "Admin"),
                    "Dirigente" => RedirectToAction("Index", "Dirigente"),
                    _ => RedirectToAction("Index")
                };
            }
            catch (ApplicationException ex)
            {
                model.ErrorLogin = ex.Message;
                return View(model);
            }
        }

        [HttpGet]
        public ActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Home");
        }
    }
}
