# VetFlow API (.NET) — Challenge FIAP 2026

Solução desenvolvida para o **Challenge FIAP 2026** em parceria com a **CLYVO VET**.

## Integrantes do Grupo

| Nome | RM | Turma |
|------|----|-------|
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

Clean Architecture com 4 projetos:

```
VetFlow.sln
├── VetFlow.API           → Controllers, Extensions, Program.cs
├── VetFlow.Application   → DTOs, Interfaces de repositórios (IRepository, ISpecificRepositories)
├── VetFlow.Domain        → Entidades, Enums, BaseEntity (sem dependência externa)
└── VetFlow.Infrastructure → DbContext (EF Core), Configurations, Repositórios
```

## Tecnologias

| Camada | Tecnologia |
|--------|-----------|
| Framework | ASP.NET Core 9 |
| ORM | Entity Framework Core 9 |
| Banco (produção) | Oracle XE — `Oracle.EntityFrameworkCore` |
| Banco (desenvolvimento) | SQLite |
| Documentação | Swagger / OpenAPI (XML comments) |
| Build | dotnet CLI |

## Como Executar

### Desenvolvimento (SQLite — sem Oracle)

```bash
cd VetFlow.API
dotnet restore
dotnet run
```

Swagger disponível em: `http://localhost:5000` (raiz)

### Produção com Oracle FIAP

Edite `VetFlow.API/appsettings.json`:

```json
{
  "Database": { "UseSqlite": false },
  "ConnectionStrings": {
    "VetFlowOracle": "Data Source=oracle.fiap.com.br:1521/orcl;User ID=<SEU_RM>;Password=<SUA_SENHA>;"
  }
}
```

E `appsettings.Development.json`:

```json
{
  "Database": { "UseSqlite": false }
}
```

## Rotas da API

### Tutors `/api/Tutor`
| Método | Rota | Descrição | Status |
|--------|------|-----------|--------|
| GET | `/api/Tutor` | Lista todos os tutores | 200 |
| GET | `/api/Tutor/{id}` | Busca por Id | 200 / 404 |
| GET | `/api/Tutor/by-email?email=x` | Busca por e-mail | 200 / 404 |
| POST | `/api/Tutor` | Cria tutor | 201 / 400 |
| PUT | `/api/Tutor/{id}` | Atualiza tutor | 200 / 404 |
| DELETE | `/api/Tutor/{id}` | Remove tutor | 204 / 404 |

### Pets `/api/Pet`
| Método | Rota | Descrição | Status |
|--------|------|-----------|--------|
| GET | `/api/Pet` | Lista todos os pets | 200 |
| GET | `/api/Pet/{id}` | Busca por Id | 200 / 404 |
| GET | `/api/Pet/by-tutor/{tutorId}` | Pets de um tutor | 200 |
| POST | `/api/Pet` | Cadastra pet | 201 / 400 |
| PUT | `/api/Pet/{id}` | Atualiza pet | 200 / 404 |
| DELETE | `/api/Pet/{id}` | Remove pet | 204 / 404 |

### Clinics `/api/Clinic`
| Método | Rota | Descrição | Status |
|--------|------|-----------|--------|
| GET | `/api/Clinic` | Lista clínicas | 200 |
| GET | `/api/Clinic/{id}` | Busca por Id | 200 / 404 |
| POST | `/api/Clinic` | Cadastra clínica | 201 / 400 |
| PUT | `/api/Clinic/{id}` | Atualiza clínica | 200 / 404 |
| DELETE | `/api/Clinic/{id}` | Remove clínica | 204 / 404 |

### Appointments `/api/Appointment`
| Método | Rota | Descrição | Status |
|--------|------|-----------|--------|
| GET | `/api/Appointment` | Lista todos | 200 |
| GET | `/api/Appointment/{id}` | Busca por Id | 200 / 404 |
| GET | `/api/Appointment/by-pet/{petId}` | Por pet | 200 |
| GET | `/api/Appointment/pending` | Pendentes e futuros | 200 |
| POST | `/api/Appointment` | Cria agendamento | 201 / 400 |
| PUT | `/api/Appointment/{id}/complete` | Marca como concluído | 200 / 404 |
| DELETE | `/api/Appointment/{id}` | Remove | 204 / 404 |

### Vaccines `/api/Vaccine`
| Método | Rota | Descrição | Status |
|--------|------|-----------|--------|
| GET | `/api/Vaccine` | Lista todas | 200 |
| GET | `/api/Vaccine/{id}` | Busca por Id | 200 / 404 |
| GET | `/api/Vaccine/by-pet/{petId}` | Por pet | 200 |
| GET | `/api/Vaccine/expired` | Vencidas | 200 |
| POST | `/api/Vaccine` | Registra vacina | 201 / 400 |
| PUT | `/api/Vaccine/{id}` | Atualiza vacina | 200 / 404 |
| DELETE | `/api/Vaccine/{id}` | Remove | 204 / 404 |

### Medications `/api/Medication`
| Método | Rota | Descrição | Status |
|--------|------|-----------|--------|
| GET | `/api/Medication` | Lista todos | 200 |
| GET | `/api/Medication/{id}` | Busca por Id | 200 / 404 |
| GET | `/api/Medication/by-pet/{petId}` | Por pet | 200 |
| GET | `/api/Medication/active` | Ativos | 200 |
| POST | `/api/Medication` | Prescreve medicamento | 201 / 400 |
| PUT | `/api/Medication/{id}/suspend` | Suspende | 200 / 404 |
| PUT | `/api/Medication/{id}/complete` | Conclui | 200 / 404 |
| DELETE | `/api/Medication/{id}` | Remove | 204 / 404 |

## Benefícios para o Negócio

- Aumento da recorrência de consultas preventivas nas clínicas parceiras
- Redução de vacinas vencidas e abandono de tratamentos
- Histórico longitudinal estruturado por pet
- Base escalável para integração com app mobile e WhatsApp
