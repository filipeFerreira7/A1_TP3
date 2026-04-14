// ====================== CARDAPIO.JS ======================

async function loadCardapio() {
    const grid = document.getElementById('cardapioGrid');
    grid.innerHTML = '<div class="spinner" style="margin:40px auto;display:block;"></div>';

    const r = await api('GET', '/api/cardapio');

    if (!r.ok) {
        grid.innerHTML = `<div class="alert alert-error">Erro ao carregar cardápio: ${r.data?.erro || 'Erro desconhecido'}</div>`;
        return;
    }

    cardapioData = r.data || [];
    renderCardapio();
}

function filterCardapio(periodo, el) {
    document.querySelectorAll('.filter-pills .pill').forEach(p => p.classList.remove('active'));
    el.classList.add('active');
    currentFilter = periodo;
    renderCardapio();
}

function renderCardapio() {
    const grid = document.getElementById('cardapioGrid');
    const items = currentFilter === null
        ? cardapioData
        : cardapioData.filter(i => i.periodo === currentFilter);

    if (items.length === 0) {
        grid.innerHTML = '<div class="empty-state"><div class="empty-text">Nenhum item encontrado</div></div>';
        return;
    }

    grid.innerHTML = items.map(item => `
        <div class="menu-item-card">
            <div style="display:flex;align-items:center;gap:8px;margin-bottom:6px;">
                <span class="menu-item-name">${item.nome}</span>
                ${item.isSugestaoChefe ? `
                    <span class="menu-badge menu-badge-chef" style="background:#c0392b; color:white; font-weight:600;">
                        ★ Sugestão do Chefe
                    </span>` : ''}
            </div>
            <div class="menu-item-desc">${item.descricao || ''}</div>
            
            ${item.ingredientes?.length ? `
                <div style="font-size:11px;color:var(--text3);margin-bottom:8px;">
                    ${item.ingredientes.join(', ')}
                </div>` : ''}
            
            <div class="menu-item-footer">
                <span class="menu-price">
                    ${item.precoComDesconto
            ? `
                            <s style="color:var(--text3);font-size:12px;">R$ ${Number(item.precoBase).toFixed(2)}</s> 
                            <strong style="color:var(--gold);">R$ ${Number(item.precoComDesconto).toFixed(2)}</strong>
                            <small style="color:#27ae60; margin-left:4px;">(20% OFF)</small>
                          `
            : `R$ ${Number(item.precoBase).toFixed(2)}`}
                </span>
                <span class="menu-badge ${item.periodo === 0 ? 'menu-badge-almoco' : 'menu-badge-jantar'}">
                    ${item.periodo === 0 ? 'Almoço' : 'Jantar'}
                </span>
            </div>
        </div>
    `).join('');
}

// Função para carregar sugestões do chefe (pode ser chamada no Dashboard também)
async function loadSugestoesChefe() {
    const r = await api('GET', '/api/cardapio/sugestoes-hoje');
    if (r.ok && r.data) {
        // Você pode usar isso em outro lugar se quiser atualizar em tempo real
        console.log('Sugestões do Chefe hoje:', r.data);
    }
}

async function submitNovoItem() {
    const nome = document.getElementById('iNome').value.trim();
    const descricao = document.getElementById('iDesc').value.trim();
    const precoBase = parseFloat(document.getElementById('iPreco').value);
    const periodo = parseInt(document.getElementById('iPeriodo').value);

    if (!nome || isNaN(precoBase) || precoBase <= 0) {
        showAlert('novoItemAlert', 'Nome e preço válido são obrigatórios.');
        return;
    }

    document.getElementById('novoItemSpinner').style.display = '';

    const r = await api('POST', '/api/cardapio', {
        nome,
        descricao,
        precoBase,
        periodo,
        ingredienteIds: []
    }, true);

    document.getElementById('novoItemSpinner').style.display = 'none';

    if (r.ok) {
        closeModal('modalNovoItem');
        loadCardapio();                    // Atualiza a lista
        showAlert('novoItemAlert', 'Item criado com sucesso!', 'success');

        // Limpa formulário
        ['iNome', 'iDesc', 'iPreco'].forEach(id => {
            const el = document.getElementById(id);
            if (el) el.value = '';
        });
    } else {
        showAlert('novoItemAlert', r.data?.erro || 'Erro ao criar item.');
    }
}