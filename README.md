# Conexão Solidária — Esperança Solidária

MVP de plataforma para a ONG **Esperança Solidária** gerenciar campanhas de arrecadação e receber doações, com foco em escalabilidade, observabilidade e automação.

Projeto do Hackathon — Pós-Tech.

---

## Sobre o problema

A ONG Esperança Solidária atua há mais de 10 anos acolhendo crianças em situação de vulnerabilidade. A gestão de doadores e campanhas era feita manualmente, limitando a capacidade de expansão da ONG. Esta plataforma digitaliza esse processo, entregando um MVP arquitetado para crescer.

---

## Arquitetura

Monorepo com microsserviços em .NET, comunicação assíncrona via mensageria e persistência em MongoDB.

```
.
├─ .github/workflows/     # pipelines de CI/CD (um par CI/CD por serviço)
├─ k8s/                   # manifests Kubernetes (namespaces, infra, serviços)
└─ src/
   ├─ Shared/EsperancaSolidaria.Contracts   # eventos e contratos compartilhados
   ├─ UserService/                          # usuários, autenticação, consumidor de doações
   ├─ DonationService/                      # produtor de eventos de doação
   └─ CampaignService/                      # CRUD de campanhas, consumidor de doações
```

| Serviço | Responsabilidade |
|---|---|
| **UserService** | Cadastro/login (JWT), gestão de usuários, mantém o total de doações por usuário. Consome `DonationReceivedEvent` de forma idempotente (transações Mongo + registro por `MessageId`). |
| **DonationService** | Recebe a intenção de doação e **publica** `DonationReceivedEvent` no broker — não grava o valor arrecadado diretamente. |
| **CampaignService** | CRUD de campanhas (criar, editar, cancelar) e consumidor do evento de doação para atualizar o valor arrecadado. |
| **Contracts** | DTOs e eventos compartilhados, fonte única de verdade para o roteamento no MassTransit. |

`[AJUSTAR]` — inserir aqui a imagem/link do diagrama de arquitetura entregável (mostrando microsserviços, bancos, broker e observabilidade), conforme exigido pelo enunciado.

---

## Funcionalidades

**Autenticação e autorização (RBAC)**
- Login via JWT.
- Duas roles: `GestorONG` e `Doador`.
- Endpoints de gestão restritos a `GestorONG`.

**Gestão de campanhas** *(GestorONG)*
- Criar/editar campanha com `Título`, `Descricao`, `DataInicio`, `DataFim`, `MetaFinanceira`, `Status` (`Ativa`, `Concluida`, `Cancelada`).
- Regras: data de término não pode estar no passado; meta financeira deve ser maior que zero.

**Cadastro de doador** *(público)*
- Campos: Nome completo, Email (único), CPF (validado), Senha (hash com BCrypt).

**Painel de transparência** *(público)*
- Lista apenas campanhas com status `Ativa`, exibindo título, meta financeira e valor total arrecadado até o momento.

**Doação** *(doador autenticado)*
- Envio de intenção de doação com `IdCampanha` e `ValorDoacao`.
- Não é permitido doar para campanhas encerradas ou canceladas.
- O valor arrecadado **não** é atualizado direto no banco pela API: o `DonationService` publica `DonationReceivedEvent`, e o `CampaignService` consome esse evento para atualizar o total.

---

## Stack

- **.NET 10** — microsserviços em arquitetura Clean/Onion (Domain, Application, Infra, WebApi)
- **MongoDB** — persistência, com transações no `UserService` para consumo idempotente de eventos
- **RabbitMQ + MassTransit** — mensageria assíncrona entre serviços
- **JWT** — autenticação e autorização baseada em roles
- **xUnit + Moq** — testes unitários
- **Prometheus** (`prometheus-net`) — métricas via `app.MapMetrics()`
- **Swagger** — documentação de API em ambiente de desenvolvimento
- **Kubernetes** — orquestração dos 3 serviços e infra (RabbitMQ, Grafana, Zabbix), manifests em `k8s/`
- **Zabbix + Grafana** — monitoramento e dashboards de CPU, threads, requisições HTTP e memória dos 3 serviços
- **GitHub Actions** — CI (build/test/validação de imagem) e CD (publicação no GHCR com scan Trivy e assinatura Cosign) por serviço

