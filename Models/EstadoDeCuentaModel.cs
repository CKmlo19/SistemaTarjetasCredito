using System;

namespace SistemaTarjetasCredito.Models
{
    public class EstadoCuentaModel
    {
        public int Id { get; set; }
        public int IdTCM { get; set; }
        public decimal SaldoActual { get; set; }
        public decimal PagoMinimo { get; set; }
        public DateTime FechaEstadoCuenta { get; set; }
        public decimal InteresesCorrientes { get; set; }
        public decimal InteresesMoratorios { get; set; }
        public int CantidadOperacionesATM { get; set; }
        public int CantidadOperacionesVentanilla { get; set; }
    }
}
