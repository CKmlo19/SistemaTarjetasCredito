using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using SistemaTarjetasCredito.Data;
using SistemaTarjetasCredito.Models;
using System.Security.Claims;
using System.Net;

namespace SistemaTarjetasCredito.Controllers
{
    public class UsuarioController : Controller
    {


        private readonly TarjetaData _tarjetaData = new TarjetaData();

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string username, string password)
        {
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                TempData["ErrorMessage"] = "Por favor, complete todos los campos.";
                return RedirectToAction("Login");
            }

            int tipoUsuario = _tarjetaData.ObtenerIdTipoUsuario(username, password);
            //int tipoUsuario = 2;
            
            if (tipoUsuario == -1)
            {
                TempData["ErrorMessage"] = "Usuario o contraseña incorrectos.";
                return RedirectToAction("Login");
            }

            var claims = new List<Claim>
    {
        new Claim(ClaimTypes.Name, username),
        new Claim("TipoUsuario", tipoUsuario.ToString())
    };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity));

            if (tipoUsuario == 1) // Admin
            {
                var tarjetas = _tarjetaData.ListarTodasLasTarjetas();
                return View("/Views/Tarjeta/GridTodasTarjetas.cshtml", tarjetas); // Usa la vista de Tarjeta
            }
            else // TarjetaHabiente
            {
                int idUsuario = _tarjetaData.ObtenerIdUsuario(username, password);
                var tarjetas = _tarjetaData.ListarTarjetasPorUsuario(idUsuario);
                return View("/Views/Tarjeta/GridMisTarjetas.cshtml", tarjetas); // Usa la vista de Tarjeta
            }
        }


        public async Task<IActionResult> Salir(UsuarioModel usuario)
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            HttpContext.Session.Clear(); // Limpiar la sesión
            return RedirectToAction("Login");
        }
    }
}