---

## Como rodar localmente

**Pré-requisitos:** .NET 10 SDK, Docker.

**1. Subir a infraestrutura**

```bash
cd src
docker compose up -d                          # RabbitMQ
docker compose --profile local-mongo up -d    # opcional: Mongo replica set (necessário para transações)
```

- RabbitMQ expõe `5672` (AMQP) e `15672` (painel de gestão web, `http://localhost:15672`, usuário/senha `guest`/`guest`).
- O MongoDB local é **opcional**: por padrão os serviços usam o Atlas configurado no `appsettings`; suba o Mongo local só se quiser rodar 100% offline. Sobe como replica-set single-node (`rs0`), pois a idempotência do consumo de eventos depende de transações Mongo. Connection string local: `mongodb://localhost:27017/?replicaSet=rs0&directConnection=true`.
- `docker compose down` mantém os volumes; `docker compose down -v` zera os dados.
- **Atenção:** não rode este compose junto com `src/UserService/docker-compose.yml` — ambos publicam a porta `5672` e usam o mesmo nome de container. Este é a infra compartilhada do monorepo; aquele roda o `UserService` isolado.

**2. Configurar variáveis de ambiente / secrets**

O repositório mantém apenas configurações não sensíveis em `appsettings.json`. Segredos devem ser fornecidos via variáveis de ambiente, user-secrets ou `.env`.

Em .NET, use `__` como separador de configurações aninhadas (ex.: `Jwt__Key` para `Jwt:Key`).

O `UserService` inclui um arquivo de exemplo em `src/UserService/.env.example` com as variáveis esperadas (chave JWT, connection string do Mongo, credenciais do gestor inicial).

**3. Rodar um serviço**

```bash
dotnet run --project src/UserService/UserService.WebApi
```

Cada WebApi expõe Swagger em ambiente de desenvolvimento. As portas padrão estão configuradas no `appsettings.json` de cada projeto.

---

## Kubernetes

Os manifests ficam em `k8s/`, organizados em namespaces, infraestrutura e serviços:

```
k8s/
 ├─ namespaces.yaml
 ├─ infra/
 │   ├─ grafana.yaml
 │   ├─ rabbitmq.yaml
 │   └─ zabbix.yaml
 └─ services/
     ├─ configmap.yaml
     ├─ secrets.yaml
     ├─ campaign-service/   (configmap, deployment, secrets, service)
     ├─ donation-service/   (configmap, deployment, secrets, service)
     └─ user-service/       (configmap, deployment, secrets, service)
```

**Deploy no cluster** (Minikube/Kind/Docker Desktop K8s):

```bash
# namespace
kubectl apply -f k8s/namespaces.yaml

# infraestrutura (RabbitMQ, Grafana, Zabbix)
kubectl apply -f k8s/infra/

# configs e secrets compartilhados
kubectl apply -f k8s/services/configmap.yaml
kubectl apply -f k8s/services/secrets.yaml

# cada microsserviço
kubectl apply -f k8s/services/campaign-service/
kubectl apply -f k8s/services/donation-service/
kubectl apply -f k8s/services/user-service/
```

**Detalhes de cada deployment de serviço:**
- Imagem consumida diretamente do GHCR (ex.: `ghcr.io/tech-challenge-fiap-58/campaign-service:latest`), publicada pelo CD.
- `initContainer` que aguarda o RabbitMQ ficar disponível (`nc -z rabbitmq 5672`) antes de subir o container principal — evita crash loop por dependência de mensageria ainda não pronta.
- Configuração via `ConfigMap` (específico do serviço + um compartilhado `services-config`) e `Secret` (`services-secrets`), incluindo connection string, chave JWT, e credenciais do RabbitMQ.
- `readinessProbe` e `livenessProbe` apontando para `/health` na porta `8080`.
- `resources` com requests/limits definidos (ex.: `100m`/`300m` de CPU, `128Mi`/`256Mi` de memória).
- **Exposição:** cada serviço tem um `Service` do tipo `NodePort` (ex.: `campaign-service` expõe a porta `80` → `targetPort 8080`, acessível externamente via `nodePort 30080`).

