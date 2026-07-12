# UserService

Microsserviço de **usuários e autenticação** da plataforma EsperançaSolidária. Expõe uma API REST protegida por JWT e consome eventos de doação do RabbitMQ para manter um total doado por usuário.

## Stack

- **.NET 10** · ASP.NET Core Web API
- **MongoDB** (driver v3) — persistência
- **MassTransit + RabbitMQ** — mensageria
- **JWT Bearer** · **BCrypt** — autenticação e hash de senha
- **AutoMapper** · **xUnit + Moq** — mapeamento e testes

## Arquitetura

Camadas em Clean/Onion — as dependências apontam **para dentro**, o `Domain` no núcleo não depende de ninguém.

| Camada | Responsabilidade | Exemplos |
|---|---|---|
| **Domain** | Entidades e contratos (interfaces). Zero dependências. | `User`, `UserDonationStat`, `Roles`, `IUserRepository`, `IJwtTokenGenerator` |
| **Application** | Casos de uso e orquestração. DTOs, validação, regras. | `AuthService`, `UserApplicationService`, `DonationStatsService`, `DonationReceivedEventConsumer` |
| **Infra** | Implementações concretas dos contratos do Domain. | `UserRepository`, `UserStatisticsRepository`, `BCryptPasswordHasher`, `JwtTokenGenerator`, bootstrap do Mongo |
| **WebApi** | Superfície HTTP + raiz de composição (DI, middleware, health). | `AuthController`, `UserController`, `Program`, extensions |

O contrato de evento fica num projeto **compartilhado** (`../Shared/EsperancaSolidaria.Contracts`), referenciado por todos os serviços que publicam/consomem — é o que garante que o roteamento do MassTransit case entre eles.

```
src/
 ├─ Shared/EsperancaSolidaria.Contracts   ← eventos compartilhados (DonationReceivedEvent)
 └─ UserService/
     ├─ UserService.Domain
     ├─ UserService.Application
     ├─ UserService.Infra
     ├─ UserService.WebApi                ← ponto de entrada
     └─ UserService.Test
```

## Como funciona

### Autenticação e perfis

Dois perfis (`Roles`): **GestorONG** (gestão) e **Doador** (comum).

- `POST /Auth/Register` é **público** e cria sempre um **Doador**.
- Os endpoints de gestão (`/User/*`) exigem token de **GestorONG**.
- Senha é guardada com **BCrypt**; **e-mail e CPF são únicos** (índice no banco) e o CPF é validado no formato.
- Um GestorONG padrão é criado no boot (seção `BootstrapGestor` do `appsettings`) caso não exista.

### Consumo de doações (idempotente)

Ao receber um `DonationReceivedEvent` (`DonorUserId` + `Amount`), o serviço **soma o valor ao total do usuário** na coleção `user_statistics` (um documento por usuário, atualizado com `$inc` atômico).

Como o RabbitMQ entrega *at-least-once*, a mesma mensagem pode chegar duas vezes. Para não somar em dobro, o consumo é **idempotente pelo `MessageId`** do envelope: numa **transação**, o `MessageId` é gravado numa coleção separada (`processed_messages`) e o total é incrementado — se o `MessageId` já existir, a transação é abortada e nada é somado. Assim a `user_statistics` fica limpa (só usuário + total) e o dedupe vive à parte.

> A transação exige um replica set do Mongo (o Atlas já é; o Mongo local do compose sobe como replica set de nó único por isso).

## Endpoints

| Método | Rota | Acesso |
|---|---|---|
| `POST` | `/Auth/Login` | Público |
| `POST` | `/Auth/Register` | Público (cria Doador) |
| `POST` | `/User/Create` | GestorONG (role no corpo) |
| `GET` | `/User/GetAll` | GestorONG |
| `GET` | `/User/GetById/{id}` | GestorONG |
| `GET` | `/User/{id}/Total` | GestorONG (total de um usuário) |
| `PUT` | `/User/Update/{id}` | GestorONG |
| `DELETE` | `/User/Delete/{id}` | GestorONG |
| `GET` | `/Users/Me` | Autenticado (próprio perfil) |
| `PUT` | `/Users/Me` | Autenticado (autoatualização) |
| `DELETE` | `/Users/Me` | Autenticado (autorremoção) |
| `GET` | `/Statistics/MyTotal` | Autenticado (total do próprio usuário) |

**Health checks:** `/health/live` (liveness), `/health/ready` e `/health` (readiness — pinga o Mongo).

## Rodando localmente

**Pré-requisitos:** .NET 10 SDK e Docker.

### Configuração (segredos)

O `appsettings.json` versionado **não contém segredos** — `Jwt:Key`, `Mongo:ConnectionString` e todo o bloco `BootstrapGestor` (Email/Cpf/Password) vêm vazios. O `appsettings.example.json` documenta todas as chaves. Forneça os valores por **user-secrets** (dev) ou **variáveis de ambiente** (demais ambientes); ambos sobrescrevem o `appsettings.json`:

```bash
cd src/UserService
dotnet user-secrets set "Jwt:Key" "<32+ bytes aleatorios>" --project UserService.WebApi
dotnet user-secrets set "Mongo:ConnectionString" "<connection string do Atlas>" --project UserService.WebApi
dotnet user-secrets set "BootstrapGestor:Email" "<email do gestor>" --project UserService.WebApi
dotnet user-secrets set "BootstrapGestor:Cpf" "<cpf do gestor>" --project UserService.WebApi
dotnet user-secrets set "BootstrapGestor:Password" "<senha do gestor>" --project UserService.WebApi
```

O equivalente por variável de ambiente usa `__` no lugar de `:` (ex.: `Jwt__Key`). Para o container, as mesmas chaves ficam num `.env` (veja `.env.example`).

### Subindo

Com os segredos configurados, só o RabbitMQ precisa subir localmente:

```bash
docker compose up -d          # RabbitMQ (painel em http://localhost:15672 — guest/guest)
dotnet run --project UserService.WebApi
```

A API sobe em `http://localhost:5222`. Em ambiente de desenvolvimento, o **Swagger** fica em `http://localhost:5222/swagger`.

Para logar como gestor, use as credenciais do `BootstrapGestor` (e-mail e senha configurados nos secrets/`.env`) em `POST /Auth/Login` e envie o token no header `Authorization: Bearer <token>`.

### Mongo local (opcional)

Para rodar 100% offline (sem o Atlas), suba o Mongo do compose compartilhado — ele já vem configurado como replica set (necessário para as transações):

```bash
cd src
docker compose --profile local-mongo up -d
```

E aponte a conexão para ele (via variável de ambiente ou `appsettings`):

```
Mongo__ConnectionString=mongodb://localhost:27017/?replicaSet=rs0
Mongo__Database=user_db
```

## Testes

```bash
dotnet test
```

Testes unitários (xUnit + Moq) cobrindo serviços, autenticação, validação, mapeamento e o consumer (via test harness do MassTransit).
