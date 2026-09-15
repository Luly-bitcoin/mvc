using System;

namespace mvc.Models
{
    public class Pago
    {
        public int Id { get; set; }
        public int IdReserva { get; set; }
        public string Concepto { get; set; } = "";
        public DateTime Fecha { get; set; }
        public decimal Importe { get; set; }
        public int Activo { get; set; } = 1;
        public int? CreadoPorUserId { get; set; }
        public int? AnuladoPorUserId { get; set; }
        public string? ReservaDetalle { get; set; }
    }
}