# OrderSystem

OrderSystem é um sistema de gerenciamento de pedidos desenvolvido em C# utilizando .NET 8. O projeto consiste em uma API web para gerenciar produtos, pedidos e autenticação, além de um serviço de worker para processamento assíncrono de eventos via Kafka.

## Arquitetura

O sistema é composto por dois principais componentes:

<img width="386" height="820" alt="image" src="https://github.com/user-attachments/assets/ebc775ff-a40a-4eeb-b52d-dd904b836f5d" />

- **OrderSystem**: API RESTful construída com ASP.NET Core, responsável por gerenciar produtos, pedidos e autenticação de usuários.
- **OrderWorkerService**: Serviço de worker que consome eventos de criação de pedidos via Kafka e processa-os de forma assíncrona.

### Tecnologias Utilizadas

- **Linguagem**: C#
- **Framework**: .NET 8 (ASP.NET Core)
- **Banco de Dados**: SQL Server
- **Mensageria**: Apache Kafka
- **Containerização**: Docker e Docker Compose
- **Outros**: Entity Framework Core (implicado pelos repositórios)

## Pré-requisitos

Antes de executar o projeto, certifique-se de ter instalado:

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [Docker](https://www.docker.com/get-started) e [Docker Compose](https://docs.docker.com/compose/install/)
- (Opcional) [Visual Studio 2022](https://visualstudio.microsoft.com/) ou [VS Code](https://code.visualstudio.com/) com extensões para C#

## Configuração e Execução

### 1. Clonagem do Repositório

Clone o repositório para sua máquina local:

```bash
git clone <url-do-repositorio>
cd OrderSystem
```

### 2. Configuração do Ambiente

O projeto utiliza Docker Compose para gerenciar dependências como SQL Server e Kafka. Para iniciar os serviços de infraestrutura:

```bash
docker-compose up -d
```

Isso iniciará os containers necessários em segundo plano.

### 3. Configuração da Aplicação

As configurações da aplicação estão nos arquivos `appsettings.json` e `appsettings.Development.json`. Verifique e ajuste as seguintes configurações se necessário:

- **Connection String do Banco de Dados**: Certifique-se de que aponta para o SQL Server em execução no Docker.
- **Configurações do Kafka**: Endereços dos brokers do Kafka.

### 4. Execução da API (OrderSystem)

Para executar a API web:

```bash
cd OrderSystem
dotnet restore
dotnet build
dotnet run
```

A API estará disponível em `https://localhost:5001` (ou conforme configurado em `launchSettings.json`).

### 5. Execução do Worker Service (OrderWorkerService)

Em um terminal separado:

```bash
cd OrderWorkerService
dotnet restore
dotnet build
dotnet run
```

## Endpoints da API

A API OrderSystem expõe os seguintes endpoints principais:

### Autenticação
- `POST /api/auth/login` - Login de usuário
- `POST /api/auth/register` - Registro de novo usuário

### Produtos
- `GET /api/products` - Listar produtos
- `POST /api/products` - Criar produto
- `GET /api/products/{id}` - Obter produto por ID
- `PUT /api/products/{id}` - Atualizar produto
- `DELETE /api/products/{id}` - Deletar produto

### Pedidos
- `GET /api/orders` - Listar pedidos
- `POST /api/orders` - Criar pedido
- `GET /api/orders/{id}` - Obter pedido por ID
- `PUT /api/orders/{id}` - Atualizar pedido
- `DELETE /api/orders/{id}` - Deletar pedido

Para testar os endpoints, você pode usar ferramentas como [Postman](https://www.postman.com/) ou o arquivo `OrderSystem.http` incluído no projeto.

## Estrutura do Projeto

```
OrderSystem/
├── Controllers/          # Controladores da API
├── Data/                 # Fábricas de conexão e scripts do banco
├── Messaging/            # Produtor Kafka
├── Models/               # Modelos de dados (Order, Product)
├── Repositories/         # Repositórios para acesso a dados
└── Contracts/            # Contratos de eventos
|
OrderWorkerService/
├── Consumers/            # Consumidor de eventos Kafka
├── Data/                 # Fábricas de conexão
├── Repositories/         # Repositórios
└── Contracts/            # Contratos de eventos
│
├── docker-compose.yml             # Orquestração de containers
├── .gitignore                     # Arquivos ignorados pelo Git
└── README.md                      # Documentação do projeto

```
## ⚙️ Fluxo de Processamento
<img width="1726" height="608" alt="image" src="https://github.com/user-attachments/assets/8a77f3a7-be97-4d32-935c-431b379b1253" />

## Banco de Dados

O sistema utiliza SQL Server. Os scripts de criação do banco estão em `OrderSystem/Data/ScriptsDb.txt`. Execute-os no SQL Server antes de iniciar a aplicação.

## Mensageria

O sistema utiliza Kafka para comunicação assíncrona. Quando um pedido é criado na API, um evento `OrderCreatedEvent` é publicado no Kafka, e o worker service o consome para processamento adicional.

## Desenvolvimento

Para contribuir com o projeto:

1. Faça um fork do repositório
2. Crie uma branch para sua feature (`git checkout -b feature/nova-feature`)
3. Commit suas mudanças (`git commit -am 'Adiciona nova feature'`)
4. Push para a branch (`git push origin feature/nova-feature`)
5. Abra um Pull Request

## Licença

Este projeto está licenciado sob a [MIT License](LICENSE).

## Contato

Para dúvidas ou sugestões, entre em contato com a equipe de desenvolvimento.
