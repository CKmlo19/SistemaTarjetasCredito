using Microsoft.AspNetCore.Mvc;
using SistemaTarjetasCredito.Data;
using SistemaTarjetasCredito.Models;
using System.Collections.Generic;

namespace SistemaTarjetasCredito.Controllers
{
    public class TarjetaController : Controller
    {
        private readonly TarjetaData _tarjetaData = new TarjetaData();


        // Acción para ver los estados de cuenta en modo Administrador
        public ActionResult VerTodosLosEstados(int idTarjeta)
        {
            var estadosCuenta = _tarjetaData.ObtenerEstadosCuentaTCM(idTarjeta);
            ViewBag.IdTarjeta = idTarjeta;
            return View("/Views/Tarjeta/GridEstados.cshtml", estadosCuenta);
        }


        // Acción para ver los movimientos
        public ActionResult VerTodosLosMovimientos(int idTarjetaFisica)
        {
            var movimientos = _tarjetaData.ObtenerMovimientosPorEstado(idTarjetaFisica);
            ViewBag.IdTarjetaFisica = idTarjetaFisica;
            return View("GridMovimientos", movimientos);
        }


        public ActionResult estadoYsubEstados(int idTarjeta, string tipoCuenta)
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
