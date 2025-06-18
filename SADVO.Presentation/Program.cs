using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using SADVO.Application.Interfaces;
using SADVO.Application.Mappings;
using SADVO.Application.Services;
using SADVO.Domain.Interfaces;
using SADVO.Infrastructure.AppDbContext;
using SADVO.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllersWithViews();

//dbcontext
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Login/Index";
        options.AccessDeniedPath = "/Login/AccesoDenegado";
    });
    
//services
builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
builder.Services.AddScoped<ILoginService, LoginService>();
builder.Services.AddScoped<ILoginRepository, LoginRepository>();
builder.Services.AddScoped<ICiudadanoService, CiudadanoService>();
builder.Services.AddScoped<IEleccionService, EleccionService>();
builder.Services.AddScoped<IPuestoElectivoService, PuestoElectivoService>();
builder.Services.AddScoped<IPartidoPoliticoService, PartidoPoliticoService>();
builder.Services.AddScoped<IUsuarioService, UsuarioService>();
builder.Services.AddScoped<IRolService, RolService>();
builder.Services.AddScoped<IAsignacionDirigenteService, AsignacionDirigenteService>();
builder.Services.AddScoped<ICandidatoService, CandidatoService>();
builder.Services.AddScoped<IAlianzaPoliticaService, AlianzaPoliticaService>();
builder.Services.AddScoped<IDirigenteService, DirigenteService>();


builder.Services.AddAutoMapper(typeof(MappingProfile));

//sesion
builder.Services.AddSession(opt =>
{
    opt.IdleTimeout = TimeSpan.FromMinutes(10);
    opt.Cookie.HttpOnly = true;
});

var app = builder.Build();

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication(); 
app.UseSession();    //  
app.UseAuthorization(); 

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

// Rutes
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
