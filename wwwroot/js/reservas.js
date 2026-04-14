// ====================== RESERVAS.JS ======================

async function loadReservas() {
    const container = document.getElementById('reservasList');
    if (!container) return;

    container.innerHTML = '<span class="spinner"></span>';

    try {
        const r = await api('GET', '/api/reservas', null, true);
        const list = r.ok ? (r.data || []) : [];

        if (list.length === 0) {
            container.innerHTML = `
                <div class="empty-state">
                    <div class="empty-icon">⊟</div>
                    <div class="empty-text">Nenhuma reserva encontrada</div>
                </div>`;
            return;
        }

        let html = '';

        for (const res of list) {
            let d;
            try {
                // Tenta vários possíveis nomes de campo que o backend pode retornar
                const dateStr = res.dataHora || res.horario || res.DataHora || res.data;
                d = dateStr ? new Date(dateStr) : new Date();

                if (isNaN(d.getTime())) {
                    d = new Date(); // fallback
                }
            } catch (e) {
                d = new Date();
            }

            const day = d.getDate().toString().padStart(2, '0');
            const monthShort = d.toLocaleString('pt-BR', { month: 'short' }).replace('.', '');
            const time = d.toLocaleTimeString('pt-BR', { hour: '2-digit', minute: '2-digit' });

            html += `
                <div class="reserva-card">
                    <div class="reserva-date">
                        <div class="reserva-day">${day}</div>
                        <div class="reserva-month">${monthShort}</div>
                    </div>
                    <div class="reserva-divider"></div>
                    <div class="reserva-info">
                        <div class="reserva-name">${res.nomeDoCliente || '–'}</div>
                        <div class="reserva-detail">
                            Mesa ${res.mesaNumero || res.mesaId || '–'} · ${time}
                        </div>
                    </div>
                    <span class="stat-badge badge-gold">#${res.id}</span>
                </div>`;
        }

        container.innerHTML = html;

    } catch (error) {
        console.error('Erro ao carregar reservas:', error);
        container.innerHTML = `
            <div class="alert alert-error" style="margin:20px;">
                Erro ao carregar reservas. Tente novamente.
            </div>`;
    }
}

async function submitReserva() {
    const nome = document.getElementById('rNome').value.trim();
    const horarioStr = document.getElementById('rHorario').value;
    const mesaId = parseInt(document.getElementById('rMesa').value);

    if (!nome || !horarioStr || isNaN(mesaId)) {
        showAlert('novaReservaAlert', 'Preencha todos os campos obrigatórios.', 'error');
        return;
    }

    document.getElementById('reservaSpinner').style.display = '';
    clearAlert('novaReservaAlert');

    const r = await api('POST', '/api/reservas', {
        nomeDoCliente: nome,
        horario: horarioStr,
        mesaId: mesaId
    }, true);

    document.getElementById('reservaSpinner').style.display = 'none';

    if (r.ok) {
        closeModal('modalNovaReserva');
        loadReservas();                    // Atualiza a lista

        // Limpa formulário
        document.getElementById('rNome').value = '';
        document.getElementById('rHorario').value = '';
        document.getElementById('rMesa').value = '';

        showAlert('novaReservaAlert', 'Reserva realizada com sucesso!', 'success');
    } else {
        showAlert('novaReservaAlert', r.data?.erro || 'Erro ao criar reserva.', 'error');
    }
}

// Expor as funções para o onclick do HTML
window.loadReservas = loadReservas;
window.submitReserva = submitReserva;