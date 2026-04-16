
function enterApp() {
    document.getElementById('authPage').style.display = 'none';
    document.getElementById('appPage').style.display = 'flex';

    document.getElementById('userName').textContent = currentUser.nome || 'Usuário';
    document.getElementById('userEmail').textContent = currentUser.email || '';
    document.getElementById('userAvatar').textContent = (currentUser.nome || 'U')[0].toUpperCase();

    applyPermissions();

    navigate(isAdmin() ? 'dashboard' : 'cardapio');
}

function logout() {
    clearSession();
    document.getElementById('appPage').style.display = 'none';
    document.getElementById('authPage').style.display = 'flex';
}

function navigate(page) {
    const adminPages = new Set(['dashboard', 'sugestao-chefe', 'relatorios']);
    if (adminPages.has(page) && !isAdmin()) {
        page = 'cardapio';
    }

    document.querySelectorAll('.page').forEach(p => p.classList.remove('active'));
    document.querySelectorAll('.nav-item').forEach(n => n.classList.remove('active'));

    const targetPage = document.getElementById(`page-${page}`);
    if (targetPage) targetPage.classList.add('active');

    document.querySelectorAll('.nav-item').forEach(n => {
        if (n.getAttribute('onclick')?.includes(`'${page}'`)) {
            n.classList.add('active');
        }
    });

    switch (page) {
        case 'dashboard': loadDashboard(); break;
        case 'cardapio': loadCardapio(); break;
        case 'pedidos': loadPedidos(); break;
        case 'novo-pedido': initNovoPedido(); break;
        case 'reservas': loadReservas(); break;
        case 'enderecos': loadEnderecos(); break;
        case 'sugestao-chefe': loadSugestoesChefe(); break;
        case 'relatorios': break;
        default:
            console.warn(`Página não implementada: ${page}`);
    }
}

function applyPermissions() {
    const adminOnlyIds = ['navDashboard', 'navSugestaoChefe', 'navRelatorios', 'btnNovoItem', 'btnNovaSugestao'];

    for (const id of adminOnlyIds) {
        const el = document.getElementById(id);
        if (el) {
            el.style.display = isAdmin() ? '' : 'none';
        }
    }
}

document.addEventListener('DOMContentLoaded', () => {
    if (loadSession()) {
        enterApp();
    }

    const today = new Date();
    const firstDay = new Date(today.getFullYear(), today.getMonth(), 1);

    const relInicio = document.getElementById('relInicio');
    const relFim = document.getElementById('relFim');

    if (relInicio) relInicio.value = firstDay.toISOString().split('T')[0];
    if (relFim) relFim.value = today.toISOString().split('T')[0];
});
