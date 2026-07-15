# DevHabit

> API REST para rastreamento de hábitos e rotinas pessoais, construída em ASP.NET Core aplicando práticas modernas de design de APIs.

## Sobre o projeto

O DevHabit é uma API para criação e acompanhamento de hábitos: cadastro de hábitos, registro de progresso e consulta de rotinas ao longo do tempo. O projeto foi desenvolvido a partir do curso **Pragmatic REST APIs**, do Milan Jovanović, como forma de aplicar na prática um conjunto abrangente de boas práticas de design de APIs REST em .NET — versionamento, autenticação, jobs em background, observabilidade e containerização.

> Nota: é um projeto de estudo guiado por curso, mas implementado e ajustado por mim — abaixo estão as decisões técnicas e o que fica de aprendizado.

## Funcionalidades

- CRUD de hábitos e acompanhamento de progresso
- Autenticação e autorização via JWT
- Processamento de jobs em background
- Versionamento de API
- Observabilidade estruturada (tracing e telemetria)
- Cobertura de testes unitários e de integração (com Testcontainers)

## Arquitetura e decisões técnicas

- **Padrão de API:** design REST pragmático — recursos bem definidos, versionamento explícito, contratos consistentes de request/response
- **Autenticação:** JWT, com fluxo de emissão e renovação de token
- **Persistência:** PostgreSQL via EF Core
- **Observabilidade:** tracing distribuído e telemetria via OpenTelemetry (Seq ou .NET Aspire Dashboard como backend)
- **Containerização:** Docker, com suporte a ambiente de desenvolvimento e build de produção com imagem mínima
- **Estratégia de testes:** testes unitários cobrindo regras de negócio e handlers isoladamente, e testes de integração usando **Testcontainers** para subir dependências reais (ex: PostgreSQL) em containers efêmeros durante a execução dos testes — evitando mocks de banco e validando o comportamento real da camada de persistência

## Tecnologias utilizadas

- ASP.NET Core (.NET)
- PostgreSQL + Entity Framework Core
- JWT Authentication
- OpenTelemetry
- Docker / Docker Compose
- xUnit
- Testcontainers

## Como rodar o projeto

Pré-requisitos:
- .NET SDK
- Docker e Docker Compose

```bash
# clonar o repositório
git clone https://github.com/RichardKT88/DevHabit.git
cd DevHabit

# subir dependências (banco de dados, observabilidade)
docker compose up -d

# restaurar e rodar a API
dotnet restore
dotnet run --project DevHabit
```

A API estará disponível em `http://localhost:5000` (ajuste conforme sua configuração em `appsettings.json` / `.env`).

## Testes

O projeto conta com dois níveis de teste:

- **Testes unitários** — validam regras de negócio e handlers isoladamente, sem dependências externas
- **Testes de integração** — usam **Testcontainers** para provisionar um banco PostgreSQL real em container durante a execução dos testes, garantindo que as consultas e o comportamento da camada de persistência sejam validados contra o banco de verdade (não um in-memory ou mock), e sem exigir setup manual de infraestrutura local

```bash
dotnet test
```

> Pré-requisito: Docker precisa estar rodando localmente, já que o Testcontainers sobe os containers de teste automaticamente.

## Estrutura do projeto

```
DevHabit/                            # código-fonte da API
DevHabit.Api/                        # API REST
DevHabit.FunctionalTests/            # testes funcionais
DevHabit.Integration.Tests/          # testes de integração
DevHabit.Unit.Tests/                 # testes unitários
.github/workflows/                   # pipelines de CI
```

## Status do projeto

- Projeto de estudo, concluído / em evolução conforme novos módulos do curso são aplicados

## Aprendizados

Este projeto foi minha forma de estudar, na prática, um conjunto amplo de decisões de design de REST APIs em .NET — desde versionamento até observabilidade — que vão além do CRUD básico e se aproximam de cenários de produção.
