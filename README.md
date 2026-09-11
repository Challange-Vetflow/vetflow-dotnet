# VetFlow API (.NET) — Challenge FIAP 2026

Solução desenvolvida para o Challenge FIAP 2026 em parceria com a CLYVO VET.

## Integrantes do Grupo

| Nome | RM | Turma |
|---|---|---|
| Andrei de Paiva Gibbini | 563061 | 2TDSPF |
| Pedro Sakai Silva Zambaca | 565956 | 2TDSPF |
| Pedro Santos Pequini | 561842 | 2TDSPF |
| Arthur Câmara | 562310 | 2TDSPG |
| Diogo Cunha | 563654 | 2TDSPF |

## Problema de Negócio

Tutores de pets só acionam clínicas em urgências ou vacinas óbvias. Isso gera baixa recorrência, menor LTV para as clínicas e histórico clínico fragmentado.

## Solução

API REST construída com ASP.NET Core 9 que centraliza o histórico clínico do pet, organiza agendamentos, registra vacinas e medicamentos — servindo como backend para app mobile, dashboard clínico e integrações via WhatsApp.

## Arquitetura

Clean Architecture com 6 projetos:

```
VetFlow.sln
├── VetFlow.API                → Controllers, Extensions, Program.cs
├── VetFlow.Application         → DTOs, Interfaces de repositórios
├── VetFlow.Domain              → Entidades, Enums, BaseEntity (sem dependência externa)
├── VetFlow.Infrastructure      → DbContext (EF Core), Configurations, Repositórios
├── VetFlow.UnitTests           → Testes unitários (Domínio + Aplicação)
└── VetFlow.IntegrationTests    → Testes de integração (endpoints ponta a ponta)
```

## Tecnologias

| Camada | Tecnologia |
|---|---|
| Framework | ASP.NET Core 9 |
| ORM | Entity Framework Core 9 |
| Banco (produção) | Oracle XE — Oracle.EntityFrameworkCore |
| Banco (desenvolvimento) | SQLite |
| Documentação | Swagger / OpenAPI |
| Logging | Serilog (console + arquivo) |
| Observabilidade | Health Checks + OpenTelemetry |
| Testes | xUnit + Moq + WebApplicationFactory |

## Como Executar

### Desenvolvimento (SQLite)

```bash
cd VetFlow.API
dotnet restore
dotnet run
```

Swagger disponível em: http://localhost:5000

Em desenvolvimento a API Key já vem configurada (`dev-local-key`, ver
`appsettings.Development.json`) só para facilitar testes manuais locais —
mande `X-Api-Key: dev-local-key` nas chamadas. Em produção essa chave é
obrigatoriamente sobrescrita por variável de ambiente ou user-secrets (ver
seção Autenticação).

### Produção com Oracle FIAP

Nunca commite usuário/senha do Oracle em `appsettings.json` (por isso ele vai
vazio no repositório). Prefira variável de ambiente ou `dotnet user-secrets`:

```bash
cd VetFlow.API
dotnet user-secrets set "ConnectionStrings:VetFlowOracle" "Data Source=oracle.fiap.com.br:1521/orcl;User ID=<SEU_RM>;Password=<SUA_SENHA>;"
dotnet user-secrets set "Authentication:ApiKey" "<UMA_CHAVE_QUALQUER>"
```

Em ambiente de produção real, use variáveis de ambiente equivalentes
(`ConnectionStrings__VetFlowOracle` e `Authentication__ApiKey`), que o
ASP.NET Core lê automaticamente e sobrescrevem o `appsettings.json`.

## Autenticação

A API usa autenticação simples por API Key no header `X-Api-Key`, aplicada a
todos os controllers (`[Authorize]`). Os endpoints de Health Check
(`/health`, `/health/live`, `/health/ready`) continuam públicos, para não
atrapalhar ferramentas de monitoramento externas.

```bash
curl -H "X-Api-Key: <SUA_CHAVE>" http://localhost:5000/api/Tutor
```

Sem o header, ou com uma chave inválida, a API responde `401 Unauthorized`.
Essa chave é a mesma configurada em `Authentication:ApiKey` (env var ou
user-secrets, ver seção anterior). Nos testes de integração, a chave usada é
fixa e local ao processo de teste (`VetFlowApiFixture.TestApiKey`), sem
depender de nenhuma configuração externa.

## Monitoramento e Observabilidade

### Health Checks

A API expõe três endpoints de verificação de saúde:

| Endpoint | Descrição |
|---|---|
| `GET /health` | Health check completo em formato HealthCheckUI (todos os checks) |
| `GET /health/live` | Liveness — confirma que o processo da API está no ar |
| `GET /health/ready` | Readiness — confirma conectividade com o banco Oracle |
| `GET /health-ui` | Painel visual com histórico dos health checks |

Exemplo de resposta de `/health`:

```json
{
  "status": "Healthy",
  "totalDuration": "00:00:00.0842",
  "entries": {
    "oracle-database": { "status": "Healthy", "tags": ["db", "oracle", "ready"] },
    "self": { "status": "Healthy", "tags": ["live"] }
  }
}
```

### Logging Estruturado (Serilog)

Todo request é logado com nível apropriado (`Information`, `Warning`, `Error`) e um `X-Correlation-Id` único por requisição, propagado em todo o pipeline. Os logs saem simultaneamente para:

- **Console**: formato legível durante desenvolvimento
- **Arquivo**: `logs/vetflow-{data}.log`, com rotação diária

