using Microsoft.AspNetCore.Mvc;
using SistemaTarjetasCredito.Data;
using SistemaTarjetasCredito.Models;
using System.Collections.Generic;

namespace SistemaTarjetasCredito.Controllers
{
    public class TarjetaController : Controller
    {
        private readonly TarjetaData _tarjetaData = new TarjetaData();

        //// Acción para ver la lista de todas las tarjetas
        //public ActionResult ListarTodas()
        //{
        //    List<TarjetaModel> tarjetas = _tarjetaData.ListarTodasLasTarjetas();
        //    return View("GridTodasTarjetas", tarjetas); 
        //}

        //// Acción para ver los estados de cuenta
        //public ActionResult VerEstados(int idTarjeta)
        //{
        //    var estadosCuenta = _tarjetaData.ObtenerEstadosCuentaTCM(idTarjeta);
        //    ViewBag.IdTarjeta = idTarjeta;
        //    return View("GridEstados", estadosCuenta);
        //}

        //// Acción para ver los movimientos
        //public ActionResult VerMovimientos(int idTarjetaFisica)
        //{
        //    var movimientos = _tarjetaData.ObtenerMovimientosPorEstado(idTarjetaFisica);
        //    ViewBag.IdTarjetaFisica = idTarjetaFisica;
        //    return View("GridMovimientos", movimientos);
        //}


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
            return 172; // Ejemplo: Retorna un id de prueba (reemplaza esto con el código real)
        }

        public ActionResult VerEstados(int idTarjeta, string tipoCuenta)
        {
            if (tipoCuenta == "TCM")
            {
                var estadosCuenta = _tarjetaData.ObtenerEstadosCuentaTCM(idTarjeta);
                return View("GridEstadosTCM", estadosCuenta); // Muestra la vista de estados de cuenta para TCM
            }
            else if (tipoCuenta == "TCA")
            {
                var subestadosCuenta = _tarjetaData.ObtenerSubestadosCuentaTCA(idTarjeta);
                return View("GridSubestadosTCA", subestadosCuenta); // Muestra la vista de subestados de cuenta para TCA
            }

            return RedirectToAction("MisTarjetas"); // Redirige a "Mis Tarjetas" si no coincide el tipo de cuenta
        }


        public ActionResult VerMovimientos(int idTarjetaFisica)
        {
            var movimientos = _tarjetaData.ObtenerMovimientosPorEstado(idTarjetaFisica);
            return View("GridMovimientos", movimientos);
        }

        public ActionResult VerMovimientosTCM(int idTarjetaFisica)
        {
            var movimientos = _tarjetaData.ObtenerMovimientosPorEstado(idTarjetaFisica);
            return View("GridMovimientosTCM", movimientos);
        }

    }
}
