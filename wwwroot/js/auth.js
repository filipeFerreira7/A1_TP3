function switchAuthTab(tab) {
    document.querySelectorAll('.auth-tab').forEach((b, i) => {
        b.classList.toggle('active',
            (i === 0 && tab === 'login') || (i === 1 && tab === 'cadastro')
        );
    });

    document.getElementById('loginForm').style.display = tab === 'login' ? '' : 'none';
    document.getElementById('cadastroForm').style.display = tab === 'cadastro' ? '' : 'none';
    clearAlert('authAlert');
}
async function doLogin() {
    const email = document.getElementById('loginEmail').value.trim();
    const senha = document.getElementById('loginSenha').value.trim();

    if (!email || !senha) {
        showAlert('authAlert', 'Preencha e-mail e senha.');
        return;
    }

    document.getElementById('loginSpinner').style.display = '';
    const r = await api('POST', '/api/auth/login', { email, senha });
    document.getElementById('loginSpinner').style.display = 'none';

    if (r.ok && r.data?.token) {
        token = r.data.token;
        currentUser = {
            nome: r.data.nome,
            email: r.data.email,
            perfil: r.data.perfil || 'Cliente'
        };

        saveSession(token, currentUser);  
        enterApp();
    } else {
        showAlert('authAlert', r.data?.erro || 'Credenciais inválidas.');
    }
}

async function doCadastro() {
    const nome = document.getElementById('cadNome').value.trim();
    const email = document.getElementById('cadEmail').value.trim();
    const senha = document.getElementById('cadSenha').value.trim();

    if (!nome || !email || !senha) {
        showAlert('authAlert', 'Preencha todos os campos.');
        return;
    }

    document.getElementById('cadSpinner').style.display = '';
    const r = await api('POST', '/api/auth/cadastro', { nome, email, senha });
    document.getElementById('cadSpinner').style.display = 'none';

    if (r.ok && r.data?.token) {
        token = r.data.token;
        currentUser = {
            nome: r.data.nome,
            email: r.data.email,
            perfil: r.data.perfil || 'Cliente'
        };

        saveSession(token, currentUser);
        enterApp();
    } else {
        showAlert('authAlert', r.data?.erro || 'Erro ao cadastrar.');
    }
}
