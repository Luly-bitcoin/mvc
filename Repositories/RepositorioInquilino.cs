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
                            inquilinos.Add(new Inquilino
                            {
                                Id = reader.GetInt32(nameof(Inquilino.Id)),
                                Nombre = reader.GetString(nameof(Inquilino.Nombre)),
                                Apellido = reader.GetString(nameof(Inquilino.Apellido)),
                                Dni = reader.GetString(nameof(Inquilino.Dni)),

                                Email = reader.IsDBNull(
                                    reader.GetOrdinal(nameof(Inquilino.Email)))
                                    ? null
                                    : reader.GetString(nameof(Inquilino.Email)),

                                Telefono = reader.IsDBNull(
                                    reader.GetOrdinal(nameof(Inquilino.Telefono)))
                                    ? null
                                    : reader.GetString(nameof(Inquilino.Telefono))
                            });
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
                            inquilinos.Add(new Inquilino
                            {
                                Id = reader.GetInt32(nameof(Inquilino.Id)),
                                Nombre = reader.GetString(nameof(Inquilino.Nombre)),
                                Apellido = reader.GetString(nameof(Inquilino.Apellido)),
                                Dni = reader.GetString(nameof(Inquilino.Dni)),
                                Email = reader.IsDBNull(reader.GetOrdinal(nameof(Inquilino.Email))) ? null : reader.GetString(nameof(Inquilino.Email)),
                                Telefono = reader.IsDBNull(reader.GetOrdinal(nameof(Inquilino.Telefono))) ? null : reader.GetString(nameof(Inquilino.Telefono))
                            });
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
                    INSERT INTO inquilino
                    (Nombre, Apellido, Dni, Email, Telefono)
                    VALUES
                    (@nombre, @apellido, @dni, @email, @telefono)";

                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue(
                        "@nombre",
                        inquilino.Nombre);

                    command.Parameters.AddWithValue(
                        "@apellido",
                        inquilino.Apellido);

                    command.Parameters.AddWithValue(
                        "@dni",
                        inquilino.Dni);

                    command.Parameters.AddWithValue(
                        "@email",
                        inquilino.Email ?? (object)DBNull.Value);

                    command.Parameters.AddWithValue(
                        "@telefono",
                        inquilino.Telefono ?? (object)DBNull.Value);

                    connection.Open();

                    command.ExecuteNonQuery();
                }
            }
        }

        public Inquilino? ObtenerPorId(int id)
        {
            Inquilino? inquilino = null;

            using (var connection = new MySqlConnection(_connectionString))
            {
                var sql = @"
                    SELECT Id, Nombre, Apellido, Dni, Email, Telefono
                    FROM inquilino
                    WHERE Id = @id";

                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@id", id);

                    connection.Open();

                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            inquilino = new Inquilino
                            {
                                Id = reader.GetInt32(
                                    nameof(Inquilino.Id)),

                                Nombre = reader.GetString(
                                    nameof(Inquilino.Nombre)),

                                Apellido = reader.GetString(
                                    nameof(Inquilino.Apellido)),

                                Dni = reader.GetString(
                                    nameof(Inquilino.Dni)),

                                Email = reader.IsDBNull(
                                    reader.GetOrdinal(nameof(Inquilino.Email)))
                                    ? null
                                    : reader.GetString(nameof(Inquilino.Email)),

                                Telefono = reader.IsDBNull(
                                    reader.GetOrdinal(nameof(Inquilino.Telefono)))
                                    ? null
                                    : reader.GetString(nameof(Inquilino.Telefono))
                            };
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
                    command.Parameters.AddWithValue(
                        "@nombre",
                        inquilino.Nombre);

                    command.Parameters.AddWithValue(
                        "@apellido",
                        inquilino.Apellido);

                    command.Parameters.AddWithValue(
                        "@dni",
                        inquilino.Dni);

                    command.Parameters.AddWithValue(
                        "@email",
                        inquilino.Email ?? (object)DBNull.Value);

                    command.Parameters.AddWithValue(
                        "@telefono",
                        inquilino.Telefono ?? (object)DBNull.Value);

                    command.Parameters.AddWithValue(
                        "@id",
                        inquilino.Id);

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
    }
}