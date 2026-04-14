// ====================== PEDIDOS.JS ======================

async function loadPedidos() {
    const tbody = document.getElementById('pedidosTable');
    tbody.innerHTML = '<tr><td colspan="6" style="text-align:center;padding:30px;"><span class="spinner"></span></td></tr>';

    const r = await api('GET', '/api/pedidos', null, true);
    const peds = r.ok ? (r.data || []) : [];

    tbody.innerHTML = peds.length ? peds.map(p => `
        <tr>
            <td class="td-name">#${p.id}</td>
            <td>${new Date(p.data).toLocaleDateString('pt-BR')}</td>
            <td><span class="menu-badge ${p.periodo === 0 ? 'menu-badge-almoco' : 'menu-badge-jantar'}">
                ${p.periodo === 0 ? 'Almoço' : 'Jantar'}
            </span></td>
            <td>${p.tipoAtendimento || 'Presencial'}</td>
            <td>${p.itens?.length || 0} itens</td>
            <td style="color:var(--gold);font-weight:600;">R$ ${Number(p.valorTotal).toFixed(2)}</td>
        </tr>
    `).join('') : `
        <tr>
            <td colspan="6">
                <div class="empty-state"><div class="empty-text">Nenhum pedido encontrado</div></div>
            </td>
        </tr>`;
}

async function initNovoPedido() {
    orderCart = {};

    const r = await api('GET', '/api/enderecos', null, true);
    const ends = r.ok ? (r.data || []) : [];
    const sel = document.getElementById('pedEnderecoId');

    sel.innerHTML = ends.length
        ? ends.map(e => `<option value="${e.id}">${e.logradouro} – ${e.cidade}/${e.estado}</option>`).join('')
        : '<option value="">Nenhum endereço cadastrado</option>';

    await loadCardapioForOrder();
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
                <button class="qty-btn" onclick="changeQty(${item.id}, -1, ${item.precoBase})">−</button>
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
                        ★ Sugestão
                    </span>` : ''}
            </div>
        </div>
    `).join('') : '<div class="empty-state"><div class="empty-text">Sem itens para este período</div></div>';
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

function updateCartUI() {
    const entries = Object.entries(orderCart);
    const cartDiv = document.getElementById('cartItems');

    cartDiv.innerHTML = entries.length ? entries.map(([id, { qty, preco }]) => `
        <div class="cart-item">
            <div class="cart-item-info">
                <div class="cart-item-name">${document.querySelector(`#oi-${id} .menu-item-name`)?.textContent || 'Item'}</div>
                <div class="cart-item-qty">× ${qty}</div>
            </div>
            <div class="cart-item-price">R$ ${(qty * preco).toFixed(2)}</div>
        </div>
    `).join('') : `
        <div class="empty-state" style="padding:30px 10px;">
            <div class="empty-text" style="font-size:13px;">Nenhum item no carrinho</div>
        </div>`;

    const subtotal = entries.reduce((s, [_, { qty, preco }]) => s + qty * preco, 0);
    const taxa = parseFloat(document.getElementById('pedTaxaFixa')?.value || 0) || 0;

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
    const body = {
        periodo: parseInt(document.getElementById('pedPeriodo').value),
        tipoAtendimento: tipo,
        itens: itens,
        taxaFixa: tipo === 'DeliveryProprio' ? (parseFloat(document.getElementById('pedTaxaFixa').value) || null) : null,
        nomeApp: tipo === 'DeliveryApp' ? document.getElementById('pedNomeApp').value : null,
        enderecoId: tipo !== 'Presencial' ? (parseInt(document.getElementById('pedEnderecoId').value) || null) : null,
    };

    document.getElementById('orderSpinner').style.display = '';
    clearAlert('orderAlert');

    const r = await api('POST', '/api/pedidos', body, true);
    document.getElementById('orderSpinner').style.display = 'none';

    if (r.ok) {
        showAlert('orderAlert', `Pedido #${r.data?.id} criado com sucesso! Total: R$ ${Number(r.data?.valorTotal).toFixed(2)}`, 'success');

        // Resetar carrinho
        orderCart = {};
        updateCartUI();
        document.querySelectorAll('.qty-val').forEach(el => el.textContent = '0');
        document.querySelectorAll('.menu-item-card').forEach(el => el.classList.remove('selected'));
    } else {
        showAlert('orderAlert', r.data?.erro || 'Erro ao criar pedido.', 'error');
    }
}