
async function loadEnderecos() {
    const container = document.getElementById('enderecosList');
    if (!container) return;

    container.innerHTML = '<span class="spinner"></span>';

    const r = await api('GET', '/api/enderecos', null, true);
    const list = r.ok ? (r.data || []) : [];

    if (list.length === 0) {
        container.innerHTML = `
            <div class="empty-state">
                <div class="empty-icon">?</div>
                <div class="empty-text">Nenhum endereço cadastrado</div>
            </div>`;
        return;
    }

    container.innerHTML = list.map(e => `
        <div class="card" style="display:flex; align-items:center; gap:16px;">
            <div style="width:40px;height:40px;border-radius:50%;background:rgba(201,169,110,0.12);display:flex;align-items:center;justify-content:center;font-size:16px;">👤</div>
            <div style="flex:1;">
                <div style="font-size:14px;font-weight:500;color:var(--text)">${e.logradouro}</div>
                <div style="font-size:12px;color:var(--text3)">${e.cidade} / ${e.estado}</div>
                ${e.clienteNome ? `
                    <div style="font-size:12px;color:var(--gold);margin-top:4px;">
                        ${e.clienteNome}${e.clienteEmail ? ` · ${e.clienteEmail}` : ''}
                    </div>` : ''}
            </div>
            <button class="btn btn-danger btn-sm" onclick="deleteEndereco(${e.id})">Remover</button>
        </div>
    `).join('');
}

async function deleteEndereco(id) {
    if (!confirm('Deseja realmente remover este endereço?')) return;

    const r = await api('DELETE', `/api/enderecos/${id}`, null, true);
    if (r.ok) {
        loadEnderecos();
    } else {
        alert(r.data?.erro || 'Erro ao remover endereço.');
    }
}

async function submitEndereco() {
    const logradouro = document.getElementById('eLog').value.trim();
    const cidade = document.getElementById('eCidade').value.trim();
    const estado = document.getElementById('eEstado').value.trim().toUpperCase();

    if (!logradouro || !cidade || !estado) {
        showAlert('novoEndAlert', 'Preencha todos os campos.', 'error');
        return;
    }

    const spinner = document.getElementById('endSpinner');
    if (spinner) spinner.style.display = '';

    try {
        const r = await api('POST', '/api/enderecos', {
            logradouro: logradouro,
            cidade: cidade,
            estado: estado
        }, true);

        if (spinner) spinner.style.display = 'none';

        if (r.ok) {
            closeModal('modalNovoEndereco');
            loadEnderecos();
            document.getElementById('eLog').value = '';
            document.getElementById('eCidade').value = '';
            document.getElementById('eEstado').value = '';

            showAlert('novoEndAlert', 'Endereço salvo com sucesso!', 'success');
        } else {
            showAlert('novoEndAlert', r.data?.erro || 'Erro ao salvar endereço.', 'error');
        }
    } catch (err) {
        if (spinner) spinner.style.display = 'none';
        showAlert('novoEndAlert', 'Erro de comunicação com o servidor.', 'error');
    }
}

window.submitEndereco = submitEndereco;
