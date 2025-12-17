## Aplicação de uma plataforma educacional online

## Apresentação

Este projeto é uma entrega do MBA DevXpert Full Stack .NET e refere-se ao módulo Arquitetura, Modelagem e Qualidade de Software.

O principal objetivo é desenvolver uma plataforma educacional online com múltiplos bounded contexts
(BC), aplicando DDD, TDD, CQRS e padrões arquiteturais para gestão eficiente de
conteúdos educacionais, alunos e processos financeiros.


## Autor(es)
- Leonardo Da Silva Rocha

## Proposta do Projeto

O projeto consiste em:

- Desenvolver uma plataforma educacional online baseada em DDD, CQRS e TDD, que permita gerenciar cursos, alunos e pagamentos de forma modular e escalável.

## Tecnologias Utilizadas

- **Linguagem de Programação:** C#
- **Frameworks:**
  - ASP.NET Core Web API
  - Entity Framework Core
- **Banco de Dados:** SQL Server
- **Autenticação e Autorização:**
  - ASP.NET Core Identity
  - JWT (JSON Web Token) para autenticação na API

## 📁 Estrutura do Projeto

A solução **SaberOnline** é organizada em **bounded contexts**, separando responsabilidades de domínio, aplicação e API, seguindo princípios de **DDD leve** e **Clean Architecture**.

---

### 📦 Services/
Contém os contextos de negócio da aplicação, responsáveis pelas regras de domínio e aplicação.

- **Aluno/**  
  Contexto de Gestão de Alunos  
  Responsável por matrículas, certificados e histórico de aprendizado.

- **Autenticacao/**  
  Contexto de Autenticação e Controle de Acesso  
  Responsável por Identity, login, registro de usuários, claims e geração de tokens.

- **Conteudo/**  
  Contexto de Gestão de Conteúdo  
  Responsável por cursos, aulas e materiais didáticos.

- **Core/**  
  Núcleo compartilhado entre os contextos  
  Contém entidades base, notificações, validações, contratos e componentes comuns.

- **Faturamento/**  
  Contexto de Pagamentos e Faturamento  
  Responsável por pagamentos, transações, status e eventos de cobrança.

---

### 🌐 SaberOnline.API/
Projeto principal responsável por expor a **API REST** e orquestrar os contextos da aplicação.

- **Authentications/**  
  Configurações e serviços de autenticação  
  Inclui JWT, Identity, serviços de token e gerenciamento de claims.

- **Configurations/**  
  Configurações globais da aplicação  
  Configuração de Swagger, CORS, Injeção de Dependência, Identity e JWT.

- **Controllers/**  
  Endpoints da API organizados por contexto  
  Responsáveis por receber requisições HTTP e delegar ações aos serviços.

- **Data/**  
  Acesso a dados da API  
  Contém DbContexts, inicialização do banco e Seeds.

- **Enumerators/**  
  Enumerações de uso comum na aplicação  
  (status, tipos, perfis, etc.).

- **Filters/**  
  Filtros globais da API  
  Inclui filtros de exceção e autorização baseada em claims.

- **MigrationHelper/**  
  Utilitários para criação e migração automática do banco de dados.

- **Settings/**  
  Classes de configuração da aplicação  
  Mapeiam as seções do `appsettings.json`.

- **ViewModels/**  
  Modelos de entrada e saída de dados (DTOs)  
  Utilizados para requests e responses da API.


## 📌 Funcionalidades e Contextos de Negócio

A aplicação é composta por diferentes **Bounded Contexts**, cada um responsável por um conjunto específico de regras e funcionalidades.

- **BC Alunos**  
  Gerenciamento completo do aluno, incluindo cadastro, matrícula e acompanhamento do histórico acadêmico.

- **BC Conteúdo Programático**  
  Administração de cursos e aulas, permitindo o controle estruturado do conteúdo educacional.

- **BC Pagamentos**  
  Controle do processo de pagamento relacionado à matrícula dos alunos, incluindo status e validações.

- **Autenticação e Autorização**  
  Mecanismo de controle de acesso com diferenciação de perfis, garantindo permissões distintas para alunos e administradores.

- **API RESTful**  
  Exposição de endpoints para integração via API, com regras de acesso baseadas em autenticação e autorização.

- **Documentação da API**  
  Geração automática da documentação dos endpoints utilizando **Swagger**, facilitando o consumo e entendimento da API.



## 🧪 Testes Automatizados

A solução possui **cobertura completa de testes unitários**, garantindo confiabilidade e segurança na evolução do código.

- ✅ **100% dos Commands, Handlers, Entidades e Value Objects** possuem testes unitários.
- ✅ **Event Handlers** testados com **simulação de integração**, validando cenários reais de execução.
- ✅ **Consultas (Queries)** implementadas com **mocks** e **validação de retorno**, garantindo consistência dos dados.
- ✅ Testes focados em regras de negócio, validações e fluxos críticos da aplicação.


## **Como Executar o Projeto**

### **Pré-requisitos**

- .NET SDK 8.0 ou superior
- SQL Server
- Visual Studio 2022 ou superior (ou qualquer IDE de sua preferência)
- Git

### **Passos para Execução**

1. **Clone o Repositório:**
   em min
   - `https://github.com/Leonardo-Da-Silva-Rocha/SaberOnline.git`

2. **Configuração do Banco de Dados:**
   
   - No arquivo appsettings.json, você pode escolher qual banco de dados utilizar
   
   - SQLite (padrão) A string de conexão para SQLite já está configurada por padrão. O banco de dados será gerado automaticamente.
	
   - Caso prefira usar o SQL Server, altere a string de conexão.

4. **Executar a API:**
   
   - No Visual Studio, selecione o projeto API como projeto de inicialização.
   
   - Execute a aplicação.

## Instruções de Configuração

- **JWT para API:** As chaves de configuração do JWT estão no `appsettings.json`.
- **Migrações do Banco de Dados:** As migrações são gerenciadas pelo Entity Framework Core. Não é necessário aplicar devido a configuração do Seed de dados.

## Documentação da API

A documentação da API está disponível através do Swagger. Após iniciar a API, acesse a documentação em:

https://localhost:7150/swagger/index.html
