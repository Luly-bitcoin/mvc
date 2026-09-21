using mvc.Models;
using MySqlConnector;
using System.Linq;

namespace mvc.Repositories
{
    public interface IRepositorioPropietario
    {
        List<Propietario> ObtenerTodos();
        IEnumerable<Propietario> ObtenerPaginado(int pagina, int cantidadPorPagina);
        void Alta(Propietario propietario);
        Propietario? ObtenerPorId(int id);
        void Modificacion(Propietario propietario);
        void Baja(int id);
    }

    public class RepositorioPropietario : IRepositorioPropietario
    {
        private readonly string _connectionString;

        public RepositorioPropietario(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")!;
        }

        public List<Propietario> ObtenerTodos()
        {
            var propietarios = new List<Propietario>();

            using (var connection = new MySqlConnection(_connectionString))
            {
                var sql = "SELECT Id, Nombre, Apellido, Dni, Email, Telefono FROM propietario";

                using (var command = new MySqlCommand(sql, connection))
                {
                    connection.Open();

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            propietarios.Add(MapPropietario(reader));
                        }
                    }
                }
            }

            return propietarios;
        }

        public IEnumerable<Propietario> ObtenerPaginado(int pagina, int cantidadPorPagina)
        {
            var lista = new List<Propietario>();
            int offset = (pagina - 1) * cantidadPorPagina;

            using (var connection = new MySqlConnection(_connectionString))
            {
                string sql = "SELECT Id, Nombre, Apellido, Dni, Email, Telefono FROM propietario ORDER BY Id LIMIT @Cantidad OFFSET @Offset";

                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@Offset", offset);
                    command.Parameters.AddWithValue("@Cantidad", cantidadPorPagina);

                    connection.Open();

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            lista.Add(MapPropietario(reader));
                        }
                    }
                }
            }

            return lista;
        }

        public void Alta(Propietario propietario)
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                var sql = @"
                    INSERT INTO propietario (Nombre, Apellido, Dni, Email, Telefono)
                    VALUES (@nombre, @apellido, @dni, @email, @telefono);
                    SELECT LAST_INSERT_ID();";

                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@nombre", propietario.Nombre);
                    command.Parameters.AddWithValue("@apellido", propietario.Apellido);
                    command.Parameters.AddWithValue("@dni", propietario.Dni);
                    command.Parameters.AddWithValue("@email", propietario.Email ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@telefono", propietario.Telefono ?? (object)DBNull.Value);

                    connection.Open();
                    propietario.Id = Convert.ToInt32(command.ExecuteScalar());
                }
            }
        }

        public Propietario? ObtenerPorId(int id)
        {
            Propietario? propietario = null;

            using (var connection = new MySqlConnection(_connectionString))
            {
                var sql = @"
                    SELECT
                        p.id AS PropietarioId,
                        p.nombre AS PropietarioNombre,
                        p.apellido AS PropietarioApellido,
                        p.dni AS PropietarioDni,
                        p.email AS PropietarioEmail,
                        p.telefono AS PropietarioTelefono,

                        i.id AS InmuebleId,
                        i.id_propietario AS IdPropietario,
                        i.direccion AS Direccion,
                        i.precio AS Precio,
                        i.activo AS InmuebleActivo,

                        ti.descripcion AS TipoNombre,

                        r.id AS ReservaId,
                        r.id_inquilino AS IdInquilino,
                        r.fecha_desde AS FechaDesde,
                        r.fecha_hasta AS FechaHasta,
                        r.monto_diario AS MontoDiario,
                        r.activo AS ReservaActivo,

                        CONCAT(inq.nombre, ' ', inq.apellido) AS InquilinoNombre

                    FROM propietario p

                    LEFT JOIN inmueble i
                        ON i.id_propietario = p.id

                    LEFT JOIN tipo_inmueble ti
                        ON ti.id = i.id_tipo_inmueble

                    LEFT JOIN reserva r
                        ON r.id_inmueble = i.id

                    LEFT JOIN inquilino inq
                        ON inq.id = r.id_inquilino

                    WHERE p.id = @id

                    ORDER BY i.id, r.fecha_desde DESC";

                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@id", id);

                    connection.Open();

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {

                            if (propietario == null)
                            {
                                propietario = new Propietario
                                {
                                    Id = reader.GetInt32(
                                        reader.GetOrdinal("PropietarioId")
                                    ),

                                    Nombre = reader.GetString(
                                        reader.GetOrdinal("PropietarioNombre")
                                    ),

                                    Apellido = reader.GetString(
                                        reader.GetOrdinal("PropietarioApellido")
                                    ),

                                    Dni = reader.GetString(
                                        reader.GetOrdinal("PropietarioDni")
                                    ),

                                    Email = reader.IsDBNull(
                                        reader.GetOrdinal("PropietarioEmail")
                                    )
                                        ? null
                                        : reader.GetString(
                                            reader.GetOrdinal("PropietarioEmail")
                                        ),

                                    Telefono = reader.IsDBNull(
                                        reader.GetOrdinal("PropietarioTelefono")
                                    )
                                        ? null
                                        : reader.GetString(
                                            reader.GetOrdinal("PropietarioTelefono")
                                        ),

                                    Inmuebles = new List<Inmueble>()
                                };
                            }


                            if (!reader.IsDBNull(reader.GetOrdinal("InmuebleId")))
                            {
                                int inmuebleId = reader.GetInt32(
                                    reader.GetOrdinal("InmuebleId")
                                );

                                var inmueble = propietario.Inmuebles!
                                    .FirstOrDefault(i => i.Id == inmuebleId);

                                if (inmueble == null)
                                {
                                    inmueble = new Inmueble
                                    {
                                        Id = inmuebleId,

                                        IdPropietario = reader.GetInt32(
                                            reader.GetOrdinal("IdPropietario")
                                        ),

                                        Direccion = reader.GetString(
                                            reader.GetOrdinal("Direccion")
                                        ),

                                        Precio = reader.GetDecimal(
                                            reader.GetOrdinal("Precio")
                                        ),

                                        Activo = reader.GetInt32(
                                            reader.GetOrdinal("InmuebleActivo")
                                        ),

                                        TipoNombre = reader.IsDBNull(
                                            reader.GetOrdinal("TipoNombre")
                                        )
                                            ? ""
                                            : reader.GetString(
                                                reader.GetOrdinal("TipoNombre")
                                            ),

                                        Reservas = new List<Reserva>()
                                    };

                                    propietario.Inmuebles!.Add(inmueble);
                                }


                                if (!reader.IsDBNull(reader.GetOrdinal("ReservaId")))
                                {
                                    int reservaId = reader.GetInt32(
                                        reader.GetOrdinal("ReservaId")
                                    );

                                    var reservaExistente = inmueble.Reservas!
                                        .FirstOrDefault(r => r.Id == reservaId);

                                    if (reservaExistente == null)
                                    {
                                        var reserva = new Reserva
                                        {
                                            Id = reservaId,

                                            IdInmueble = inmuebleId,

                                            IdInquilino = reader.GetInt32(
                                                reader.GetOrdinal("IdInquilino")
                                            ),

                                            FechaDesde = reader.GetDateTime(
                                                reader.GetOrdinal("FechaDesde")
                                            ),

                                            FechaHasta = reader.GetDateTime(
                                                reader.GetOrdinal("FechaHasta")
                                            ),

                                            MontoDiario = reader.GetDecimal(
                                                reader.GetOrdinal("MontoDiario")
                                            ),

                                            Activo = reader.GetInt32(
                                                reader.GetOrdinal("ReservaActivo")
                                            ),

                                            InquilinoNombre = reader.IsDBNull(
                                                reader.GetOrdinal("InquilinoNombre")
                                            )
                                                ? ""
                                                : reader.GetString(
                                                    reader.GetOrdinal("InquilinoNombre")
                                                )
                                        };

                                        inmueble.Reservas!.Add(reserva);
                                    }
                                }
                            }
                        }
                    }
                }
            }

            return propietario;
        }

        public void Modificacion(Propietario propietario)
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                var sql = @"
                    UPDATE propietario
                    SET
                        Nombre = @nombre,
                        Apellido = @apellido,
                        Dni = @dni,
                        Email = @email,
                        Telefono = @telefono
                    WHERE Id = @id";

                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@nombre", propietario.Nombre);
                    command.Parameters.AddWithValue("@apellido", propietario.Apellido);
                    command.Parameters.AddWithValue("@dni", propietario.Dni);
                    command.Parameters.AddWithValue("@email", propietario.Email ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@telefono", propietario.Telefono ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@id", propietario.Id);

                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
        }

        public void Baja(int id)
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                var sql = "DELETE FROM propietario WHERE Id = @id";

                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@id", id);

                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
        }

        private static Propietario MapPropietario(MySqlDataReader reader)
        {
            int emailOrdinal = reader.GetOrdinal(nameof(Propietario.Email));
            int telefonoOrdinal = reader.GetOrdinal(nameof(Propietario.Telefono));

            return new Propietario
            {
                Id = reader.GetInt32(reader.GetOrdinal(nameof(Propietario.Id))),
                Nombre = reader.GetString(reader.GetOrdinal(nameof(Propietario.Nombre))),
                Apellido = reader.GetString(reader.GetOrdinal(nameof(Propietario.Apellido))),
                Dni = reader.GetString(reader.GetOrdinal(nameof(Propietario.Dni))),
                Email = reader.IsDBNull(emailOrdinal) ? null : reader.GetString(emailOrdinal),
                Telefono = reader.IsDBNull(telefonoOrdinal) ? null : reader.GetString(telefonoOrdinal)
            };
        }
    }
}