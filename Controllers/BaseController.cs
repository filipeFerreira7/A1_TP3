namespace A1_order_system.Controllers;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;


public abstract class BaseController : ControllerBase
{
    protected long UsuarioIdLogado =>
        long.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? throw new UnauthorizedAccessException("Usuário não autenticado."));

    protected string PerfilUsuarioLogado =>
        User.FindFirstValue(ClaimTypes.Role)
            ?? "Cliente";
}