Verificar os pods:

```bash
kubectl get pods -n services
```

---

## Variáveis de ambiente e configuração

Cada serviço combina três fontes de configuração: `appsettings.json` (não sensível), `ConfigMap` (K8s) e `Secret`/`.env` (sensível).

**ConfigMaps (Kubernetes)**

`services-config` (compartilhado entre os 3 serviços):

| Chave | Valor |
|---|---|
| `ASPNETCORE_ENVIRONMENT` | `Development` |
| `Jwt__Issuer` | `esperanca_solidaria` |
| `Jwt__Audience` | `esperanca_solidaria` |

`campaign-config` (específico do CampaignService — os demais seguem o mesmo padrão, com o nome do banco correspondente):

| Chave | Valor |
|---|---|
| `Mongo__Database` | `campaign-db` |

**Secrets / variáveis sensíveis** (via `services-secrets` no K8s, ou `.env` localmente):

| Variável | Descrição |
|---|---|
| `Mongo__ConnectionString` | Connection string do MongoDB (Atlas em produção) — **nunca commitar em texto puro** |
| `Jwt__Key` | Chave de assinatura do JWT (gerar 32+ bytes aleatórios) |
| `Jwt__Expiration` | Expiração do token |
| `RabbitMQ__UserName` / `RabbitMQ__Password` | Credenciais do RabbitMQ |
| `RabbitMQ__VirtualHost` / `RabbitMQ__Host` | Vhost e host do broker |
| `BootstrapGestor__Email` / `BootstrapGestor__Cpf` / `BootstrapGestor__Password` | Credenciais do gestor inicial (seed), usadas pelo `UserService` |

O `UserService` traz um exemplo completo em `src/UserService/.env.example`, com instruções de uso (`docker run --env-file .env ...` ou `env_file: .env` no compose).

---

## Testes

```bash
dotnet test
```

Executa os testes unitários (xUnit + Moq) presentes no `UserService` e no `CampaignService`.

---

## Observabilidade

- **Health checks:** `/health/live` (liveness) e `/health/ready` (readiness) em cada WebApi.
- **Métricas:** expostas via Prometheus (`prometheus-net`), com um contador de requisições instrumentado em `Program.cs` de cada serviço.
- **Swagger:** disponível em ambiente de desenvolvimento.
- **Zabbix → Grafana:** métricas coletadas no Zabbix e visualizadas em dashboards no Grafana, para os três serviços (`campaign-service`, `user-service`, `donation-service`):
  - Process CPU Seconds
  - Threads Count
  - HTTP Total Requests
  - Memory Usage

  `[AJUSTAR]` — inserir aqui prints dos dashboards (um por serviço ou um consolidado) e, se possível, o link de acesso ao Grafana.

---

## CI/CD

Pipelines via GitHub Actions, um par de CI/CD por serviço, em `.github/workflows/`:

| Workflow | Responsabilidade |
|---|---|
| `01-CI-Campaign.yml` | CI do CampaignService |
| `01-CI-Donation.yml` | CI do DonationService |
| `01-CI-User.yml` | CI do UserService |
| `02-CD-Campaign.yml` | CD do CampaignService |
| `02-CD-Donation.yml` | CD do DonationService |
| `02-CD-User.yml` | CD do UserService |

**Gatilho (CI):** roda em Pull Requests para `main` e `develop`, e em pushes diretos para `develop` — sempre filtrado por path (só dispara se houver mudança em `src/<Service>/**` ou `src/Shared/**`, evitando builds desnecessários dos outros serviços).

