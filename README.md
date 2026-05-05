# GestaPME — Sistema de Gestão Empresarial para PMEs com Assistente IA

Sistema web de gestão empresarial e de funcionários voltado a pequenas e médias empresas, com assistente conversacional integrado baseado em Inteligência Artificial.

**Trabalho de Conclusão de Curso** — Ciência da Computação — UNIP (Universidade Paulista)

---

## Tecnologias

- **Backend:** ASP.NET Core 8 / C#
- **Apresentação:** ASP.NET Core MVC + Razor Views (server-rendered) + Bootstrap 5 + jQuery Validation Unobtrusive
- **Banco de Dados:** SQL Server / Azure SQL Database
- **ORM:** Entity Framework Core 8
- **Autenticação:** Cookie Authentication + BCrypt (hashing de senhas)
- **IA:** GitHub Models (compatível com OpenAI API) — substituível por Azure OpenAI Service ou OpenAI direto via configuração
- **SDK de IA:** OpenAI .NET v2 (provider-agnostic)
- **Nuvem:** Microsoft Azure (App Service + SQL Database)
- **Documentação API:** Swagger / OpenAPI

---

## Arquitetura

A aplicação adota uma arquitetura híbrida pragmática:

- **Camada de apresentação:** ASP.NET Core MVC com Razor Views renderizando HTML no servidor para todas as operações de gestão (CRUDs, autenticação, painel inicial).
- **Camada de API:** um único endpoint REST (`/api/Assistente`) consumido pela tela de chat via `fetch` assíncrono, isolando a integração com o modelo de linguagem.
- **Multi-empresa:** isolamento de dados por empresa (multi-tenant) via `IContextoEmpresa`, baseado em claims do cookie autenticado. Um usuário jamais acessa dados de outra empresa, mesmo com manipulação de URL.

---

## Funcionalidades

- **Cadastro público de empresa** + criação automática do usuário Administrador inicial em transação única
- **Autenticação** por e-mail/senha com BCrypt + Cookie Authentication (8h sliding)
- **Controle de acesso por perfil:** Administrador (gestão completa) e Gestor (leitura + solicitação de férias)
- **Painel inicial** com cards de resumo (funcionários totais, ativos, inativos, departamentos, férias em curso, férias pendentes de aprovação)
- **Gestão de empresa** (perfil, edição apenas pelo Admin)
- **Gestão de departamentos** (CRUD com validação de integridade referencial)
- **Gestão de cargos** (CRUD)
- **Gestão de funcionários** (cadastro, filtros por departamento/cargo/status, busca por nome ou CPF, paginação, soft delete)
- **Gestão de férias** (solicitação, aprovação/rejeição pelo Admin, validação de sobreposição de períodos)
- **Gestão de usuários da empresa** (CRUD, soft delete, redefinição de senha — apenas Admin)
- **Assistente conversacional com IA** — consulte dados da empresa em linguagem natural
- **Páginas de erro customizadas** (404, 500, 403)
- **Proteção CSRF** global em formulários

---

