
async function loadSugestoesChefe() {
    const container = document.getElementById('sugestoesList');
    if (!container) return;

    container.innerHTML = '<div class="spinner" style="margin:80px auto; display:block;"></div>';

    try {
        const r = await api('GET', '/api/cardapio/sugestoes-hoje', null, true);

        if (!r.ok) {
            container.innerHTML = `
                <div class="alert alert-error" style="grid-column: 1 / -1;">
                    Erro ao carregar sugestões: ${r.data?.erro || 'Erro no servidor'}
                </div>`;
            return;
        }

        const sugestoes = r.data || [];

        if (sugestoes.length === 0) {
            container.innerHTML = `
                <div class="empty-state" style="grid-column: 1 / -1; padding:80px 20px; text-align:center;">
                 <div style="font-size:64px; margin-bottom:16px; opacity:0.6;">⭐</div>
                    <div class="empty-text">Nenhuma Sugestão do Chefe definida hoje</div>
                    <div style="color:var(--text3); margin-top:8px;">Clique em "+ Nova Sugestão" para definir.</div>
                </div>`;
            return;
        }

        container.innerHTML = sugestoes.map(s => `
            <div class="card">
                <div style="display:flex; justify-content:space-between; align-items:start;">
                    <span class="menu-badge menu-badge-chef" style="background:#c0392b; color:white; font-weight:600;">
                        ★ Sugestão do Chefe
                    </span>
                    <span class="menu-badge ${s.periodo === 0 ? 'menu-badge-almoco' : 'menu-badge-jantar'}">
                        ${s.periodo === 0 ? 'Almoço' : 'Jantar'}
                    </span>
                </div>
                
                <h3 style="margin:12px 0 8px; color:var(--text);">${s.nomeItem || s.nome || 'Item'}</h3>
                
                <div style="display:flex; align-items:center; gap:12px; margin-top:16px;">
                    <span style="font-size:18px; font-weight:600; color:var(--gold);">
                        R$ ${Number(s.precoComDesconto || 0).toFixed(2)}
                    </span>
                    <small style="color:#27ae60; font-weight:500;">(20% OFF)</small>
                </div>
                
                <small style="color:var(--text3); margin-top:8px; display:block;">
                    Definida em ${new Date(s.data).toLocaleDateString('pt-BR')}
                </small>
            </div>
        `).join('');

    } catch (err) {
        console.error(err);
        container.innerHTML = `<div class="alert alert-error" style="grid-column: 1 / -1;">Falha ao carregar sugestões.</div>`;
    }
}
async function openNovaSugestaoModal() {
    const modalBody = document.getElementById('modalSugestaoBody');
    const alertDiv = document.getElementById('modalSugestaoAlert');

    if (alertDiv) alertDiv.innerHTML = '';

    if (!modalBody) {
        console.error("ERRO: Elemento #modalSugestaoBody não encontrado no HTML!");
        alert("Erro interno: modalSugestaoBody não encontrado. Verifique o index.html");
        return;
    }

    modalBody.innerHTML = '<div class="spinner" style="margin:60px auto; display:block;"></div>';

    try {
        const r = await api('GET', '/api/cardapio', null, true);

        if (!r.ok) {
            throw new Error(r.data?.erro || 'Falha ao buscar cardápio');
        }

        const items = r.data || [];

        modalBody.innerHTML = `
            <div class="form-group">
                <label class="form-label">Período</label>
                <select class="form-control" id="sugestaoPeriodo">
                    <option value="0">Almoço</option>
                    <option value="1">Jantar</option>
                </select>
            </div>
            <div class="form-group">
                <label class="form-label">Selecione um prato</label>
                <div id="sugestaoItemGrid" style="display:grid; grid-template-columns:repeat(auto-fill,minmax(200px,1fr)); gap:12px; margin-top:8px;"></div>
                <input type="hidden" id="sugestaoItemId" value="">
            </div>
        `;

        function renderItemGrid(periodoFilter) {
            const grid = document.getElementById('sugestaoItemGrid');
            const filteredItems = items.filter(i => i.periodo == periodoFilter);

            grid.innerHTML = filteredItems.length ? filteredItems.map(item => `
                <div class="menu-item-card" id="sg-item-${item.id}" onclick="selectSugestaoItem(${item.id}, ${item.periodo})" style="cursor:pointer;">
                    <div class="menu-item-name">${item.nome}</div>
                    <div class="menu-item-desc">${item.descricao || ''}</div>
                    <div class="menu-item-footer">
                        <span class="menu-price">R$ ${Number(item.precoBase).toFixed(2)}</span>
                        <span class="menu-badge ${item.periodo === 0 ? 'menu-badge-almoco' : 'menu-badge-jantar'}">
                            ${item.periodo === 0 ? 'Almoço' : 'Jantar'}
                        </span>
                    </div>
                </div>
            `).join('') : '<div style="grid-column:1/-1;text-align:center;padding:20px;color:var(--text3);">Nenhum prato para este período</div>';
        }

        window.selectSugestaoItem = function(id, periodo) {
            document.querySelectorAll('#sugestaoItemGrid .menu-item-card').forEach(el => el.classList.remove('selected'));
            const card = document.getElementById(`sg-item-${id}`);
            if (card) card.classList.add('selected');
            document.getElementById('sugestaoItemId').value = id;
        };

        const periodoSelect = document.getElementById('sugestaoPeriodo');
        periodoSelect.addEventListener('change', () => renderItemGrid(periodoSelect.value));
        renderItemGrid(periodoSelect.value);

        openModal('modalNovaSugestao');

    } catch (err) {
        console.error(err);
        modalBody.innerHTML = `
            <div class="alert alert-error">
                Erro ao carregar os pratos: ${err.message}
            </div>`;
    }
}

async function submitNovaSugestao() {
    const periodo = parseInt(document.getElementById('sugestaoPeriodo').value);
    const itemCardapioId = parseInt(document.getElementById('sugestaoItemId').value);

    if (!itemCardapioId) {
        showAlert('modalSugestaoAlert', 'Por favor, selecione um prato.', 'error');
        return;
    }

    const spinner = document.getElementById('sugestaoSpinner');
    if (spinner) spinner.style.display = '';

    try {
        const r = await api('POST', '/api/cardapio/sugestao', {
            itemCardapioId: itemCardapioId,
            periodo: periodo
        }, true);

        if (spinner) spinner.style.display = 'none';

        if (r.ok) {
            closeModal('modalNovaSugestao');
            loadSugestoesChefe();
            if (document.getElementById('page-dashboard').classList.contains('active')) {
                loadDashboard();
            }
            showAlert('modalSugestaoAlert', 'Sugestão do Chefe definida com sucesso!', 'success');
        } else {
            showAlert('modalSugestaoAlert', r.data?.erro || 'Não foi possível definir a sugestão.', 'error');
        }
    } catch (err) {
        if (spinner) spinner.style.display = 'none';
        showAlert('modalSugestaoAlert', 'Erro de comunicação com o servidor.', 'error');
    }
}

