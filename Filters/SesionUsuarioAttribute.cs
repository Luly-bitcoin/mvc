using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace mvc.Filters
{
    public class SesionUsuarioAttribute : ActionFilterAttribute
    {
        public string? RolRequerido { get; set; }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var session = context.HttpContext.Session;

            var usuarioId = session.GetInt32("UsuarioId");

            if (usuarioId == null)
            {
                context.Result = new RedirectToActionResult(
                    "Index",
                    "Login",
                    null
                );

                return;
            }

            if (!string.IsNullOrEmpty(RolRequerido))
            {
                var rol = session.GetString("Rol");

                if (!string.Equals(
                    rol,
                    RolRequerido,
                    StringComparison.OrdinalIgnoreCase))
                {
                    context.Result = new RedirectToActionResult(
                        "AccesoDenegado",
                        "Login",
                        null
                    );

                    return;
                }
            }

            base.OnActionExecuting(context);
        }
    }
}