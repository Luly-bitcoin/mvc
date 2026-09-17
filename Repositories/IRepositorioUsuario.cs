using System.Collections.Generic;
using mvc.Models;

namespace mvc.Repositories
{
    public interface IRepositorioUsuario
    {
        Usuario? ObtenerPorEmail(string email);
        Usuario? ObtenerPorNombreUsuario(string nombreUsuario);
        Usuario? ObtenerPorId(int id);
        List<Usuario> ObtenerTodos();
        IEnumerable<Usuario> ObtenerPaginado(
            int pagina,
            int cantidadPorPagina,
            string? busqueda = null
        );
        int Alta(Usuario usuario);
        int Modificacion(Usuario usuario);
        int Baja(int id);
    }
}