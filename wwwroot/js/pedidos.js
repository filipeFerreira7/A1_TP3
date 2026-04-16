async function loadPedidos() {
    const tbody = document.getElementById('pedidosTable');
    const thead = document.getElementById('pedidosHeader');
    const storedUser = JSON.parse(localStorage.getItem('currentUser') || '{}');
    const isAdmin = storedUser?.perfil === 'Admin';
    const colCount = isAdmin ? 8 : 6;
    
    const btnNovoPedido = document.getElementById('btnNovoPedido');
    if (btnNovoPedido) btnNovoPedido.style.display = isAdmin ? 'none' : '';
    
    thead.innerHTML = `<tr>${isAdmin ? '<th>Cliente</th>' : ''}<th>#</th><th>Data</th><th>Período</th><th>Tipo</th>${isAdmin ? '<th>Endereço</th>' : ''}<th>Itens</th><th>Total</th></tr>`;
    tbody.innerHTML = `<tr><td colspan="${colCount}" style="text-align:center;padding:30px;"><span class="spinner"></span></td></tr>`;

    const endpoint = isAdmin ? '/api/pedidos/todos' : '/api/pedidos';
    const r = await api('GET', endpoint, null, true);
    const peds = r.ok ? (r.data || []) : [];

    tbody.innerHTML = peds.length ? peds.map(p => `
        <tr>
            ${isAdmin ? `<td>${p.nomeUsuario || '–'}</td>` : ''}
            <td class="td-name">#${p.id}</td>
            <td>${new Date(p.data).toLocaleDateString('pt-BR')}</td>
            <td><span class="menu-badge ${p.periodo === 0 ? 'menu-badge-almoco' : 'menu-badge-jantar'}">
                ${p.periodo === 0 ? 'Almoço' : 'Jantar'}
            </span></td>
            <td>${p.tipoAtendimento || 'Presencial'}</td>
            ${isAdmin ? `<td>${p.endereco ? `${p.endereco.logradouro}, ${p.endereco.cidade}/${p.endereco.estado}` : 'Retirada no local'}</td>` : ''}
            <td>${p.itens?.length || 0} itens</td>
            <td style="color:var(--gold);font-weight:600;">R$ ${Number(p.valorTotal).toFixed(2)}</td>
        </tr>
    `).join('') : `
        <tr>
            <td colspan="${colCount}">
                <div class="empty-state"><div class="empty-text">Nenhum pedido encontrado</div></div>
            </td>
        </tr>`;
}

﻿let taxaDeliveryProprio = 8.00;

async function initNovoPedido() {
    orderCart = {};

    const r = await api('GET', '/api/enderecos', null, true);
    const ends = r.ok ? (r.data || []) : [];
    const sel = document.getElementById('pedEnderecoId');

    sel.innerHTML = ends.length
        ? ends.map(e => `<option value="${e.id}">${e.logradouro} - ${e.cidade}/${e.estado}</option>`).join('')
        : '<option value="">Nenhum endereco cadastrado</option>';

    const storedUser = JSON.parse(localStorage.getItem('currentUser') || '{}');
    const isAdmin = storedUser?.perfil === 'Admin';

    if (isAdmin) {
        const configR = await api('GET', '/api/config/taxa-delivery', null, true);
        if (configR.ok && configR.data?.taxa) {
            taxaDeliveryProprio = parseFloat(configR.data.taxa);
        }
    }

    const taxaInput = document.getElementById('pedTaxaFixa');
    taxaInput.value = taxaDeliveryProprio.toFixed(2);
    taxaInput.readOnly = !isAdmin;

    await loadCardapioForOrder();
    updateOrderType();
    updateCartUI();
}

async function loadCardapioForOrder() {
    const periodo = document.getElementById('pedPeriodo').value;
    const grid = document.getElementById('orderMenuGrid');
    grid.innerHTML = '<span class="spinner"></span>';

    const r = await api('GET', `/api/cardapio?periodo=${periodo}`);
    const items = r.ok ? (r.data || []) : [];

    grid.innerHTML = items.length ? items.map(item => `
        <div class="menu-item-card" id="oi-${item.id}">
            <div class="qty-selector">
                <button class="qty-btn" onclick="changeQty(${item.id}, -1, ${item.precoBase})">-</button>
                <span class="qty-val" id="qty-${item.id}">0</span>
                <button class="qty-btn" onclick="changeQty(${item.id}, 1, ${item.precoBase})">+</button>
            </div>
            <div class="menu-item-name" style="padding-right:80px;">${item.nome}</div>
            <div class="menu-item-desc">${item.descricao || ''}</div>
            <div class="menu-item-footer">
                <span class="menu-price">
                    ${item.precoComDesconto
            ? `
                            <s style="color:var(--text3);font-size:12px;">R$ ${Number(item.precoBase).toFixed(2)}</s> 
                            <strong style="color:var(--gold);">R$ ${Number(item.precoComDesconto).toFixed(2)}</strong>
                          `
            : `R$ ${Number(item.precoBase).toFixed(2)}`}
                </span>
                ${item.isSugestaoChefe ? `
                    <span class="menu-badge menu-badge-chef" style="background:#c0392b;color:white;">
                        Sugestao
                    </span>` : ''}
            </div>
        </div>
    `).join('') : '<div class="empty-state"><div class="empty-text">Sem itens para este periodo</div></div>';
}

