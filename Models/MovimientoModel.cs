using System;

namespace SistemaTarjetasCredito.Models
{
    public class MovimientoModel
    {
        public DateTime FechaOperacion { get; set; }
        public string TipoMovimiento { get; set; }
        public string Descripcion { get; set; }
        public string Referencia { get; set; }
        public decimal Monto { get; set; }
        public decimal NuevoSaldo { get; set; }
    }
}
