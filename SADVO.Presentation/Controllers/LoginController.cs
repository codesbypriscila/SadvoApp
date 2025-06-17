using Microsoft.AspNetCore.Mvc;
using SADVO.Application.Interfaces;
using SADVO.Application.ViewModels;
using SADVO.Application.Utils;

namespace SADVO.Presentation.Controllers
{
    public class LoginController : Controller
    {
        private readonly ILoginService _loginService;

        public LoginController(ILoginService loginService)
        {
            _loginService = loginService;
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
                if (ex.Message == "Usuario inactivo")
                {
                    model.ErrorLogin = "Usuario inactivo";
                }
                else if (ex.Message == "Este dirigente no está asignado a ningún partido político.")
                {
                    model.ErrorLogin = "Este dirigente no está asignado a ningún partido político.";
                }
                else
                {
                    model.ErrorLogin = "Error inesperado al iniciar sesión.";
                }
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
