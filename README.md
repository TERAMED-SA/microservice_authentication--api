# Microservice Authentication API

## Descrição

A Microservice Authentication API é um microserviço desenvolvido em .NET para autenticação e autorização de usuários. Ele utiliza tecnologias como Identity Framework, JWT para autenticação baseada em tokens, e MySQL como banco de dados.

## Tecnologias Utilizadas

- **.NET 9.0** ou superior
- **MySQL**
- **Identity Framework** para autenticação de usuários
- **JWT** para tokens de autenticação
- **Swagger** para documentação interativa
- **Docker** para facilitar a execução do ambiente

## Estrutura do Projeto

### Diretórios Principais

- **`src/API/Controller/v1/AuthController.cs`**: Controlador principal para endpoints de autenticação.
- **`src/Application/Services/AuthenticationService.cs`**: Implementação dos serviços de autenticação.
- **`src/Infrastructure/Auth/JwtAuthService.cs`**: Serviço para geração e validação de tokens JWT.
- **`src/Infrastructure/Persitence/Data/IdentityDbContext.cs`**: Contexto do banco de dados para gerenciar usuários e roles.

## Endpoints Disponíveis

### Autenticação

1. **Registrar Usuário**

   - **Rota**: `POST /api/v1/auth/signup`
   - **Descrição**: Cria uma nova conta de usuário.
   - **Payload**:
     ```json
     {
       "username": "string",
       "password": "string",
       "email": "string"
     }
     ```

2. **Login**

   - **Rota**: `POST /api/v1/auth/signin`
   - **Descrição**: Autentica um usuário e retorna um token JWT.
   - **Payload**:
     ```json
     {
       "username": "string",
       "password": "string"
     }
     ```

3. **Obter Informações do Usuário**
   - **Rota**: `GET /api/v1/auth/me`
   - **Descrição**: Retorna informações do usuário autenticado.
   - **Cabeçalho**:
     ```
     Authorization: Bearer <token>
     ```

## Configuração e Execução

### Passo 1: Configurar o Arquivo `.env`

Crie um arquivo `.env` na raiz do projeto com as variáveis que estão dentro do arquivo "env.example"

### Passo 2: Executar com Docker

1. Construa e inicie os contêineres:

   ```sh
   docker-compose up --build

   ```

2. Acesse a API
   http://localhost:5007.

3. Acesse o NGINX
   http://localhost:8080

### Passo 3: Executar Localmente

1. Certifique-se de que o MySQL está rodando.
2. Execute o projeto:

```sh
  dotnet run
```
