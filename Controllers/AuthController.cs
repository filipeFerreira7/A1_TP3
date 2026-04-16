namespace A1_order_system.Controllers;
using A1_order_system.Dtos;
using A1_order_system.Services;
using Microsoft.AspNetCore.Mvc;


[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly AuthService _authService;

    public AuthController(AuthService authService) => _authService = authService;

    [HttpPost("cadastro")]
    public async Task<IActionResult> Cadastrar([FromBody] AuthDto dto)
    {
        try
        {
            var token = await _authService.CadastrarAsync(dto);
            return Ok(token);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { erro = ex.Message });
        }
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto dto)
    {
        try
        {
            var token = await _authService.LoginAsync(dto);
            return Ok(token);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new { erro = ex.Message });
        }
    }
}
