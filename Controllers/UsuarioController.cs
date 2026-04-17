namespace A1_order_system.Controllers;

using A1_order_system.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/usuarios")]
[Authorize(Roles = "Admin")]
public class UsuarioController : ControllerBase
{
    private readonly RestauranteDbContext _db;

    public UsuarioController(RestauranteDbContext db) => _db = db;

    [HttpGet]
    public IActionResult ListarTodos()
    {
        var usuarios = _db.Usuarios
            .Select(u => new
            {
                u.Id,
                u.Nome,
                u.Email,
                u.Perfil
            })
            .ToList();

        return Ok(usuarios);
    }
}
