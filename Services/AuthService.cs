using A1_order_system.Data;
using A1_order_system.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using A1_order_system.Dtos;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BCrypt.Net;
namespace A1_order_system.Services;

public class AuthService
{
    private readonly RestauranteDbContext _context;
    private readonly IConfiguration _config;

    public AuthService(RestauranteDbContext context, IConfiguration config)
    {
        _context = context;
        _config = config;
    }

    public async Task<TokenDto> CadastrarAsync(AuthDto dto)
    {
        if (await _context.Usuarios.AnyAsync(u => u.Email == dto.Email))
            throw new InvalidOperationException("E-mail já cadastrado.");

        var usuario = new Usuario
        {
            Nome = dto.Nome,
            Email = dto.Email,
            Senha = BCrypt.Net.BCrypt.HashPassword(dto.Senha),
            Perfil = PerfisUsuario.Cliente
        };

        _context.Usuarios.Add(usuario);
        await _context.SaveChangesAsync();
        return GerarToken(usuario);
    }

    public async Task<TokenDto> LoginAsync(LoginDto dto)
    {
        var usuario = await _context.Usuarios.FirstOrDefaultAsync(u => u.Email == dto.Email)
            ?? throw new UnauthorizedAccessException("Credenciais inválidas.");

        if (!BCrypt.Net.BCrypt.Verify(dto.Senha, usuario.Senha))
            throw new UnauthorizedAccessException("Credenciais inválidas.");

        if (string.Equals(usuario.Email, "admin@restaurante.com", StringComparison.OrdinalIgnoreCase)
            && usuario.Perfil != PerfisUsuario.Admin)
        {
            usuario.Perfil = PerfisUsuario.Admin;
            await _context.SaveChangesAsync();
        }

        return GerarToken(usuario);
    }

    private TokenDto GerarToken(Usuario usuario)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
            new Claim(ClaimTypes.Email, usuario.Email),
            new Claim(ClaimTypes.Name, usuario.Nome),
            new Claim(ClaimTypes.Role, usuario.Perfil)
        };

        var token = new JwtSecurityToken(
            issuer: _config["Jwt:Issuer"],
            audience: _config["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddHours(8),
            signingCredentials: creds);

        return new TokenDto(
            new JwtSecurityTokenHandler().WriteToken(token),
            usuario.Nome,
            usuario.Email,
            usuario.Perfil);
    }
}
