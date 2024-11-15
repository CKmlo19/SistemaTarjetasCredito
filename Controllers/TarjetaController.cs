using Microsoft.AspNetCore.Mvc;
using SistemaTarjetasCredito.Data;
using SistemaTarjetasCredito.Models;
using System.Collections.Generic;

namespace SistemaTarjetasCredito.Controllers
{
    public class TarjetaController : Controller
    {
        private readonly TarjetaData _tarjetaData = new TarjetaData();

        // Acción para ver la lista de todas las tarjetas
        public ActionResult ListarTodas()
        {
            List<TarjetaModel> tarjetas = _tarjetaData.ListarTodasLasTarjetas();
            return View("GridTodasTarjetas", tarjetas); 
        }

        // Acción para ver los estados de cuenta
        public ActionResult VerEstados(int idTarjeta)
        {
            var estadosCuenta = _tarjetaData.ObtenerEstadosCuentaTCM(idTarjeta);
            ViewBag.IdTarjeta = idTarjeta;
            return View("GridEstados", estadosCuenta);
        }

        // Acción para ver los movimientos
        public ActionResult VerMovimientos(int idTarjetaFisica)
        {
            var movimientos = _tarjetaData.ObtenerMovimientosPorEstado(idTarjetaFisica);
            ViewBag.IdTarjetaFisica = idTarjetaFisica;
            return View("GridMovimientos", movimientos);
        }

        // Acción para listar las tarjetas del usuario actual (Tarjeta Habiente)
        public ActionResult MisTarjetas()
        {
            int idUsuario = ObtenerIdUsuarioActual(); // Método para obtener el id del usuario autenticado
            List<TarjetaModel> tarjetas = _tarjetaData.ListarTarjetasPorUsuario(idUsuario);
            return View("GridMisTarjetas", tarjetas);
        }

        // Método auxiliar para obtener el id del usuario actual
        private int ObtenerIdUsuarioActual()
        {
            // Aquí deberías obtener el id del usuario actual de la sesión o contexto de autenticación
            return 154; // Ejemplo: Retorna un id de prueba (reemplaza esto con el código real)
        }
    }
}
