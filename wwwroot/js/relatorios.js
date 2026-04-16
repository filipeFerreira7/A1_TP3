function formatCurrency(value) {
    return `R$ ${Number(value || 0).toFixed(2)}`;
}

async function loadRelatorios() {
    const inicio = document.getElementById('relInicio').value;
    const fim = document.getElementById('relFim').value;

    if (!inicio || !fim) {
        alert('Informe a data de inicio e fim.');
        return;
    }

    const [rFat, rMv] = await Promise.all([
        api('GET', `/api/relatorios/faturamento?inicio=${inicio}&fim=${fim}`, null, true),
        api('GET', `/api/relatorios/mais-vendidos?inicio=${inicio}&fim=${fim}`, null, true)
    ]);

    const fat = rFat.ok ? (rFat.data || []) : [];
    const totalFaturado = fat.reduce((sum, item) => sum + Number(item.totalFaturado || 0), 0);
    const totalTaxas = fat.reduce((sum, item) => sum + Number(item.totalTaxas || 0), 0);
    const totalLiquido = fat.reduce((sum, item) => sum + Number(item.receitaLiquida || 0), 0);
    const taxaDeliveryApp = fat
        .filter(item => item.tipoAtendimento === 'DeliveryApp')
        .reduce((sum, item) => sum + Number(item.totalTaxas || 0), 0);
    const taxaDeliveryProprio = fat
        .filter(item => item.tipoAtendimento === 'DeliveryProprio')
        .reduce((sum, item) => sum + Number(item.totalTaxas || 0), 0);
    const maxFat = Math.max(...fat.map(f => Number(f.totalFaturado || 0)), 1);

    document.getElementById('relFaturamento').innerHTML = fat.length ? `
        <div style="display:grid;grid-template-columns:repeat(auto-fit,minmax(220px,1fr));gap:16px;margin-bottom:20px;">
            <div style="padding:22px;border-radius:20px;background:linear-gradient(135deg,#163a24,#235c37);border:1px solid rgba(74,222,128,0.22);box-shadow:0 18px 45px rgba(10,20,15,0.22);">
                <div style="font-size:13px;letter-spacing:1.4px;text-transform:uppercase;color:#9fe3b4;margin-bottom:10px;">Total Faturado</div>
                <div style="font-size:34px;line-height:1.05;font-weight:700;color:#f3fff5;">${formatCurrency(totalFaturado)}</div>
                <div style="font-size:13px;color:#bde7ca;margin-top:8px;">Receita bruta do periodo</div>
            </div>
            <div style="padding:22px;border-radius:20px;background:linear-gradient(135deg,#4a1717,#7a2222);border:1px solid rgba(248,113,113,0.22);box-shadow:0 18px 45px rgba(30,10,10,0.22);">
                <div style="font-size:13px;letter-spacing:1.4px;text-transform:uppercase;color:#f7b0b0;margin-bottom:10px;">Despesas com Taxas</div>
                <div style="font-size:34px;line-height:1.05;font-weight:700;color:#fff4f4;">${formatCurrency(totalTaxas)}</div>
                <div style="font-size:13px;color:#f0baba;margin-top:8px;">Delivery app + delivery proprio</div>
            </div>
            <div style="padding:22px;border-radius:20px;background:linear-gradient(135deg,#16273f,#214b78);border:1px solid rgba(96,165,250,0.22);box-shadow:0 18px 45px rgba(10,18,30,0.22);">
                <div style="font-size:13px;letter-spacing:1.4px;text-transform:uppercase;color:#b4d7ff;margin-bottom:10px;">Resultado Liquido</div>
                <div style="font-size:34px;line-height:1.05;font-weight:700;color:#f5faff;">${formatCurrency(totalLiquido)}</div>
                <div style="font-size:13px;color:#c5def8;margin-top:8px;">Faturamento menos taxas</div>
            </div>
        </div>
        <div style="display:grid;grid-template-columns:repeat(auto-fit,minmax(180px,1fr));gap:12px;margin-bottom:22px;">
            <div style="padding:16px 18px;border-radius:16px;background:#10171f;border:1px solid rgba(255,255,255,0.06);">
                <div style="font-size:12px;text-transform:uppercase;letter-spacing:1.2px;color:var(--text3);margin-bottom:6px;">Taxa Delivery App</div>
                <div style="font-size:24px;font-weight:700;color:#f87171;">${formatCurrency(taxaDeliveryApp)}</div>
            </div>
            <div style="padding:16px 18px;border-radius:16px;background:#10171f;border:1px solid rgba(255,255,255,0.06);">
                <div style="font-size:12px;text-transform:uppercase;letter-spacing:1.2px;color:var(--text3);margin-bottom:6px;">Taxa Delivery Proprio</div>
                <div style="font-size:24px;font-weight:700;color:#fb7185;">${formatCurrency(taxaDeliveryProprio)}</div>
            </div>
        </div>
        <div style="display:flex;flex-direction:column;gap:14px;">
            ${fat.map(f => {
        const total = Number(f.totalFaturado || 0);
        const taxas = Number(f.totalTaxas || 0);
        const liquido = Number(f.receitaLiquida || 0);
        const pedidos = Number(f.totalPedidos || 0);
        const accent = f.tipoAtendimento === 'Presencial' ? '#4ade80' : f.tipoAtendimento === 'DeliveryProprio' ? '#fb7185' : '#f87171';
        return `
                <div style="padding:18px 20px;border-radius:18px;background:#0f151c;border:1px solid rgba(255,255,255,0.06);box-shadow:0 10px 25px rgba(0,0,0,0.12);">
                    <div style="display:flex;justify-content:space-between;align-items:flex-start;gap:16px;flex-wrap:wrap;">
                        <div>
                            <div style="font-size:22px;font-weight:700;color:#f7fafc;">${f.tipoAtendimento}</div>
                            <div style="font-size:13px;color:var(--text3);margin-top:4px;">${pedidos} pedidos no periodo</div>
                        </div>
                        <div style="min-width:180px;">
                            <div style="height:10px;background:rgba(255,255,255,0.06);border-radius:999px;overflow:hidden;">
                                <div style="width:${(total / maxFat * 100).toFixed(0)}%;height:100%;background:${accent};border-radius:999px;"></div>
                            </div>
                        </div>
                    </div>
                    <div style="display:grid;grid-template-columns:repeat(auto-fit,minmax(140px,1fr));gap:14px;margin-top:16px;">
                        <div>
                            <div style="font-size:12px;text-transform:uppercase;letter-spacing:1px;color:var(--text3);">Faturamento</div>
                            <div style="font-size:22px;font-weight:700;color:#4ade80;margin-top:6px;">${formatCurrency(total)}</div>
                        </div>
                        <div>
                            <div style="font-size:12px;text-transform:uppercase;letter-spacing:1px;color:var(--text3);">Taxas</div>
                            <div style="font-size:22px;font-weight:700;color:#f87171;margin-top:6px;">${formatCurrency(taxas)}</div>
                        </div>
                        <div>
                            <div style="font-size:12px;text-transform:uppercase;letter-spacing:1px;color:var(--text3);">Liquido</div>
                            <div style="font-size:22px;font-weight:700;color:#93c5fd;margin-top:6px;">${formatCurrency(liquido)}</div>
                        </div>
                    </div>
                </div>`;
    }).join('')}
        </div>` : '<div class="empty-state" style="padding:30px 0"><div class="empty-text">Sem dados de faturamento neste periodo</div></div>';

    const mv = rMv.ok ? (rMv.data || []) : [];
    const maxMv = Math.max(...mv.map(m => m.quantidadeVendida || 0), 1);

    document.getElementById('relMaisVendidos').innerHTML = mv.length ? `
        <div style="display:flex;flex-direction:column;gap:14px;">
            ${mv.slice(0, 8).map(m => {
        const q = Number(m.quantidadeVendida || 0);
        const comoSugestao = Number(m.quantidadeComoSugestao || 0);
        return `
                <div style="padding:18px 20px;border-radius:18px;background:#0f151c;border:1px solid rgba(255,255,255,0.06);box-shadow:0 10px 25px rgba(0,0,0,0.12);">
                    <div style="display:flex;justify-content:space-between;align-items:center;gap:14px;flex-wrap:wrap;">
                        <div>
                            <div style="font-size:20px;font-weight:700;color:#f8fafc;">${m.nomeItem || m.nome || '-'}</div>
                            <div style="font-size:13px;color:var(--text3);margin-top:4px;">${m.periodo === 0 ? 'Almoco' : 'Jantar'}</div>
                        </div>
                        <div style="font-size:24px;font-weight:700;color:#4ade80;">${q}x</div>
                    </div>
                    <div style="margin-top:14px;height:10px;background:rgba(255,255,255,0.06);border-radius:999px;overflow:hidden;">
                        <div style="width:${(q / maxMv * 100).toFixed(0)}%;height:100%;background:${comoSugestao > 0 ? '#f87171' : '#4ade80'};border-radius:999px;"></div>
                    </div>
                    <div style="display:flex;justify-content:space-between;align-items:center;margin-top:10px;font-size:13px;color:var(--text3);">
                        <span>Quantidade vendida</span>
                        <span>${comoSugestao > 0 ? `Sugestao em ${comoSugestao} itens` : 'Sem desconto de sugestao'}</span>
                    </div>
                </div>`;
    }).join('')}
        </div>` : '<div class="empty-state" style="padding:30px 0"><div class="empty-text">Sem itens vendidos neste periodo</div></div>';
}
