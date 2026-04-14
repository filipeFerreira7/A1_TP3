// ====================== UTILS.JS ======================
// Funções globais reutilizáveis

let token = null;
let currentUser = {};
let cardapioData = [];
let orderCart = {};
let currentFilter = null;

async function api(method, path, body = null, auth = false) {
    const headers = { 'Content-Type': 'application/json' };
    if (auth && token) headers['Authorization'] = `Bearer ${token}`;

    const opts = { method, headers };
    if (body) opts.body = JSON.stringify(body);

    try {
        const r = await fetch(path, opts);
        const ct = r.headers.get('content-type') || '';
        let data = null;
        if (ct.includes('json')) data = await r.json();

        return { ok: r.ok, status: r.status, data };
    } catch (e) {
        console.error('API Error:', e);
        return { ok: false, status: 0, data: { erro: e.message } };
    }
}

function showAlert(id, msg, type = 'error') {
    const html = `<div class="alert alert-${type}">${msg}</div>`;
    document.getElementById(id).innerHTML = html;
}

function clearAlert(id) {
    document.getElementById(id).innerHTML = '';
}

function openModal(id) {
    document.getElementById(id).classList.add('open');
}

function closeModal(id) {
    const modal = document.getElementById(id);
    modal.classList.remove('open');
    // Limpa alerts dentro do modal
    modal.querySelectorAll('.alert').forEach(a => a.remove());
}

// Fechar modal ao clicar no overlay
document.addEventListener('click', e => {
    if (e.target.classList.contains('modal-overlay')) {
        e.target.classList.remove('open');
    }
});