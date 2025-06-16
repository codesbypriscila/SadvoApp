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
            return RedirectToAction("Login");
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View(new LoginViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
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
                    _ => RedirectToAction("Login")
                };
            }
            catch (ApplicationException ex) when (ex.Message == "Usuario inactivo")
            {
                model.ErrorLogin = "Usuario inactivo";
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