**O que o CI faz**, por serviço:
1. Checkout do código
2. Setup do .NET 10
3. `dotnet restore` + `dotnet build` (Release)
4. `dotnet test` com coleta de cobertura (`XPlat Code Coverage`)
5. Upload dos resultados de teste como artifact
6. **Validação** do build da imagem Docker (`docker build -t <service>:ci -f <Service>.WebApi/Dockerfile ..`) — builda localmente para garantir que o Dockerfile funciona, **sem publicar em nenhum registry**
7. Upload dos binários de build como artifact

**Gatilho (CD):** roda em push direto para `main`, também filtrado por path (`src/<Service>/**` ou `src/Shared/**`).

**O que o CD faz**, por serviço:
1. Checkout, setup .NET 10, `dotnet restore`/`build`/`test`
2. Login no **GHCR** (`ghcr.io`, autenticado via `GITHUB_TOKEN`)
3. Build da imagem Docker, taggeada com `github.sha` e `latest`:
   - `ghcr.io/tech-challenge-fiap-58/campaign-service`
   - `ghcr.io/tech-challenge-fiap-58/donation-service`
   - `ghcr.io/tech-challenge-fiap-58/user-service`
4. **Push** das duas tags pro GHCR
5. **Scan de vulnerabilidades** com Trivy (falha o pipeline em achados `CRITICAL` sem correção disponível)
6. **Assinatura da imagem** com Cosign (`cosign sign`)

> Deploy no cluster (`kubectl apply`) não faz parte do CD atual — é feito manualmente, conforme a seção [Kubernetes](#kubernetes).

---

## Endpoints principais

**UserService** (`AuthController`, `UsersController`):

| Método | Rota | Acesso | Descrição |
|---|---|---|---|
| `POST` | `/Auth/Login` | Público | Autenticação, retorna JWT |
| `POST` | `/Auth/Register` | Público | Cadastro de doador |
| `GET` | `/Users/Me` | Autenticado | Perfil do usuário logado |
| `PUT` | `/Users/Me` | Autenticado | Autoatualização de dados |
| `DELETE` | `/Users/Me` | Autenticado | Autorremoção da conta |

> A identidade em `/Users/Me` vem do token JWT (não da rota) — o usuário só age sobre a própria conta. Gestão de terceiros por um `GestorONG` fica em outro controller do serviço.

**CampaignService** (`CampaignController`):

| Método | Rota | Descrição |
|---|---|---|
| `GET` | `/Campaign` | Lista campanhas |
| `GET` | `/Campaign/{id}` | Busca campanha por id |
| `POST` | `/Campaign` | Cria campanha |
| `PUT` | `/Campaign` | Atualiza campanha |
| `PUT` | `/Campaign/{id}/cancel` | Cancela campanha |
| `POST` | `/Campaign/donation` | Registra intenção de doação |

`[AJUSTAR]` — confirmar as restrições de role (`GestorONG` vs público vs `Doador`) em cada rota do `CampaignController`; os atributos `[Authorize(Roles=...)]` não estavam visíveis no trecho analisado.

---

## Contrato de mensageria

`Shared/EsperancaSolidaria.Contracts` define o evento `DonationReceivedEvent`:

| Campo | Descrição |
|---|---|
| `DonationId` | Identificador da doação |
| `DonorUserId` | Identificador do doador |
| `CampaignId` | Identificador da campanha |
| `Amount` | Valor doado |
| `OccurredAt` | Data/hora do evento |

---

## Documentação adicional

- Detalhes do `UserService` (autenticação, consumidor idempotente, seed inicial): `src/UserService/README.md`
- DI e configuração do MassTransit: pasta `Extensions` e `appsettings.example.json` de cada WebApi.
- `[AJUSTAR]` Documento PDF justificando a escolha dos bancos de dados (entregável separado).

---

## Estrutura de commits e contribuição

O projeto segue arquitetura em camadas: regras de domínio em `Domain`, orquestração/casos de uso em `Application`, implementações concretas em `Infra` e a superfície HTTP em `WebApi`.

---

## Licença

Projeto educacional (MVP de hackathon). Consulte os autores para uso ou licenciamento.
