async function loadDashboard() {
    try {
        const storedUser = JSON.parse(localStorage.getItem('currentUser') || '{}');
        const isAdmin = storedUser?.perfil === 'Admin';
        const pedidosEndpoint = isAdmin ? '/api/pedidos/todos' : '/api/pedidos';
        
        const [rCard, rSug, rPed, rRes, rEnd] = await Promise.all([
            api('GET', '/api/cardapio'),
            api('GET', '/api/cardapio/sugestoes-hoje'),
            api('GET', pedidosEndpoint, null, true),
            api('GET', '/api/reservas', null, true),
            api('GET', '/api/enderecos', null, true)
        ]);

        // Estatísticas
        document.getElementById('statCardapio').textContent = rCard.ok ? (rCard.data?.length || 0) : '–';
        document.getElementById('statPedidos').textContent = rPed.ok ? (rPed.data?.length || 0) : '–';
        document.getElementById('statReservas').textContent = rRes.ok ? (rRes.data?.length || 0) : '–';
        document.getElementById('statEnderecos').textContent = rEnd.ok ? (rEnd.data?.length || 0) : '–';

        // Sugestões do Chefe
        const sug = rSug.ok && rSug.data ? rSug.data : [];

        const sugestoesHTML = sug.length ? sug.map(s => `
            <div style="display:flex;align-items:center;gap:12px;padding:12px 0;border-bottom:1px solid rgba(255,255,255,0.05);">
                <span class="menu-badge menu-badge-chef" style="background:#c0392b; color:white; font-weight:600;">
                    ★ Sugestão do Chefe
                </span>
                <span style="font-size:14px;font-weight:500;flex:1;color:var(--text)">
                    ${s.nomeItem || s.nome || 'Item não identificado'}
                </span>
                <div style="text-align:right;">
                    ${s.precoComDesconto && s.precoComDesconto > 0
                ? `<span style="font-size:15px;color:var(--gold);font-weight:600;">
                                R$ ${Number(s.precoComDesconto).toFixed(2)}
                           </span>
                           <small style="display:block;color:#27ae60;font-size:11px;">20% OFF</small>`
                : `<span style="font-size:15px;color:var(--gold);font-weight:600;">
                                R$ ${Number(s.precoBase || 0).toFixed(2)}
                           </span>`}
                </div>
            </div>
        `).join('') : `
            <div class="empty-state" style="padding:40px 20px; text-align:center;">
                <div class="empty-icon" style="font-size:48px; margin-bottom:12px;">🍽️</div>
                <div class="empty-text">Nenhuma Sugestão do Chefe hoje</div>
                <div class="empty-sub" style="font-size:13px; color:var(--text3);">
                    Volte mais tarde ou verifique com o administrador
                </div>
            </div>`;

        document.getElementById('dashSugestoes').innerHTML = sugestoesHTML;

        // Últimos Pedidos
        const peds = rPed.ok && rPed.data ? rPed.data.slice(-5).reverse() : [];

        const pedidosHTML = peds.length ? peds.map(p => `
            <div style="display:flex;align-items:center;gap:10px;padding:10px 0;border-bottom:1px solid rgba(255,255,255,0.05);">
                <span style="font-size:11px;color:var(--text3)">#${p.id}</span>
                <span style="font-size:12px;color:var(--text2);flex:1;">
                    ${p.tipoAtendimento || 'Presencial'} · ${p.periodo === 0 ? 'Almoço' : 'Jantar'}
                </span>
                <span style="font-size:13px;color:var(--gold);font-weight:600;">
                    R$ ${Number(p.valorTotal || 0).toFixed(2)}
                </span>
            </div>
        `).join('') : `
            <div class="empty-state" style="padding:30px 20px;">
                <div class="empty-text">Nenhum pedido recente</div>
            </div>`;

        document.getElementById('dashPedidos').innerHTML = pedidosHTML;

    } catch (error) {
        console.error('Erro ao carregar dashboard:', error);
        document.getElementById('dashSugestoes').innerHTML =
            '<div class="alert alert-error">Erro ao carregar o dashboard</div>';
    }
}