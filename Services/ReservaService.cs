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
        DateTime horarioReserva = dto.Horario;

        if (horarioReserva.Kind == DateTimeKind.Unspecified)
        {
            horarioReserva = DateTime.SpecifyKind(horarioReserva, DateTimeKind.Local);
        }

        var horarioLocal = horarioReserva.ToLocalTime();

        Console.WriteLine($"[RESERVA DEBUG] Recebido do frontend: {horarioReserva:yyyy-MM-dd HH:mm:ss} (Kind: {horarioReserva.Kind})");
        Console.WriteLine($"[RESERVA DEBUG] Convertido para Local: {horarioLocal:yyyy-MM-dd HH:mm:ss}");

        var hora = horarioLocal.TimeOfDay;

        if (hora < new TimeSpan(19, 0, 0) || hora > new TimeSpan(22, 0, 0))
        {
            throw new InvalidOperationException(
                $"Reservas só são aceitas entre 19:00 e 22:00. Horário informado: {horarioLocal:HH:mm}");
        }

        var hojeLocal = DateTime.Now.Date;

        if (horarioLocal.Date <= hojeLocal)
        {
            throw new InvalidOperationException("A reserva deve ser feita com pelo menos 1 dia de antecedência.");
        }

        bool conflito = await _context.Reservas.AnyAsync(r =>
            r.MesaId == dto.MesaId &&
            r.DataHora.Date == horarioLocal.Date);

        if (conflito)
            throw new InvalidOperationException("Esta mesa já está reservada para esse dia.");

        var mesa = await _context.Mesas.FindAsync(dto.MesaId)
            ?? throw new KeyNotFoundException("Mesa não encontrada.");

        var reserva = new Reserva
        {
            DataHora = horarioLocal,
            NomeDoCliente = dto.NomeDoCliente?.Trim() ?? "Sem nome",
            MesaId = dto.MesaId,
            UsuarioId = usuarioId
        };

        _context.Reservas.Add(reserva);
        await _context.SaveChangesAsync();

        Console.WriteLine($"[RESERVA SUCESSO] Reserva criada para {horarioLocal:HH:mm}");

        return new ReservaResponseDto(
            reserva.Id,
            reserva.DataHora,
            reserva.NomeDoCliente,
            mesa.Numero);
    }
    public async Task<List<ReservaResponseDto>> ListarReservasAsync(long usuarioId)
    {
        return await _context.Reservas
            .Include(r => r.Mesa)
            .Where(r => r.UsuarioId == usuarioId)
            .OrderBy(r => r.DataHora)
            .Select(r => new ReservaResponseDto(
                r.Id,
                r.DataHora,
                r.NomeDoCliente,
                r.Mesa.Numero))
            .ToListAsync();
    }
}
