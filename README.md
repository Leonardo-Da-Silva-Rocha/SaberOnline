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

## Estrutura do Projeto

A estrutura do projeto é organizada da seguinte forma:

- Services/
  - Aluno/             -> Contexto de Gestão de Alunos (matrículas, certificados, histórico)
  - Autenticacao/      -> Contexto de Autenticação e Controle de Acesso (JWT, identidade) 
  - Conteudo/          -> Contexto de Gestão de Conteúdo (cursos, aulas, material didático) AINDA NÃO IMPLEMENTADO
  - Core/              -> Núcleo compartilhado entre os contextos (entidades base, mensagens, mediadores) 
  - Faturamento/       -> Contexto de Pagamentos e Faturamento (transações, status, eventos) AINDA NÃO IMPLEMENTADO

- SaberOnline.API/     -> Projeto principal da API RESTful, responsável pela orquestração dos bounded contexts e exposição dos endpoints
  - Authenticantions/  -> Configurações e serviços de autenticação JWT
  - Configurations/    -> Configurações gerais da aplicação e injeção de dependências
  - Controllers/       -> Endpoints da API organizados por contexto
  - Data/              -> Acesso a dados e inicialização do banco (DbContexts e Seeds)
  - Enumerators/       -> Enumerações de uso comum
  - Filters/           -> Filtros globais de exceção e autorização
  - MigrationHelper/   -> Utilitários para migrações automáticas e criação do banco
  - Settings/          -> Classes de configuração e AppSettings
  - ViewModels/        -> Modelos de entrada e saída de dados (DTOs)


## Funcionalidades Implementadas

- ** Autenticação e Autorização: Implementação de login e cadastro utilizando ASP.NET Identity com JWT, permitindo autenticação segura de usuários.

- ** Integração com Aluno: Criação automática de um registro na tabela Aluno ao cadastrar um novo usuário, garantindo o vínculo entre a identidade e a persona do sistema.

- ** API RESTful: Estrutura inicial configurada para exposição de endpoints e integração entre os bounded contexts.

- **Configuração de Banco de Dados: Persistência de dados com Entity Framework Core, compatível com SQL Server e SQLite.

## **Como Executar o Projeto**

### **Pré-requisitos**

- .NET SDK 8.0 ou superior
- SQL Server
- Visual Studio 2022 ou superior (ou qualquer IDE de sua preferência)
- Git

### **Passos para Execução**

1. **Clone o Repositório:**
   
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
