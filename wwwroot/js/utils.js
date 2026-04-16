

let token = null;
let currentUser = {};
let cardapioData = [];
let orderCart = {};
let currentFilter = null;

function setAuthState(nextToken, nextUser) {
    token = nextToken || null;
    currentUser = nextUser || {};
}

function parseJwtPayload(jwt) {
    if (!jwt) return null;

    try {
        const parts = jwt.split('.');
        if (parts.length !== 3) return null;

        const base64 = parts[1].replace(/-/g, '+').replace(/_/g, '/');
        const padded = base64.padEnd(base64.length + (4 - base64.length % 4) % 4, '=');
        return JSON.parse(atob(padded));
    } catch (e) {
        console.error('Erro ao decodificar token JWT:', e);
        return null;
    }
}

function buildUserFromToken(jwt) {
    const payload = parseJwtPayload(jwt);
    if (!payload) return null;

    const nome =
        payload['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name'] ||
        payload.name ||
        '';

    const email =
        payload['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress'] ||
        payload.email ||
        '';

    const perfil =
        payload['http://schemas.microsoft.com/ws/2008/06/identity/claims/role'] ||
        payload.role ||
        'Cliente';

    if (!nome && !email) return null;

    return { nome, email, perfil };
}

function parseStoredUser(userStr, jwt) {
    if (!userStr) return buildUserFromToken(jwt);

    try {
        const parsed = JSON.parse(userStr);
        if (parsed && typeof parsed === 'object') {
            return {
                nome: parsed.nome || '',
                email: parsed.email || '',
                perfil: parsed.perfil || 'Cliente'
            };
        }
    } catch (e) {
        console.warn('currentUser inválido no localStorage. Recriando a partir do token.', e);
    }

    return buildUserFromToken(jwt);
}

function isTokenExpired(jwt) {
    const payload = parseJwtPayload(jwt);
    if (!payload?.exp) return false;

    return payload.exp * 1000 <= Date.now();
}

function saveSession(token, user) {
    if (!token || !user) return;

    try {
        setAuthState(token, user);
        localStorage.setItem('token', token);
        localStorage.setItem('currentUser', JSON.stringify(user));
        console.log('Sessão salva com sucesso');
    } catch (e) {
        console.error('Erro ao salvar sessão:', e);
    }
}

function loadSession() {
    try {
        const savedToken = localStorage.getItem('token');
        const userStr = localStorage.getItem('currentUser');

        if (savedToken) {
            if (isTokenExpired(savedToken)) {
                clearSession();
                return false;
            }

            const user = parseStoredUser(userStr, savedToken);
            if (!user) {
                clearSession();
                return false;
            }

            setAuthState(savedToken, user);
            localStorage.setItem('currentUser', JSON.stringify(user));
            console.log('Sessão carregada com sucesso');
            return true;
        }
    } catch (e) {
        console.error('Erro ao carregar sessão:', e);
        clearSession();
    }
    return false;
}

function clearSession() {
    localStorage.removeItem('token');
    localStorage.removeItem('currentUser');
    setAuthState(null, {});
}

function isAdmin() {
    return currentUser?.perfil === 'Admin';
}

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
    modal.querySelectorAll('.alert').forEach(a => a.remove());
}

document.addEventListener('click', e => {
    if (e.target.classList.contains('modal-overlay')) {
        e.target.classList.remove('open');
    }
});
