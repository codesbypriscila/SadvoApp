using Microsoft.AspNetCore.Mvc;
using SADVO.Application.Interfaces;
using SADVO.Application.ViewModels;

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
    public IActionResult Login() => View();

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
            ModelState.AddModelError("", "Credenciales inválidas o usuario inactivo.");
            return View(model);
        }

        if (usuario.Rol == "Administrador")
            return RedirectToAction("Index", "Admin");
        else if (usuario.Rol == "Dirigente")
            return RedirectToAction("Index", "Dirigente");

        return RedirectToAction("Login");
    }

    [HttpGet]
    public ActionResult Logout()
    {

        return RedirectToAction("Index", "Home");
    }
}