## Pré-requisitos

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [SQL Server](https://www.microsoft.com/sql-server) (local, LocalDB ou Azure SQL Database)
- Conta no [GitHub](https://github.com) com [Personal Access Token (fine-grained)](https://github.com/settings/personal-access-tokens) e escopo **Models: read** — para o assistente IA via [GitHub Models](https://github.com/marketplace/models)
- [Visual Studio 2022](https://visualstudio.microsoft.com/) ou VS Code com extensão C# Dev Kit

---

## Configuração local

### 1. Clone o repositório

```bash
git clone https://github.com/SEU-USUARIO/GestaPME.git
cd GestaPME
```

### 2. Restaure dependências e bibliotecas front

```bash
dotnet restore
dotnet tool install -g Microsoft.Web.LibraryManager.Cli
libman restore
```

### 3. Configure os segredos (User Secrets)

**Nunca commit segredos no repositório.** Use o sistema de User Secrets do .NET:

```bash
dotnet user-secrets init

# Connection string do banco (ajuste para seu ambiente)
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Server=localhost;Database=GestaPME;Trusted_Connection=True;TrustServerCertificate=True;"

# Provedor de IA — endpoint, chave e modelo
dotnet user-secrets set "AssistenteIA:Endpoint" "https://models.github.ai/inference"
dotnet user-secrets set "AssistenteIA:ApiKey"   "github_pat_SEU_TOKEN_AQUI"
dotnet user-secrets set "AssistenteIA:Modelo"   "openai/gpt-4o-mini"
```

> **Trocar de provedor de IA é só configuração.** O código usa o SDK `OpenAI` v2 (compatível com qualquer endpoint OpenAI-compatible). Para usar Azure OpenAI Service ou OpenAI direto, basta ajustar `Endpoint`, `ApiKey` e `Modelo` — sem mudança de código.

### 4. Aplique as migrações

```bash
dotnet ef database update
```

(Se for a primeira vez no banco, isso cria todas as tabelas. Se faltar a CLI do EF: `dotnet tool install -g dotnet-ef`.)

### 5. Execute

```bash
dotnet run
```

A aplicação sobe por padrão em `https://localhost:7055`. Acesse no navegador.

- **`/Conta/Cadastro`** — cadastre sua empresa e crie o primeiro usuário Administrador.
- **`/swagger`** (apenas em ambiente Development) — documentação interativa do endpoint de IA.

---

## Estrutura do Projeto

```
GestaPME/
├── Controllers/         # Controllers MVC (todas as telas)
│   └── Api/             # Único endpoint REST (assistente IA)
├── Views/               # Razor Views
│   └── Shared/          # Layouts e partials reutilizáveis
├── ViewModels/          # DTOs específicos das views (forms, listagens)
├── Services/            # Lógica de domínio (auth, contexto multi-empresa, IA)
├── Data/                # AppDbContext (Entity Framework)
├── Models/              # Entidades do domínio
├── Validators/          # ValidationAttributes customizados (CPF, CNPJ)
├── Util/                # Helpers genéricos (paginação)
├── Migrations/          # Migrações EF Core
├── Properties/          # launchSettings.json
├── wwwroot/             # Estáticos servidos pelo navegador
│   ├── css/             # Design system (paleta teal)
│   ├── js/              # Cliente do assistente IA
│   └── lib/             # Bootstrap, jQuery (via libman)
└── docs/
    └── testes/          # Roteiros funcional, integração, usabilidade
```

---

## Perfis de usuário

| Perfil | Permissões |
|---|---|
| **Administrador** | Gestão completa: editar empresa, CRUD de departamentos/cargos/funcionários/usuários, aprovar/rejeitar férias, redefinir senhas |
| **Gestor** | Leitura de todos os módulos, solicitação de férias para si ou para outros, alteração da própria senha |

O primeiro usuário criado no cadastro público da empresa é sempre Administrador.

---

## Exemplo de uso do Assistente IA

**Request:**

```http
POST /api/Assistente
Content-Type: application/json
Cookie: GestaPME.Auth=<cookie-de-sessão>

{
  "pergunta": "Quantos funcionários estão ativos na empresa?"
}
```

> O `EmpresaId` é resolvido automaticamente a partir das claims do cookie autenticado — nunca enviado no body.

**Response:**

```json
{
  "pergunta": "Quantos funcionários estão ativos na empresa?",
  "resposta": "Atualmente a empresa possui 12 funcionários ativos, distribuídos em 3 departamentos."
}
```

---

## Implantação no Azure (produção)

Em ambiente de produção (Azure App Service), os segredos são configurados como **Application settings**, com `:` substituído por `__`:

| Setting | Valor |
|---|---|
| `ConnectionStrings__DefaultConnection` | string de conexão do Azure SQL |
| `AssistenteIA__Endpoint` | `https://models.github.ai/inference` |
| `AssistenteIA__ApiKey` | PAT do GitHub com escopo `models:read` |
| `AssistenteIA__Modelo` | `openai/gpt-4o-mini` |

---

## Documentação adicional

- [`docs/testes/`](docs/testes/) — roteiros de teste funcional, integração e usabilidade

---

## Licença

Projeto acadêmico — uso educacional.
