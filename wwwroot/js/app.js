// ====================== APP.JS ======================
// Inicialização, navegação e funções globais da aplicação

function enterApp() {
    document.getElementById('authPage').style.display = 'none';
    document.getElementById('appPage').style.display = 'flex';

    document.getElementById('userName').textContent = currentUser.nome || 'Usuário';
    document.getElementById('userEmail').textContent = currentUser.email || '';
    document.getElementById('userAvatar').textContent = (currentUser.nome || 'U')[0].toUpperCase();

    navigate('dashboard');
}

function logout() {
    token = null;
    currentUser = {};
    document.getElementById('appPage').style.display = 'none';
    document.getElementById('authPage').style.display = 'flex';
}

function navigate(page) {
    // Remove active de todas as páginas
    document.querySelectorAll('.page').forEach(p => p.classList.remove('active'));

    // Remove active de todos os itens do menu
    document.querySelectorAll('.nav-item').forEach(n => n.classList.remove('active'));

    // Ativa a página
    const targetPage = document.getElementById(`page-${page}`);
    if (targetPage) targetPage.classList.add('active');

    // Ativa o item no sidebar
    document.querySelectorAll('.nav-item').forEach(n => {
        if (n.getAttribute('onclick')?.includes(`'${page}'`)) {
            n.classList.add('active');
        }
    });

    // Carrega dados da página
    switch (page) {
        case 'dashboard':
            loadDashboard();
            break;
        case 'cardapio':
            loadCardapio();
            break;
        case 'pedidos':
            loadPedidos();
            break;
        case 'novo-pedido':
            initNovoPedido();
            break;
        case 'reservas':
            loadReservas();
            break;
        case 'enderecos':
            loadEnderecos();
            break;
        case 'sugestao-chefe':
            loadSugestoesChefe();
            break;
        case 'relatorios':
            // datas já configuradas no DOMContentLoaded
            break;
        default:
            console.warn(`Página não implementada: ${page}`);
    }
}

// Inicialização
document.addEventListener('DOMContentLoaded', () => {
    const today = new Date();
    const firstDay = new Date(today.getFullYear(), today.getMonth(), 1);

    const relInicio = document.getElementById('relInicio');
    const relFim = document.getElementById('relFim');

    if (relInicio) relInicio.value = firstDay.toISOString().split('T')[0];
    if (relFim) relFim.value = today.toISOString().split('T')[0];
});