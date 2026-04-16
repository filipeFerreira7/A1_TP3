# Sistema de Gestão para Restaurante - A1 Order System

![.NET](https://img.shields.io/badge/.NET-8.0-512BD4?style=for-the-badge&logo=dotnet&logoColor=white)
![C#](https://img.shields.io/badge/C%23-239120?style=for-the-badge&logo=c-sharp&logoColor=white)
![Entity Framework](https://img.shields.io/badge/Entity%20Framework-8.0-512BD4?style=for-the-badge&logo=nuget&logoColor=white)
![SQL Server](https://img.shields.io/badge/SQL%20Server-2022-CC2927?style=for-the-badge&logo=microsoft-sql-server&logoColor=white)

## Sobre o Projeto

Sistema de gestão completo para restaurantes, desenvolvido como projeto acadêmico da disciplina de **Tópicos III**. O sistema oferece funcionalidades para gerenciamento de pedidos, reservas de mesas, cardápio digital, sugestões do chef e relatórios de faturamento.

---

## Dados Acadêmicos

| Campo              | Informação                           |
|--------------------|--------------------------------------|
| **Disciplina**     | Tópicos III                          |
| **Professor**       | Jose Itamar Mendes de Souza Junior   |
| **Aluno**          | Filipe Ferreira                       |
| **Tecnologia**     | C# .NET 8.0                          |

---

## Funcionalidades

### Dashboard
- Visão geral com estatísticas em tempo real
- Contagem de pedidos, reservas, itens do cardápio e endereços
- Exibição das sugestões do chef do dia
- Lista dos últimos pedidos realizados

### Gestão de Pedidos
- Criação de pedidos para almoço e jantar
- Suporte a três tipos de atendimento:
  - **Presencial** - Consumo no restaurante
  - **Delivery Próprio** - Entrega feita pelo restaurante
  - **Delivery App** - Entrega via aplicativos (iFood, Rappi, 99Food)
- Cálculo automático de taxas de entrega
- Carrinho de compras interativo

### Reservas de Mesa
- Sistema de reservas para jantar (19h às 22h)
- Controle de disponibilidade por mesa
- Validação de conflitos de horário
- Restrição de reservas com antecedência mínima de 1 dia
- **Permissão por perfil:**
  - Administradores visualizam todas as reservas
  - Clientes visualizam apenas suas próprias reservas

### Cardápio Digital
- Gerenciamento de itens do cardápio
- Categorização por período (Almoço/Jantar)
- Sugestões do Chef com desconto de 20%

### Endereços
- Cadastro de múltiplos endereços de entrega
- Seleção de endereço ao fazer pedidos delivery

### Relatórios
- Filtro por período personalizado
- Faturamento detalhado por tipo de atendimento
- Ranking dos itens mais vendidos

---

## Tecnologias Utilizadas

### Backend
- **.NET 8.0** - Framework principal
- **ASP.NET Core Web API** - API RESTful
- **Entity Framework Core 8.0** - ORM para acesso ao banco de dados
- **SQL Server 2022** - Banco de dados relacional
- **JWT Authentication** - Sistema de autenticação

### Frontend
- **HTML5/CSS3** - Estrutura e estilização
- **JavaScript Vanilla** - Lógica client-side
- **Design Responsivo** - Interface adaptável

### Ferramentas
- **Swagger/OpenAPI** - Documentação da API
- **BCrypt** - Criptografia de senhas
- **LocalStorage** - Persistência de sessão no frontend

---

## Estrutura do Projeto

```
A1_order_system/
├── Controllers/           # Endpoints da API
│   ├── AuthController.cs
│   ├── CardapioController.cs
│   ├── ConfigController.cs
│   ├── EnderecoController.cs
│   ├── PedidoController.cs
│   ├── RelatorioController.cs
│   └── ReservaController.cs
├── Services/               # Lógica de negócio
│   ├── AuthService.cs
│   ├── CardapioService.cs
│   ├── ConfigService.cs
│   ├── EnderecoService.cs
│   ├── PedidoService.cs
│   ├── RelatorioService.cs
│   ├── ReservaService.cs
│   └── SugestaoService.cs
├── Entities/               # Modelos de domínio
├── Dtos/                   # Objetos de transferência de dados
├── Data/                   # Contexto do banco de dados
├── Migrations/             # Migrações do Entity Framework
├── wwwroot/                 # Arquivos estáticos
│   ├── css/
│   ├── js/
│   └── index.html
└── Program.cs              # Configuração da aplicação
```

---

## Configuração e Execução

### Pré-requisitos
- .NET 8.0 SDK
- SQL Server (local ou container Docker)
- Visual Studio 2022 ou VS Code

### Configuração do Banco de Dados

Edite o arquivo `appsettings.json` com a string de conexão do seu SQL Server:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=RestauranteDb;User Id=sa;Password=SuaSenha;TrustServerCertificate=True"
  }
}
```

### Execução

```bash
# Restaurar dependências
dotnet restore

# Aplicar migrations e iniciar
dotnet run

# Ou via Visual Studio
# Pressione F5 para depuração
```

A aplicação estará disponível em: `http://localhost:5000`

---

## Credenciais Padrão

| Perfil     | Email                    | Senha      |
|------------|--------------------------|------------|
| Administrador | admin@restaurante.com  | admin123   |

---

## API Endpoints

### Autenticação
| Método | Endpoint          | Descrição           |
|--------|-------------------|---------------------|
| POST   | /api/auth/login   | Login de usuário    |
| POST   | /api/auth/cadastro| Cadastro de usuário |

### Pedidos
| Método | Endpoint              | Descrição                    |
|--------|-----------------------|------------------------------|
| GET    | /api/pedidos          | Listar pedidos do usuário    |
| GET    | /api/pedidos/todos    | Listar todos (Admin)         |
| POST   | /api/pedidos          | Criar novo pedido            |

### Cardápio
| Método | Endpoint                    | Descrição                      |
|--------|-----------------------------|--------------------------------|
| GET    | /api/cardapio               | Listar itens do cardápio       |
| GET    | /api/cardapio/sugestoes-hoje| Sugestões do chef de hoje      |
| POST   | /api/cardapio               | Criar item (Admin)            |
| PUT    | /api/cardapio/{id}          | Atualizar item (Admin)        |
| DELETE | /api/cardapio/{id}          | Remover item (Admin)           |

### Reservas
| Método | Endpoint          | Descrição                    |
|--------|-------------------|------------------------------|
| GET    | /api/reservas     | Listar reservas              |
| POST   | /api/reservas     | Criar nova reserva           |

### Endereços
| Método | Endpoint             | Descrição               |
|--------|----------------------|-------------------------|
| GET    | /api/enderecos       | Listar endereços        |
| POST   | /api/enderecos       | Criar endereço          |
| PUT    | /api/enderecos/{id}  | Atualizar endereço      |
| DELETE | /api/enderecos/{id}  | Remover endereço        |

### Relatórios
| Método | Endpoint                    | Descrição              |
|--------|-----------------------------|------------------------|
| GET    | /api/relatorios/faturamento | Faturamento por período|

---

## Modelo de Dados

### Entidades Principais

- **Usuario** - Usuários do sistema (clientes e administradores)
- **Mesa** - Mesas disponíveis para reserva
- **Reserva** - Reservas de mesa com horário e cliente
- **ItemCardapio** - Itens do cardápio com preço e período
- **Pedido** - Pedidos realizados com itens e valores
- **PedidoItem** - Itens individuais de cada pedido
- **SugestaoChefe** - Sugestão do chef por período
- **Endereco** - Endereços de entrega dos clientes
- **Periodo** - Enum para Almoco/Jantar

---

## Autenticação e Autorização

O sistema utiliza **JWT (JSON Web Tokens)** para autenticação:

1. Usuário faz login e recebe um token
2. Token é armazenado no LocalStorage do navegador
3. Token é enviado no header `Authorization: Bearer <token>` em todas as requisições
4. O middleware valida o token e extrai as informações do usuário

### Perfis de Acesso

| Recurso              | Cliente | Administrador |
|----------------------|---------|---------------|
| Fazer pedidos         | ✅      | ✅            |
| Ver meus pedidos      | ✅      | ✅            |
| Ver todos os pedidos  | ❌      | ✅            |
| Fazer reservas        | ✅      | ✅            |
| Ver todas as reservas | ❌      | ✅            |
| Gerenciar cardápio    | ❌      | ✅            |
| Ver relatórios        | ❌      | ✅            |
| Gerenciar configurações| ❌     | ✅            |

---

## Regras de Negócio

### Reservas
- Horário: Apenas entre 19h e 22h
- Antecedência: Mínimo 1 dia de antecedência
- Conflictos: Uma mesa não pode ter mais de uma reserva no mesmo dia

### Pedidos
- Almoço: 11h às 14h59
- Jantar: 18h às 21h59
- Taxa de entrega para delivery próprio é configurável
- Taxa fixa de 20% para delivery via aplicativo

### Sugestões do Chef
- Máximo 1 sugestão por período (Almoço/Jantar)
- Desconto automático de 20% sobre o preço base

---

## Autor

**Filipe Ferreira**  
Discente do curso de Sistemas de Informação  
Universidade Federal - Tópicos III

**Professor:** Jose Itamar Mendes de Souza Junior

---

## Licença

Este projeto é de uso acadêmico. Todos os direitos reservados.
