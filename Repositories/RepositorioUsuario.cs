using System;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using MySqlConnector;
using mvc.Models;

namespace mvc.Repositories
{
    public class RepositorioUsuario : IRepositorioUsuario
    {
        private readonly string connectionString;

        public RepositorioUsuario(IConfiguration configuration)
        {
            connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException(
                    "No se encontró la cadena de conexión DefaultConnection");
        }

        public Usuario? ObtenerPorEmail(string email)
        {
            Usuario? usuario = null;

            using var connection = new MySqlConnection(connectionString);
            connection.Open();

            var sql = @"
                SELECT id, nombre_usuario, nombre, apellido, email, password, avatar, rol
                FROM usuario
                WHERE email = @email
                LIMIT 1";

            using var command = new MySqlCommand(sql, connection);
            command.Parameters.AddWithValue("@email", email);

            using var reader = command.ExecuteReader();

            if (reader.Read())
            {
                usuario = MapearUsuario(reader);
            }

            return usuario;
        }

        public Usuario? ObtenerPorNombreUsuario(string nombreUsuario)
        {
            Usuario? usuario = null;

            using var connection = new MySqlConnection(connectionString);
            connection.Open();

            var sql = @"
                SELECT id, nombre_usuario, nombre, apellido, email, password, avatar, rol
                FROM usuario
                WHERE nombre_usuario = @nombre_usuario
                LIMIT 1";

            using var command = new MySqlCommand(sql, connection);
            command.Parameters.AddWithValue("@nombre_usuario", nombreUsuario);

            using var reader = command.ExecuteReader();

            if (reader.Read())
            {
                usuario = MapearUsuario(reader);
            }

            return usuario;
        }

        public Usuario? ObtenerPorId(int id)
        {
            Usuario? usuario = null;

            using var connection = new MySqlConnection(connectionString);
            connection.Open();

            var sql = @"
                SELECT id, nombre_usuario, nombre, apellido, email, password, avatar, rol
                FROM usuario
                WHERE id = @id
                LIMIT 1";

            using var command = new MySqlCommand(sql, connection);
            command.Parameters.AddWithValue("@id", id);

            using var reader = command.ExecuteReader();

            if (reader.Read())
            {
                usuario = MapearUsuario(reader);
            }

            return usuario;
        }

        public List<Usuario> ObtenerTodos()
        {
            var usuarios = new List<Usuario>();

            using var connection = new MySqlConnection(connectionString);
            connection.Open();

            var sql = @"
                SELECT id, nombre_usuario, nombre, apellido, email, password, avatar, rol
                FROM usuario
                ORDER BY apellido, nombre";

            using var command = new MySqlCommand(sql, connection);
            using var reader = command.ExecuteReader();

            while (reader.Read())
            {
                usuarios.Add(MapearUsuario(reader));
            }

            return usuarios;
        }

        public int Alta(Usuario usuario)
        {
            using var connection = new MySqlConnection(connectionString);
            connection.Open();

            var sql = @"
                INSERT INTO usuario
                (nombre_usuario, nombre, apellido, email, password, avatar, rol)
                VALUES
                (@nombre_usuario, @nombre, @apellido, @email, @password, @avatar, @rol);

                SELECT LAST_INSERT_ID();";

            using var command = new MySqlCommand(sql, connection);

            command.Parameters.AddWithValue("@nombre_usuario", usuario.NombreUsuario);
            command.Parameters.AddWithValue("@nombre", usuario.Nombre);
            command.Parameters.AddWithValue("@apellido", usuario.Apellido);
            command.Parameters.AddWithValue("@email", usuario.Email);
            command.Parameters.AddWithValue("@password", usuario.Password);
            command.Parameters.AddWithValue("@avatar", usuario.Avatar);
            command.Parameters.AddWithValue("@rol", usuario.Rol);

            return Convert.ToInt32(command.ExecuteScalar());
        }

        public int Modificacion(Usuario usuario)
        {
            using var connection = new MySqlConnection(connectionString);
            connection.Open();

            var sql = @"
                UPDATE usuario
                SET nombre_usuario = @nombre_usuario,
                    nombre = @nombre,
                    apellido = @apellido,
                    email = @email,
                    password = @password,
                    avatar = @avatar,
                    rol = @rol
                WHERE id = @id";

            using var command = new MySqlCommand(sql, connection);

            command.Parameters.AddWithValue("@id", usuario.Id);
            command.Parameters.AddWithValue("@nombre_usuario", usuario.NombreUsuario);
            command.Parameters.AddWithValue("@nombre", usuario.Nombre);
            command.Parameters.AddWithValue("@apellido", usuario.Apellido);
            command.Parameters.AddWithValue("@email", usuario.Email);
            command.Parameters.AddWithValue("@password", usuario.Password);
            command.Parameters.AddWithValue("@avatar", usuario.Avatar);
            command.Parameters.AddWithValue("@rol", usuario.Rol);

            return command.ExecuteNonQuery();
        }

        public int Baja(int id)
        {
            using var connection = new MySqlConnection(connectionString);
            connection.Open();

            var sql = @"
                DELETE FROM usuario
                WHERE id = @id";

            using var command = new MySqlCommand(sql, connection);
            command.Parameters.AddWithValue("@id", id);

            return command.ExecuteNonQuery();
        }

        private Usuario MapearUsuario(MySqlDataReader reader)
        {
            return new Usuario
            {
                Id = reader.GetInt32("id"),
                NombreUsuario = reader.GetString("nombre_usuario"),
                Nombre = reader.GetString("nombre"),
                Apellido = reader.GetString("apellido"),
                Email = reader.GetString("email"),
                Password = reader.GetString("password"),
                Avatar = reader.IsDBNull(reader.GetOrdinal("avatar"))
                    ? null
                    : reader.GetString("avatar"),
                Rol = reader.GetString("rol")
            };
        }
    }
}