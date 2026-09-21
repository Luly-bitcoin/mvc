using mvc.Models;
using MySqlConnector;

namespace mvc.Repositories
{
    public interface IRepositorioInquilino
    {
        List<Inquilino> ObtenerTodos();
        IEnumerable<Inquilino> ObtenerPaginado(int pagina, int cantidadPorPagina);
        void Alta(Inquilino inquilino);
        Inquilino? ObtenerPorId(int id);
        void Modificacion(Inquilino inquilino);
        void Baja(int id);
    }

    public class RepositorioInquilino : IRepositorioInquilino
    {
        private readonly string _connectionString;

        public RepositorioInquilino(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")!;
        }

        public List<Inquilino> ObtenerTodos()
        {
            var inquilinos = new List<Inquilino>();

            using (var connection = new MySqlConnection(_connectionString))
            {
                var sql = "SELECT Id, Nombre, Apellido, Dni, Email, Telefono FROM inquilino";

                using (var command = new MySqlCommand(sql, connection))
                {
                    connection.Open();

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            inquilinos.Add(MapInquilino(reader));
                        }
                    }
                }
            }

            return inquilinos;
        }

        public IEnumerable<Inquilino> ObtenerPaginado(int pagina, int cantidadPorPagina)
        {
            var inquilinos = new List<Inquilino>();
            int offset = (pagina - 1) * cantidadPorPagina;

            using (var connection = new MySqlConnection(_connectionString))
            {
                string sql = "SELECT Id, Nombre, Apellido, Dni, Email, Telefono FROM inquilino ORDER BY Id LIMIT @Cantidad OFFSET @Offset";

                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@Offset", offset);
                    command.Parameters.AddWithValue("@Cantidad", cantidadPorPagina);

                    connection.Open();

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            inquilinos.Add(MapInquilino(reader));
                        }
                    }
                }
            }

            return inquilinos;
        }

        public void Alta(Inquilino inquilino)
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                var sql = @"
                    INSERT INTO inquilino (Nombre, Apellido, Dni, Email, Telefono)
                    VALUES (@nombre, @apellido, @dni, @email, @telefono);
                    SELECT LAST_INSERT_ID();";

                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@nombre", inquilino.Nombre);
                    command.Parameters.AddWithValue("@apellido", inquilino.Apellido);
                    command.Parameters.AddWithValue("@dni", inquilino.Dni);
                    command.Parameters.AddWithValue("@email", inquilino.Email ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@telefono", inquilino.Telefono ?? (object)DBNull.Value);

                    connection.Open();
                    inquilino.Id = Convert.ToInt32(command.ExecuteScalar());
                }
            }
        }

        public Inquilino? ObtenerPorId(int id)
        {
            Inquilino? inquilino = null;

            using (var connection = new MySqlConnection(_connectionString))
            {
                var sql = @"
                    SELECT 
                        i.Id,
                        i.Nombre,
                        i.Apellido,
                        i.Dni,
                        i.Email,
                        i.Telefono,

                        r.id AS ReservaId,
                        r.id_inmueble,
                        r.id_inquilino,
                        r.fecha_desde,
                        r.fecha_hasta,
                        r.monto_diario,
                        r.activo,

                        inm.direccion AS InmuebleDireccion

                    FROM inquilino i

                    LEFT JOIN reserva r
                        ON r.id_inquilino = i.Id

                    LEFT JOIN inmueble inm
                        ON inm.id = r.id_inmueble

                    WHERE i.Id = @id

                    ORDER BY r.fecha_desde DESC";

                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@id", id);

                    connection.Open();

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            if (inquilino == null)
                            {
                                inquilino = new Inquilino
                                {
                                    Id = reader.GetInt32(reader.GetOrdinal("Id")),
                                    Nombre = reader.GetString(reader.GetOrdinal("Nombre")),
                                    Apellido = reader.GetString(reader.GetOrdinal("Apellido")),
                                    Dni = reader.GetString(reader.GetOrdinal("Dni")),

                                    Email = reader.IsDBNull(reader.GetOrdinal("Email"))
                                        ? null
                                        : reader.GetString(reader.GetOrdinal("Email")),

                                    Telefono = reader.IsDBNull(reader.GetOrdinal("Telefono"))
                                        ? null
                                        : reader.GetString(reader.GetOrdinal("Telefono")),

                                    Reservas = new List<Reserva>()
                                };
                            }

                            if (!reader.IsDBNull(reader.GetOrdinal("ReservaId")))
                            {
                                var reserva = new Reserva
                                {
                                    Id = reader.GetInt32(reader.GetOrdinal("ReservaId")),

                                    IdInmueble = reader.GetInt32(
                                        reader.GetOrdinal("id_inmueble")
                                    ),

                                    IdInquilino = reader.GetInt32(
                                        reader.GetOrdinal("id_inquilino")
                                    ),

                                    FechaDesde = reader.GetDateTime(
                                        reader.GetOrdinal("fecha_desde")
                                    ),

                                    FechaHasta = reader.GetDateTime(
                                        reader.GetOrdinal("fecha_hasta")
                                    ),

                                    MontoDiario = reader.GetDecimal(
                                        reader.GetOrdinal("monto_diario")
                                    ),

                                    Activo = reader.GetInt32(
                                        reader.GetOrdinal("activo")
                                    ),

                                    InmuebleDireccion =
                                        reader.IsDBNull(
                                            reader.GetOrdinal("InmuebleDireccion")
                                        )
                                        ? ""
                                        : reader.GetString(
                                            reader.GetOrdinal("InmuebleDireccion")
                                        )
                                };

                                inquilino.Reservas!.Add(reserva);
                            }
                        }
                    }
                }
            }

            return inquilino;
        }

        public void Modificacion(Inquilino inquilino)
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                var sql = @"
                    UPDATE inquilino
                    SET
                        Nombre = @nombre,
                        Apellido = @apellido,
                        Dni = @dni,
                        Email = @email,
                        Telefono = @telefono
                    WHERE Id = @id";

                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@nombre", inquilino.Nombre);
                    command.Parameters.AddWithValue("@apellido", inquilino.Apellido);
                    command.Parameters.AddWithValue("@dni", inquilino.Dni);
                    command.Parameters.AddWithValue("@email", inquilino.Email ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@telefono", inquilino.Telefono ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@id", inquilino.Id);

                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
        }

        public void Baja(int id)
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                var sql = "DELETE FROM inquilino WHERE Id = @id";

                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@id", id);

                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
        }

        private static Inquilino MapInquilino(MySqlDataReader reader)
        {
            int emailOrdinal = reader.GetOrdinal(nameof(Inquilino.Email));
            int telefonoOrdinal = reader.GetOrdinal(nameof(Inquilino.Telefono));

            return new Inquilino
            {
                Id = reader.GetInt32(reader.GetOrdinal(nameof(Inquilino.Id))),
                Nombre = reader.GetString(reader.GetOrdinal(nameof(Inquilino.Nombre))),
                Apellido = reader.GetString(reader.GetOrdinal(nameof(Inquilino.Apellido))),
                Dni = reader.GetString(reader.GetOrdinal(nameof(Inquilino.Dni))),
                Email = reader.IsDBNull(emailOrdinal) ? null : reader.GetString(emailOrdinal),
                Telefono = reader.IsDBNull(telefonoOrdinal) ? null : reader.GetString(telefonoOrdinal)
            };
        }
    }
}