function changeQty(id, delta, preco) {
    if (!orderCart[id]) orderCart[id] = { qty: 0, preco };
    orderCart[id].qty = Math.max(0, orderCart[id].qty + delta);
    if (orderCart[id].qty === 0) delete orderCart[id];

    const qtyEl = document.getElementById(`qty-${id}`);
    if (qtyEl) qtyEl.textContent = orderCart[id]?.qty || 0;

    const card = document.getElementById(`oi-${id}`);
    if (card) card.classList.toggle('selected', !!(orderCart[id]?.qty));

    updateCartUI();
}

function updateOrderType() {
    const tipo = document.getElementById('pedTipo').value;
    document.getElementById('deliveryFields').style.display = tipo !== 'Presencial' ? '' : 'none';
    document.getElementById('deliveryProprioField').style.display = tipo === 'DeliveryProprio' ? '' : 'none';
    document.getElementById('deliveryAppField').style.display = tipo === 'DeliveryApp' ? '' : 'none';
    updateCartUI();
}

function calculateDeliveryAppTax(subtotal) {
    if (!subtotal) return 0;
    const hour = new Date().getHours();
    const percentage = hour >= 18 ? 0.06 : 0.04;
    return subtotal * percentage;
}

function updateCartUI() {
    const entries = Object.entries(orderCart);
    const cartDiv = document.getElementById('cartItems');

    cartDiv.innerHTML = entries.length ? entries.map(([id, { qty, preco }]) => `
        <div class="cart-item">
            <div class="cart-item-info">
                <div class="cart-item-name">${document.querySelector(`#oi-${id} .menu-item-name`)?.textContent || 'Item'}</div>
                <div class="cart-item-qty">x ${qty}</div>
            </div>
            <div class="cart-item-price">R$ ${(qty * preco).toFixed(2)}</div>
        </div>
    `).join('') : `
        <div class="empty-state" style="padding:30px 10px;">
            <div class="empty-text" style="font-size:13px;">Nenhum item no carrinho</div>
        </div>`;

    const subtotal = entries.reduce((s, [_, { qty, preco }]) => s + qty * preco, 0);
    const tipo = document.getElementById('pedTipo')?.value || 'Presencial';

    let taxa = 0;
    if (tipo === 'DeliveryProprio') {
        taxa = parseFloat(document.getElementById('pedTaxaFixa')?.value || 0) || 0;
    } else if (tipo === 'DeliveryApp') {
        taxa = calculateDeliveryAppTax(subtotal);
    }

    document.getElementById('cartSubtotal').textContent = `R$ ${subtotal.toFixed(2)}`;
    document.getElementById('cartTaxa').textContent = `R$ ${taxa.toFixed(2)}`;
    document.getElementById('cartTotal').textContent = `R$ ${(subtotal + taxa).toFixed(2)}`;
}

async function submitOrder() {
    const itens = Object.entries(orderCart).map(([id, { qty }]) => ({
        itemCardapioId: parseInt(id),
        quantidade: qty
    }));

    if (!itens.length) {
        showAlert('orderAlert', 'Selecione ao menos um item.', 'error');
        return;
    }

    const tipo = document.getElementById('pedTipo').value;
    const periodo = parseInt(document.getElementById('pedPeriodo').value);
    
    if (tipo !== 'Presencial') {
        const enderecoId = parseInt(document.getElementById('pedEnderecoId').value);
        if (!enderecoId) {
            showAlert('orderAlert', 'Cadastre um endereco de entrega antes de fazer um pedido de delivery.', 'error');
            return;
        }
    }

    const body = {
        periodo: periodo,
        tipoAtendimento: tipo,
        itens: itens,
        taxaFixa: tipo === 'DeliveryProprio' ? (parseFloat(document.getElementById('pedTaxaFixa').value) || taxaDeliveryProprio) : null,
        nomeApp: tipo === 'DeliveryApp' ? document.getElementById('pedNomeApp').value : null,
        enderecoId: tipo !== 'Presencial' ? parseInt(document.getElementById('pedEnderecoId').value) : null,
    };

    document.getElementById('orderSpinner').style.display = '';
    clearAlert('orderAlert');

    const r = await api('POST', '/api/pedidos', body, true);
    document.getElementById('orderSpinner').style.display = 'none';

    if (r.ok) {
        showAlert('orderAlert', `Pedido #${r.data?.id} criado com sucesso! Total: R$ ${Number(r.data?.valorTotal).toFixed(2)}`, 'success');

        orderCart = {};
        updateCartUI();
        document.querySelectorAll('.qty-val').forEach(el => el.textContent = '0');
        document.querySelectorAll('.menu-item-card').forEach(el => el.classList.remove('selected'));
    } else {
        let msgErro = r.data?.erro || 'Erro ao criar pedido.';
        showAlert('orderAlert', msgErro, 'error');
    }
}