### Tracing e Métricas (OpenTelemetry)

A aplicação instrumenta automaticamente:

- **Tracing distribuído** de cada requisição HTTP recebida e chamadas HTTP feitas pela API, exportado no console.
- **Métricas de runtime** (uso de memória, threads, GC) e **métricas ASP.NET Core** (tempo de resposta por rota, contagem de requisições e taxa de erros).

## Testes Automatizados

O projeto segue o padrão **AAA (Arrange, Act, Assert)** em todos os testes, com nomenclatura `MetodoTestado_Cenario_ResultadoEsperado`.

### Rodar todos os testes

```bash
dotnet test
```

### Rodar só os testes unitários

```bash
dotnet test VetFlow.UnitTests
```

### Rodar só os testes de integração

```bash
dotnet test VetFlow.IntegrationTests
```

### Organização

- **VetFlow.UnitTests** — testes de Domínio (regras de negócio das entidades `Tutor` e `Pet`) e de Aplicação (`TutorController` com repositório mockado via Moq), sem dependência de banco de dados real.
- **VetFlow.IntegrationTests** — testes end-to-end usando `WebApplicationFactory<Program>`, validando o fluxo HTTP completo (criação, busca, atualização e remoção de recursos, incluindo respostas de erro). Usa uma **Collection Fixture** (`VetFlowApiFixture`) para compartilhar uma única instância da API em memória entre todos os testes da coleção, evitando overhead de subir a aplicação a cada teste.

## Endpoints

### Tutors `/api/Tutor`

| Método | Rota | Descrição | Status |
|---|---|---|---|
| GET | /api/Tutor | Lista todos os tutores | 200 |
| GET | /api/Tutor/{id} | Busca por Id | 200 / 404 |
| GET | /api/Tutor/by-email?email=x | Busca por e-mail | 200 / 404 |
| POST | /api/Tutor | Cria tutor | 201 / 400 |
| PUT | /api/Tutor/{id} | Atualiza tutor | 200 / 404 |
| DELETE | /api/Tutor/{id} | Remove tutor | 204 / 404 |

### Pets `/api/Pet`

| Método | Rota | Descrição | Status |
|---|---|---|---|
| GET | /api/Pet | Lista todos os pets | 200 |
| GET | /api/Pet/{id} | Busca por Id | 200 / 404 |
| GET | /api/Pet/by-tutor/{tutorId} | Pets de um tutor | 200 |
| POST | /api/Pet | Cadastra pet | 201 / 400 |
| PUT | /api/Pet/{id} | Atualiza pet | 200 / 404 |
| DELETE | /api/Pet/{id} | Remove pet | 204 / 404 |

### Clinics `/api/Clinic`

| Método | Rota | Descrição | Status |
|---|---|---|---|
| GET | /api/Clinic | Lista clínicas | 200 |
| GET | /api/Clinic/{id} | Busca por Id | 200 / 404 |
| POST | /api/Clinic | Cadastra clínica | 201 / 400 |
| PUT | /api/Clinic/{id} | Atualiza clínica | 200 / 404 |
| DELETE | /api/Clinic/{id} | Remove clínica | 204 / 404 |

### Appointments `/api/Appointment`

| Método | Rota | Descrição | Status |
|---|---|---|---|
| GET | /api/Appointment | Lista todos | 200 |
| GET | /api/Appointment/{id} | Busca por Id | 200 / 404 |
| GET | /api/Appointment/by-pet/{petId} | Por pet | 200 |
| GET | /api/Appointment/pending | Pendentes e futuros | 200 |
| POST | /api/Appointment | Cria agendamento | 201 / 400 |
| PUT | /api/Appointment/{id}/complete | Marca como concluído | 200 / 404 |
| DELETE | /api/Appointment/{id} | Remove | 204 / 404 |

### Vaccines `/api/Vaccine`

| Método | Rota | Descrição | Status |
|---|---|---|---|
| GET | /api/Vaccine | Lista todas | 200 |
| GET | /api/Vaccine/{id} | Busca por Id | 200 / 404 |
| GET | /api/Vaccine/by-pet/{petId} | Por pet | 200 |
| GET | /api/Vaccine/expired | Vencidas | 200 |
| POST | /api/Vaccine | Registra vacina | 201 / 400 |
| PUT | /api/Vaccine/{id} | Atualiza vacina | 200 / 404 |
| DELETE | /api/Vaccine/{id} | Remove | 204 / 404 |

### Medications `/api/Medication`

| Método | Rota | Descrição | Status |
|---|---|---|---|
| GET | /api/Medication | Lista todos | 200 |
| GET | /api/Medication/{id} | Busca por Id | 200 / 404 |
| GET | /api/Medication/by-pet/{petId} | Por pet | 200 |
| GET | /api/Medication/active | Ativos | 200 |
| POST | /api/Medication | Prescreve medicamento | 201 / 400 |
| PUT | /api/Medication/{id}/suspend | Suspende | 200 / 404 |
| PUT | /api/Medication/{id}/complete | Conclui | 200 / 404 |
| DELETE | /api/Medication/{id} | Remove | 204 / 404 |

## Benefícios para o Negócio

- Aumento da recorrência de consultas preventivas nas clínicas parceiras
- Redução de vacinas vencidas e abandono de tratamentos
- Histórico longitudinal estruturado por pet
- Base escalável para integração com app mobile e WhatsApp
- Observabilidade completa para diagnóstico rápido de falhas em produção
