# Microservice Authentication API

## Descrição

A Microservice Authentication API é um microserviço desenvolvido em .NET para autenticação e autorização de usuários. Ele utiliza tecnologias como Identity Framework, JWT para autenticação baseada em tokens, e MySQL como banco de dados.

---

## Tecnologias Utilizadas

- **.NET 9.0** ou superior
- **MySQL**
- **Identity Framework** para autenticação de usuários
- **JWT** para tokens de autenticação
- **Swagger** para documentação interativa
- **Docker** para facilitar a execução do ambiente
- **Nginx** (proxy reverso)

---

## Estrutura do Projeto

- **src/API/Controller/v1/AuthController.cs**: Endpoints de autenticação.
- **src/Application/Services/AuthenticationService.cs**: Lógica de autenticação e registro.
- **src/Infrastructure/Auth/JwtAuthService.cs**: Geração e validação de tokens JWT.
- **src/Infrastructure/Persistence/Data/IdentityDbContext.cs**: Contexto do banco de dados Identity.
- **src/Infrastructure/Persistence/Data/SeedData.cs**: Seed inicial de roles e usuário admin.
- **src/Infrastructure/External/Notification/NotificationService.cs**: Integração para envio de SMS.

---

## Endpoints Principais

### 1. Login

- **POST** `/api/v1/auth/sign-in`
- **Body**:
  ```json
  {
    "username": "string",
    "password": "string"
  }
  ```
- **Retorno**: Token JWT ou mensagem de erro.

---

### 2. Dados do Usuário Autenticado

- **GET** `/api/v1/auth/me`
- **Headers**:
  ```
  Authorization: Bearer <token>
  ```
- **Retorno**: Dados do usuário autenticado.

---

### 3. Cadastro de Usuários

- **POST** `/api/v1/auth/register/admin` (apenas Admin)
- **POST** `/api/v1/auth/register/manager` (apenas Admin)
- **POST** `/api/v1/auth/register/patient`
- **POST** `/api/v1/auth/register/professional`
- **Body**:
  ```json
  {
    "firstName": "string",
    "lastName": "string",
    "userName": "string",
    "email": "string",
    "externalReferenceId": "string"
  }
  ```

---

### 4. Autenticação em Dois Fatores (2FA)

- **POST** `/api/v1/auth/enable-2fa`
  ```json
  { "username": "string", "phoneNumber": "string" }
  ```
- **POST** `/api/v1/auth/confirm-2fa`
  ```json
  { "username": "string", "code": "string" }
  ```
- **POST** `/api/v1/auth/verify-2fa`
  ```json
  { "username": "string", "code": "string" }
  ```

---

### 5. Recuperação e Troca de Senha

- **POST** `/api/v1/auth/requestResetPassword`
  ```json
  { "username": "string" }
  ```
- **POST** `/api/v1/auth/resetPassword`
  ```json
  { "username": "string", "code": "string", "newPassword": "string" }
  ```
- **POST** `/api/v1/auth/changePassword` (autenticado)
  ```json
  { "currentPassword": "string", "newPassword": "string" }
  ```

---

## Como Executar

### 1. Configuração do `.env`

Crie um arquivo `.env` na raiz do projeto com as variáveis do arquivo `env.example`.

### 2. Executando com Docker

```sh
docker-compose up --build
```

- API: http://localhost:5007
- NGINX: http://localhost:8080

### 3. Executando Localmente

- Certifique-se que o MySQL está rodando.
- Execute:
  ```sh
  dotnet run
  ```

---

## Observações

- O seed inicial cria um usuário admin padrão (`admin@admin.com` / senha: `Admin123!`).
- O serviço de notificação (SMS) depende da variável `NOTIFICATION_URL`.
- O projeto já está pronto para versionamento de API e uso de políticas de autorização por role.

---
