using mvc.Models;

namespace mvc.Repositorios
{
    public interface IRepositorioUsuario
    {
        Usuario? ObtenerPorEmail(string email);

        Usuario? ObtenerPorNombreUsuario(string nombreUsuario);

        Usuario? ObtenerPorId(int id);

        List<Usuario> ObtenerTodos();

        int Alta(Usuario usuario);

        int Modificacion(Usuario usuario);

        int Baja(int id);
    }
}