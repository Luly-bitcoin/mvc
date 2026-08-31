namespace mvc.Models
{
    public class Inmueble
    {
        public int Id { get; set; }

        public int IdPropietario { get; set; }

        public int IdTipoInmueble { get; set; }

        public string Direccion { get; set; } = "";

        public int Cupo { get; set; }

        public string? Coord { get; set; }

        public decimal Precio { get; set; }

        public int Activo { get; set; } = 1;

        public string? FotoPortada { get; set; }

        public string? Fotos { get; set; }

        public string? PropietarioNombre { get; set; }

        public string? TipoNombre { get; set; }

        public string Estado
        {
            get
            {
                return Activo == 1 ? "Disponible" : "No disponible";
            }
        }
    }
}