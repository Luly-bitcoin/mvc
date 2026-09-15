using System;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using MySqlConnector;
using mvc.Models;

namespace mvc.Repositories
{
    public class RepositorioPago : IRepositorioPago
    {
        private readonly string connectionString;

        public RepositorioPago(IConfiguration configuration)
        {
            connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException("No se encontró la cadena de conexión DefaultConnection.");
        }

        public List<Pago> ObtenerPorReserva(int idReserva)
        {
            var pagos = new List<Pago>();
            using var connection = new MySqlConnection(connectionString);
            connection.Open();

            string sql = @"
                SELECT 
                    p.id, p.id_reserva, p.concepto, p.fecha_pago, p.importe, p.activo, 
                    p.creado_por_user_id, p.anulado_por_user_id,
                    CONCAT('Reserva #', r.id) AS reserva_detalle
                FROM pago p
                INNER JOIN reserva r ON p.id_reserva = r.id
                WHERE p.id_reserva = @idReserva
                ORDER BY p.id DESC;";

            using var command = new MySqlCommand(sql, connection);
            command.Parameters.AddWithValue("@idReserva", idReserva);
            using var reader = command.ExecuteReader();

            while (reader.Read())
            {
                pagos.Add(MapearPago(reader));
            }

            return pagos;
        }

        public Pago? ObtenerPorId(int id)
        {
            using var connection = new MySqlConnection(connectionString);
            connection.Open();

            string sql = @"
                SELECT 
                    p.id, p.id_reserva, p.concepto, p.fecha_pago, p.importe, p.activo, 
                    p.creado_por_user_id, p.anulado_por_user_id,
                    CONCAT('Reserva #', r.id) AS reserva_detalle
                FROM pago p
                INNER JOIN reserva r ON p.id_reserva = r.id
                WHERE p.id = @id;";

            using var command = new MySqlCommand(sql, connection);
            command.Parameters.AddWithValue("@id", id);
            using var reader = command.ExecuteReader();

            if (reader.Read())
            {
                return MapearPago(reader);
            }

            return null;
        }

        public void Alta(Pago pago)
        {
            using var connection = new MySqlConnection(connectionString);
            connection.Open();

            string sql = @"
                INSERT INTO pago (
                    id_reserva, concepto, fecha_pago, importe, activo, creado_por_user_id
                ) VALUES (
                    @id_reserva, @concepto, @fecha_pago, @importe, @activo, @creado_por_user_id
                );";

            using var command = new MySqlCommand(sql, connection);
            command.Parameters.AddWithValue("@id_reserva", pago.IdReserva);
            command.Parameters.AddWithValue("@concepto", pago.Concepto);
            command.Parameters.AddWithValue("@fecha_pago", pago.Fecha);
            command.Parameters.AddWithValue("@importe", pago.Importe);
            command.Parameters.AddWithValue("@activo", pago.Activo);
            command.Parameters.AddWithValue("@creado_por_user_id", (object?)pago.CreadoPorUserId ?? DBNull.Value);

            command.ExecuteNonQuery();
        }

        public void ModificacionConcepto(int id, string concepto)
        {
            using var connection = new MySqlConnection(connectionString);
            connection.Open();

            string sql = "UPDATE pago SET concepto = @concepto WHERE id = @id;";

            using var command = new MySqlCommand(sql, connection);
            command.Parameters.AddWithValue("@id", id);
            command.Parameters.AddWithValue("@concepto", concepto);

            command.ExecuteNonQuery();
        }

        public void Anular(int id, int anuladoPorUserId)
        {
            using var connection = new MySqlConnection(connectionString);
            connection.Open();

            string sql = "UPDATE pago SET activo = 0, anulado_por_user_id = @anuladoPorUserId WHERE id = @id;";

            using var command = new MySqlCommand(sql, connection);
            command.Parameters.AddWithValue("@id", id);
            command.Parameters.AddWithValue("@anuladoPorUserId", anuladoPorUserId);

            command.ExecuteNonQuery();
        }

        private Pago MapearPago(MySqlDataReader reader)
        {
            return new Pago
            {
                Id = Convert.ToInt32(reader["id"]),
                IdReserva = Convert.ToInt32(reader["id_reserva"]),
                Concepto = reader["concepto"].ToString() ?? "",
                Fecha = Convert.ToDateTime(reader["fecha_pago"]),
                Importe = Convert.ToDecimal(reader["importe"]),
                Activo = Convert.ToInt32(reader["activo"]),
                CreadoPorUserId = reader["creado_por_user_id"] == DBNull.Value ? null : Convert.ToInt32(reader["creado_por_user_id"]),
                AnuladoPorUserId = reader["anulado_por_user_id"] == DBNull.Value ? null : Convert.ToInt32(reader["anulado_por_user_id"]),
                ReservaDetalle = reader["reserva_detalle"].ToString()
            };
        }
    }
}