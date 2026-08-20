using mvc.Models;
using MySqlConnector;

namespace mvc.Repositories
{
    public interface IRepositorioPropietario
    {
        List<Propietario> ObtenerTodos();
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
                            propietarios.Add(new Propietario
                            {
                                Id = reader.GetInt32(nameof(Propietario.Id)),
                                Nombre = reader.GetString(nameof(Propietario.Nombre)),
                                Apellido = reader.GetString(nameof(Propietario.Apellido)),
                                Dni = reader.GetString(nameof(Propietario.Dni)),
                                Email = reader.IsDBNull(reader.GetOrdinal(nameof(Propietario.Email))) ? null : reader.GetString(nameof(Propietario.Email)),
                                Telefono = reader.IsDBNull(reader.GetOrdinal(nameof(Propietario.Telefono))) ? null : reader.GetString(nameof(Propietario.Telefono))
                            });
                        }
                    }
                }
            }
            return propietarios;
        }

        public void Alta(Propietario propietario)
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                var sql = "INSERT INTO propietario (Nombre, Apellido, Dni, Email, Telefono) VALUES (@nombre, @apellido, @dni, @email, @telefono)";
                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@nombre", propietario.Nombre);
                    command.Parameters.AddWithValue("@apellido", propietario.Apellido);
                    command.Parameters.AddWithValue("@dni", propietario.Dni);
                    command.Parameters.AddWithValue("@email", propietario.Email ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@telefono", propietario.Telefono ?? (object)DBNull.Value);

                    connection.Open();
                    command.ExecuteNonQuery();
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
                            propietario = new Propietario
                            {
                                Id = reader.GetInt32(nameof(Propietario.Id)),
                                Nombre = reader.GetString(nameof(Propietario.Nombre)),
                                Apellido = reader.GetString(nameof(Propietario.Apellido)),
                                Dni = reader.GetString(nameof(Propietario.Dni)),
                                Email = reader.IsDBNull(reader.GetOrdinal(nameof(Propietario.Email))) ? null : reader.GetString(nameof(Propietario.Email)),
                                Telefono = reader.IsDBNull(reader.GetOrdinal(nameof(Propietario.Telefono))) ? null : reader.GetString(nameof(Propietario.Telefono))
                            };
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
                var sql = "UPDATE propietario SET Nombre = @nombre, Apellido = @apellido, Dni = @dni, Email = @email, Telefono = @telefono WHERE Id = @id";
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
    }
}