namespace mvc.Models
{
    public class Reserva
    {
        public int Id { get; set; }
        public int IdInmueble { get; set; }
        public int IdInquilino { get; set; }
        public DateTime FechaDesde { get; set; }
        public DateTime FechaHasta { get; set; }
        public decimal MontoDiario { get; set; }
        public int Activo { get; set; } = 1;
        public int CreadoPorUserId { get; set; }
        public int? TerminadoPorUserId { get; set; }

        public string? InmuebleDireccion { get; set; }
        public string? InquilinoNombre { get; set; }
    }
}