using System;

namespace SistemaTarjetasCredito.Models
{
    public class TarjetaModel
    {
        public int Id { get; set; }
        public string NumeroTarjeta { get; set; } // Número de la tarjeta (TCM o TCA)
        public string Estado { get; set; } // Estado de la tarjeta (Activa o No Activa)
        public string TipoCuenta { get; set; } // Tipo de cuenta (TCM o TCA)
        public DateTime FechaVencimiento { get; set; } // Fecha de vencimiento o de creación
    }
}
