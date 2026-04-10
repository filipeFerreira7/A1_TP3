namespace A1_order_system.Services;
using A1_order_system.Data;
using A1_order_system.Dtos;
using A1_order_system.Entities;
using Microsoft.EntityFrameworkCore;

public class EnderecoService
{
    private readonly RestauranteDbContext _context;

    public EnderecoService(RestauranteDbContext context) => _context = context;

    public async Task<EnderecoResponseDto> AdicionarAsync(EnderecoDto dto, long usuarioId)
    {
        var endereco = new Endereco
        {
            Logradouro = dto.Logradouro,
            Cidade = dto.Cidade,
            Estado = dto.Estado,
            UsuarioId = usuarioId
        };

        _context.Enderecos.Add(endereco);
        await _context.SaveChangesAsync();

        return new EnderecoResponseDto(endereco.Id, endereco.Logradouro, endereco.Cidade, endereco.Estado);
    }

    public async Task<List<EnderecoResponseDto>> ListarAsync(long usuarioId)
    {
        return await _context.Enderecos
            .Where(e => e.UsuarioId == usuarioId)
            .Select(e => new EnderecoResponseDto(e.Id, e.Logradouro, e.Cidade, e.Estado))
            .ToListAsync();
    }

    public async Task RemoverAsync(long enderecoId, long usuarioId)
    {
        var endereco = await _context.Enderecos
            .FirstOrDefaultAsync(e => e.Id == enderecoId && e.UsuarioId == usuarioId)
            ?? throw new KeyNotFoundException("Endereço não encontrado.");

        _context.Enderecos.Remove(endereco);
        await _context.SaveChangesAsync();
    }
}