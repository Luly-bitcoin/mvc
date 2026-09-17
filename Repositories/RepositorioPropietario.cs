using mvc.Models;
using MySqlConnector;

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
                var sql = "SELECT Id, Nombre, Apellido, Dni, Email, Telefono FROM propietario WHERE Id = @id";

                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@id", id);

                    connection.Open();

                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            propietario = MapPropietario(reader);
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