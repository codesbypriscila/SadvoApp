using Microsoft.AspNetCore.Mvc;
using SADVO.Application.Interfaces;
using SADVO.Application.ViewModels;
using SADVO.Application.Utils;

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

        var usuario = await _loginService.LoginAsync(model);
        if (usuario == null)
        {
            model.ErrorLogin = "Credenciales inválidas o usuario inactivo.";
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

    [HttpGet]
    public ActionResult Logout()
    {
        HttpContext.Session.Clear();
        return RedirectToAction("Index", "Home");
    }
}