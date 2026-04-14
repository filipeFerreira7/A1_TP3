// ====================== RELATORIOS.JS ======================

async function loadRelatorios() {
    const inicio = document.getElementById('relInicio').value;
    const fim = document.getElementById('relFim').value;

    if (!inicio || !fim) {
        alert('Informe a data de início e fim.');
        return;
    }

    console.log(`Buscando relatórios de ${inicio} até ${fim}`);

    const [rFat, rMv] = await Promise.all([
        api('GET', `/api/relatorios/faturamento?inicio=${inicio}&fim=${fim}`, null, true),
        api('GET', `/api/relatorios/mais-vendidos?inicio=${inicio}&fim=${fim}`, null, true)
    ]);

    console.log('Resposta Faturamento:', rFat);
    console.log('Resposta Mais Vendidos:', rMv);

    // Faturamento por Tipo
    const fat = rFat.ok ? (rFat.data || []) : [];
    console.log('Dados de faturamento recebidos:', fat);

    const maxFat = Math.max(...fat.map(f => f.total || f.faturamento || f.valor || 0), 1);

    document.getElementById('relFaturamento').innerHTML = fat.length ? `
        <div class="bar-chart">
            ${fat.map(f => {
        const v = f.total || f.faturamento || f.valor || 0;
        return `
                    <div class="bar-row">
                        <div class="bar-label">${f.tipoAtendimento || f.tipo || f.nome || '–'}</div>
                        <div class="bar-track"><div class="bar-fill" style="width:${(v / maxFat * 100).toFixed(0)}%"></div></div>
                        <div class="bar-val">R$ ${Number(v).toFixed(2)}</div>
                    </div>`;
    }).join('')}
        </div>` : '<div class="empty-state" style="padding:20px 0"><div class="empty-text">Sem dados de faturamento neste período</div></div>';

    // Itens Mais Vendidos
    const mv = rMv.ok ? (rMv.data || []) : [];
    const maxMv = Math.max(...mv.map(m => m.quantidadeVendida || m.quantidade || 0), 1);

    document.getElementById('relMaisVendidos').innerHTML = mv.length ? `
        <div class="bar-chart">
            ${mv.slice(0, 8).map(m => {
        const q = m.quantidadeVendida || m.quantidade || 0;
        return `
                    <div class="bar-row">
                        <div class="bar-label">${m.nomeItem || m.nome || '–'}</div>
                        <div class="bar-track">
                            <div class="bar-fill" style="width:${(q / maxMv * 100).toFixed(0)}%; background:${m.vezesComoSugestao > 0 ? '#e74c3c' : 'var(--gold)'}"></div>
                        </div>
                        <div class="bar-val">${q}x${m.vezesComoSugestao > 0 ? ` ⭑${m.vezesComoSugestao}` : ''}</div>
                    </div>`;
    }).join('')}
        </div>` : '<div class="empty-state" style="padding:20px 0"><div class="empty-text">Sem itens vendidos neste período</div></div>';
}