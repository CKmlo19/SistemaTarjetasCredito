using System;

namespace SistemaTarjetasCredito.Models
{
    public class SubestadoCuentaModel
    {
        public int Id { get; set; }
        public DateTime FechaEstadoCuenta { get; set; }
        public int CantidadOperacionesATM { get; set; }
        public int CantidadOperacionesVentanilla { get; set; }
        public int CantidadCompras { get; set; }
        public decimal SumaCompras { get; set; }
        public int CantidadRetiros { get; set; }
        public decimal SumaRetiros { get; set; }
        public decimal SumaCreditos { get; set; }   
        public decimal SumaDebitos { get; set; }    
    }
}
