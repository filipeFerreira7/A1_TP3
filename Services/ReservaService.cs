namespace A1_order_system.Services;
using A1_order_system.Data;
using A1_order_system.Dtos;
using A1_order_system.Entities;
using Microsoft.EntityFrameworkCore;
public class ReservaService
{
    private readonly RestauranteDbContext _context;

    public ReservaService(RestauranteDbContext context) => _context = context;

    public async Task<ReservaResponseDto> CriarReservaAsync(ReservaDto dto, long usuarioId)
    {
        // Regra: reserva apenas para jantar (19h–22h)
        var reserva = new Reserva
        {
            DataHora = dto.Horario.Date,
            NomeDoCliente = dto.NomeDoCliente,
            MesaId = dto.MesaId,
            UsuarioId = usuarioId
        };

        if (!reserva.HorarioValido())
            throw new InvalidOperationException(
                $"Reservas só são aceitas entre {Reserva.HorarioInicio:hh\\:mm} e {Reserva.HorarioFim:hh\\:mm}.");

        // Regra: deve ser feita com pelo menos 1 dia de antecedência (almoço)
        // Para jantar: a regra mencionada no doc é para almoço; reservas de jantar seguem horário
        if (dto.Horario.Date <= DateTime.UtcNow.Date)
            throw new InvalidOperationException("Reservas devem ser feitas com ao menos 1 dia de antecedência.");

        var mesa = await _context.Mesas.FindAsync(dto.MesaId)
            ?? throw new KeyNotFoundException("Mesa não encontrada.");

        // Verifica conflito de reserva para a mesma mesa no mesmo dia e horário
        bool conflito = await _context.Reservas.AnyAsync(r =>
            r.MesaId == dto.MesaId &&
            r.DataHora.Date == dto.Horario.Date);

        if (conflito)
            throw new InvalidOperationException("Mesa já reservada para esse dia.");



        _context.Reservas.Add(reserva);
        await _context.SaveChangesAsync();

        return new ReservaResponseDto(
            reserva.Id, reserva.DataHora,
            reserva.NomeDoCliente, mesa.Numero);
    }

    public async Task<List<ReservaResponseDto>> ListarReservasUsuarioAsync(long usuarioId)
    {
        return await _context.Reservas
            .Include(r => r.Mesa)
            .Where(r => r.UsuarioId == usuarioId)
            .OrderBy(r => r.DataHora)
            .Select(r => new ReservaResponseDto(
                r.Id, r.DataHora,
                r.NomeDoCliente, r.Mesa.Numero))
            .ToListAsync();
    }
}