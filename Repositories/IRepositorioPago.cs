using System.Collections.Generic;
using mvc.Models;

namespace mvc.Repositories
{
    public interface IRepositorioPago
    {
        List<Pago> ObtenerTodos();
        List<Pago> ObtenerPorReserva(int idReserva);
        Pago? ObtenerPorId(int id);
        void Alta(Pago pago);
        void ModificacionConcepto(int id, string concepto);
        void Anular(int id, int anuladoPorUserId);
    }
}