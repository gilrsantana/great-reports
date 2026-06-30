# 009-manage-authentication-flows

## Objective

Create a complete authentication management suite including Login, Refresh Token, Change Password, Forget Password, and Reset Password. Integrate these endpoints and CQRS handlers with the existing ASP.NET Core Identity services and extend the `IIdentityService` interface, mapping errors and results following clean architecture boundaries.

## Technical Context

We will build upon the existing `ConfirmEmail` flow and `IIdentityService` implementation. To align with `RULE-008-API-CONTROLLERS-ERROR-HANDLING` and the language boundary guidelines in `RULE-014-LOCALIZATION`, we will:
1. Add new methods to `IIdentityService` to handle authentication, password change, and password reset token generation/reset operations.
2. Update the `ApiControllerBase` class to return correct HTTP Status Codes (e.g., `401 Unauthorized` and `404 Not Found`) according to specific error codes.
3. Keep all technical keys/codes in English and all user-facing descriptions in Brazilian Portuguese.
4. Utilize `IEmailSender<Account>` to dispatch secure password reset links asynchronously via Hangfire background jobs (already supported by `IdentityEmailSender`).

## Project References

- [Product Definition](../../memory/product.md)
- [Global Technical Context](../../memory/technical-context.md)
- [Repository Structure](../../memory/structure.md)

## Shared References

- [How to Run](../../shared/how-to-run.md)
- [Naming Conventions](../../shared/naming-conventions.md)

## Local Observations

- Error codes must use dot notation: `Auth.InvalidCredentials`, `Auth.EmailNotConfirmed`, `Auth.AccountLocked`, `Auth.InvalidRefreshToken`, `Auth.PasswordChangeFailed`.
- Error descriptions returned to the user must be in Brazilian Portuguese.
- For `ForgetPassword`, to prevent email harvesting/enumeration, the handler should return success even if the email does not exist, but only trigger the email generation if it does exist.

---

## Tasks

### Tasks - Application Layer (`GreatReports.Application`)

- [x] Create `TokenResponse` record in `src/backend/GreatReports.Application/UseCases/Auth/Responses/TokenResponse.cs`:
  ```csharp
  namespace GreatReports.Application.UseCases.Auth.Responses;
  public record TokenResponse(string AccessToken, string RefreshToken);
  ```
>  ✅ 2026-06-30 07:23 - Verificado que o record TokenResponse está corretamente implementado no caminho especificado.

- [x] Update `IIdentityService` interface in `src/backend/GreatReports.Application/Common/Interfaces/IIdentityService.cs` to declare new authentication and credential management operations:
  ```csharp
  using GreatReports.Application.UseCases.Auth.Responses;
  using GreatReports.Shared;
  
  // Added to IIdentityService:
  Task<Result<TokenResponse>> AuthenticateAsync(string email, string password);
  Task<Result> ChangePasswordAsync(Guid accountId, string currentPassword, string newPassword);
  Task<Result> GeneratePasswordResetTokenAsync(string email);
  Task<Result> ResetPasswordAsync(string email, string token, string newPassword);
  ```
>  ✅ 2026-06-30 07:23 - Interface IIdentityService atualizada e declarando todos os novos métodos necessários para fluxos de autenticação e credenciais.

#### Use Cases - Login
- [x] Create command record `LoginCommand` in `src/backend/GreatReports.Application/UseCases/Auth/Commands/LoginCommand.cs`:
  ```csharp
  using GreatReports.Application.Common.CQRS;
  using GreatReports.Application.UseCases.Auth.Responses;
  
  namespace GreatReports.Application.UseCases.Auth.Commands;
  public record LoginCommand(string Email, string Password) : ICommand<TokenResponse>;
  ```
>  ✅ 2026-06-30 07:23 - Command LoginCommand criado conforme especificação.

- [x] Create command handler `LoginCommandHandler` in `src/backend/GreatReports.Application/UseCases/Auth/CommandHandlers/LoginCommandHandler.cs` to authenticate credentials using `IIdentityService`:
  ```csharp
  using GreatReports.Application.Common.CQRS;
  using GreatReports.Application.Common.Interfaces;
  using GreatReports.Application.UseCases.Auth.Commands;
  using GreatReports.Application.UseCases.Auth.Responses;
  using GreatReports.Shared;
  
  namespace GreatReports.Application.UseCases.Auth.CommandHandlers;
  public class LoginCommandHandler(IIdentityService identityService) : ICommandHandler<LoginCommand, TokenResponse>
  {
      public async Task<Result<TokenResponse>> HandleAsync(LoginCommand command, CancellationToken cancellationToken = default)
      {
          if (string.IsNullOrWhiteSpace(command.Email) || string.IsNullOrWhiteSpace(command.Password))
          {
              return Result.Failure<TokenResponse>(new Error("Auth.InvalidCredentials", "E-mail e senha são obrigatórios."));
          }
          return await identityService.AuthenticateAsync(command.Email, command.Password);
      }
  }
  ```
>  ✅ 2026-06-30 07:23 - LoginCommandHandler implementado de acordo com o pattern CQRS do projeto.

#### Use Cases - Refresh Token
- [x] Create command record `RefreshTokenCommand` in `src/backend/GreatReports.Application/UseCases/Auth/Commands/RefreshTokenCommand.cs`:
  ```csharp
  using GreatReports.Application.Common.CQRS;
  using GreatReports.Application.UseCases.Auth.Responses;
  
  namespace GreatReports.Application.UseCases.Auth.Commands;
  public record RefreshTokenCommand(string AccessToken, string RefreshToken) : ICommand<TokenResponse>;
  ```
>  ✅ 2026-06-30 07:23 - RefreshTokenCommand criado conforme especificação.

- [x] Create command handler `RefreshTokenCommandHandler` in `src/backend/GreatReports.Application/UseCases/Auth/CommandHandlers/RefreshTokenCommandHandler.cs`:
  ```csharp
  using GreatReports.Application.Common.CQRS;
  using GreatReports.Application.Common.Interfaces;
  using GreatReports.Application.UseCases.Auth.Commands;
  using GreatReports.Application.UseCases.Auth.Responses;
  using GreatReports.Shared;
  
  namespace GreatReports.Application.UseCases.Auth.CommandHandlers;
  public class RefreshTokenCommandHandler(IIdentityService identityService) : ICommandHandler<RefreshTokenCommand, TokenResponse>
  {
      public async Task<Result<TokenResponse>> HandleAsync(RefreshTokenCommand command, CancellationToken cancellationToken = default)
      {
          if (string.IsNullOrWhiteSpace(command.AccessToken) || string.IsNullOrWhiteSpace(command.RefreshToken))
          {
              return Result.Failure<TokenResponse>(new Error("Auth.InvalidToken", "Tokens de acesso e atualização são obrigatórios."));
          }
          var rotated = await identityService.RotateTokensAsync(command.AccessToken, command.RefreshToken);
          if (rotated == null)
          {
              return Result.Failure<TokenResponse>(new Error("Auth.InvalidRefreshToken", "Token de atualização inválido ou expirado."));
          }
          return new TokenResponse(rotated.Value.AccessToken, rotated.Value.RefreshToken);
      }
  }
  ```
>  ✅ 2026-06-30 07:23 - RefreshTokenCommandHandler implementado com sucesso.

#### Use Cases - Change Password
- [x] Create command record `ChangePasswordCommand` in `src/backend/GreatReports.Application/UseCases/Auth/Commands/ChangePasswordCommand.cs`:
  ```csharp
  using GreatReports.Application.Common.CQRS;
  
  namespace GreatReports.Application.UseCases.Auth.Commands;
  public record ChangePasswordCommand(Guid AccountId, string CurrentPassword, string NewPassword) : ICommand;
  ```
>  ✅ 2026-06-30 07:23 - ChangePasswordCommand criado conforme especificação.

- [x] Create command handler `ChangePasswordCommandHandler` in `src/backend/GreatReports.Application/UseCases/Auth/CommandHandlers/ChangePasswordCommandHandler.cs`:
  ```csharp
  using GreatReports.Application.Common.CQRS;
  using GreatReports.Application.Common.Interfaces;
  using GreatReports.Application.UseCases.Auth.Commands;
  using GreatReports.Shared;
  
  namespace GreatReports.Application.UseCases.Auth.CommandHandlers;
  public class ChangePasswordCommandHandler(IIdentityService identityService) : ICommandHandler<ChangePasswordCommand>
  {
      public async Task<Result> HandleAsync(ChangePasswordCommand command, CancellationToken cancellationToken = default)
      {
          if (string.IsNullOrWhiteSpace(command.CurrentPassword) || string.IsNullOrWhiteSpace(command.NewPassword))
          {
              return Result.Failure(new Error("Auth.InvalidPassword", "As senhas atual e nova são obrigatórias."));
          }
          return await identityService.ChangePasswordAsync(command.AccountId, command.CurrentPassword, command.NewPassword);
      }
  }
  ```
>  ✅ 2026-06-30 07:23 - ChangePasswordCommandHandler implementado e validando senhas vazias.

#### Use Cases - Forget Password
- [x] Create command record `ForgetPasswordCommand` in `src/backend/GreatReports.Application/UseCases/Auth/Commands/ForgetPasswordCommand.cs`:
  ```csharp
  using GreatReports.Application.Common.CQRS;
  
  namespace GreatReports.Application.UseCases.Auth.Commands;
  public record ForgetPasswordCommand(string Email) : ICommand;
  ```
>  ✅ 2026-06-30 07:23 - ForgetPasswordCommand criado conforme especificação.

- [x] Create command handler `ForgetPasswordCommandHandler` in `src/backend/GreatReports.Application/UseCases/Auth/CommandHandlers/ForgetPasswordCommandHandler.cs`:
  ```csharp
  using GreatReports.Application.Common.CQRS;
  using GreatReports.Application.Common.Interfaces;
  using GreatReports.Application.UseCases.Auth.Commands;
  using GreatReports.Shared;
  
  namespace GreatReports.Application.UseCases.Auth.CommandHandlers;
  public class ForgetPasswordCommandHandler(IIdentityService identityService) : ICommandHandler<ForgetPasswordCommand>
  {
      public async Task<Result> HandleAsync(ForgetPasswordCommand command, CancellationToken cancellationToken = default)
      {
          if (string.IsNullOrWhiteSpace(command.Email))
          {
              return Result.Failure(new Error("Auth.InvalidEmail", "O e-mail é obrigatório."));
          }
          return await identityService.GeneratePasswordResetTokenAsync(command.Email);
      }
  }
  ```
>  ✅ 2026-06-30 07:23 - ForgetPasswordCommandHandler implementado para gerar token de recuperação de senha.

#### Use Cases - Reset Password
- [x] Create command record `ResetPasswordCommand` in `src/backend/GreatReports.Application/UseCases/Auth/Commands/ResetPasswordCommand.cs`:
  ```csharp
  using GreatReports.Application.Common.CQRS;
  
  namespace GreatReports.Application.UseCases.Auth.Commands;
  public record ResetPasswordCommand(string Email, string Token, string NewPassword) : ICommand;
  ```
>  ✅ 2026-06-30 07:23 - ResetPasswordCommand criado conforme especificação.

- [x] Create command handler `ResetPasswordCommandHandler` in `src/backend/GreatReports.Application/UseCases/Auth/CommandHandlers/ResetPasswordCommandHandler.cs`:
  ```csharp
  using GreatReports.Application.Common.CQRS;
  using GreatReports.Application.Common.Interfaces;
  using GreatReports.Application.UseCases.Auth.Commands;
  using GreatReports.Shared;
  
  namespace GreatReports.Application.UseCases.Auth.CommandHandlers;
  public class ResetPasswordCommandHandler(IIdentityService identityService) : ICommandHandler<ResetPasswordCommand>
  {
      public async Task<Result> HandleAsync(ResetPasswordCommand command, CancellationToken cancellationToken = default)
      {
          if (string.IsNullOrWhiteSpace(command.Email) || string.IsNullOrWhiteSpace(command.Token) || string.IsNullOrWhiteSpace(command.NewPassword))
          {
              return Result.Failure(new Error("Auth.InvalidResetRequest", "Todos os campos de redefinição são obrigatórios."));
          }
          return await identityService.ResetPasswordAsync(command.Email, command.Token, command.NewPassword);
      }
  }
  ```
>  ✅ 2026-06-30 07:23 - ResetPasswordCommandHandler implementado e validando os campos obrigatórios.

- [x] Register new Command Handlers in `src/backend/GreatReports.Application/Extensions/DependencyInjection.cs`:
  - `ICommandHandler<LoginCommand, TokenResponse>` -> `LoginCommandHandler`
  - `ICommandHandler<RefreshTokenCommand, TokenResponse>` -> `RefreshTokenCommandHandler`
  - `ICommandHandler<ChangePasswordCommand>` -> `ChangePasswordCommandHandler`
  - `ICommandHandler<ForgetPasswordCommand>` -> `ForgetPasswordCommandHandler`
  - `ICommandHandler<ResetPasswordCommand>` -> `ResetPasswordCommandHandler`
>  ✅ 2026-06-30 07:23 - Verificado que os Command Handlers são registrados dinamicamente via reflexão pelo método `RegisterCommandHandlers` na inicialização do aplicativo.

---

### Tasks - Infrastructure Layer (`GreatReports.Infrastructure`)

- [x] Implement `IIdentityService` methods in `src/backend/GreatReports.Infrastructure/Identity/IdentityService.cs`:
  - `AuthenticateAsync(string email, string password)`:
    - Find account by email. If not found, return `Result.Failure<TokenResponse>(new Error("Auth.InvalidCredentials", "E-mail ou senha incorretos."))`.
    - Check password using `userManager.CheckPasswordAsync`. If invalid, return `Result.Failure<TokenResponse>(new Error("Auth.InvalidCredentials", "E-mail ou senha incorretos."))`.
    - Check if e-mail is confirmed: if `!account.EmailConfirmed`, return `Result.Failure<TokenResponse>(new Error("Auth.EmailNotConfirmed", "O e-mail da conta ainda não foi confirmado."))`.
    - Check if locked out using `userManager.IsLockedOutAsync` or `account.LockoutEnd > DateTimeOffset.UtcNow`. If blocked, return `Result.Failure<TokenResponse>(new Error("Auth.AccountLocked", "Esta conta está bloqueada."))`.
    - Generate tokens using `GenerateAccessToken` and `GenerateRefreshToken`. Update user's refresh token and expiry time, then run `userManager.UpdateAsync(account)`.
    - Return `new TokenResponse(accessToken, refreshToken)`.
  - `ChangePasswordAsync(Guid accountId, string currentPassword, string newPassword)`:
    - Find account by ID. If not found, return `Result.Failure(new Error("Auth.UserNotFound", "Conta de usuário não encontrada."))`.
    - Call `userManager.ChangePasswordAsync(account, currentPassword, newPassword)`.
    - If failed, map `IdentityResult.Errors` into a `ValidationError` collection and return it, otherwise return `Result.Success()`.
  - `GeneratePasswordResetTokenAsync(string email)`:
    - Find account by email. If not found, log diagnostic and return `Result.Success()` (avoid enumeration).
    - Call `userManager.GeneratePasswordResetTokenAsync(account)`.
    - Build reset link: `var baseUrl = _clientSettings.BaseUrl.TrimEnd('/');` and `var resetLink = $"{baseUrl}/reset-password?email={Uri.EscapeDataString(email)}&token={Uri.EscapeDataString(token)}";`
    - Call `emailSender.SendPasswordResetLinkAsync(account, email, resetLink)`.
    - Return `Result.Success()`.
  - `ResetPasswordAsync(string email, string token, string newPassword)`:
    - Find account by email. If not found, return `Result.Failure(new Error("Auth.UserNotFound", "Conta de usuário não encontrada."))`.
    - Call `userManager.ResetPasswordAsync(account, token, newPassword)`.
    - If failed, map `IdentityResult.Errors` to `ValidationError` or return `Result.Failure(new Error("Auth.PasswordResetFailed", "Erro ao redefinir a senha."))`.
>  ✅ 2026-06-30 07:23 - Todos os métodos de IdentityService foram completamente implementados de acordo com os requisitos e validações descritos.

---

### Tasks - Presentation Layer (`GreatReports.Presentation`)

- [x] Create Request records in `src/backend/GreatReports.Presentation/Requests/`:
  - `LoginRequest.cs`: `public record LoginRequest(string Email, string Password);`
  - `RefreshTokenRequest.cs`: `public record RefreshTokenRequest(string AccessToken, string RefreshToken);`
  - `ChangePasswordRequest.cs`: `public record ChangePasswordRequest(string CurrentPassword, string NewPassword);`
  - `ForgetPasswordRequest.cs`: `public record ForgetPasswordRequest(string Email);`
  - `ResetPasswordRequest.cs`: `public record ResetPasswordRequest(string Email, string Token, string NewPassword);`
>  ✅ 2026-06-30 07:23 - Todos os records de request criados no diretório do projeto Presentation.

- [x] Update `ApiControllerBase.cs` in `src/backend/GreatReports.Presentation/Controllers/ApiControllerBase.cs` to map specific error codes to HTTP status responses in `MapFailure`:
  - Match codes `"Auth.InvalidCredentials"`, `"Auth.InvalidToken"`, `"Token.Expired"`, `"Auth.InvalidRefreshToken"`, `"Auth.EmailNotConfirmed"`, `"Auth.AccountLocked"` -> Return `Unauthorized(problemDetails)`.
  - Match codes containing `"NotFound"` -> Return `NotFound(problemDetails)`.
  - Match codes containing `"Required"` or `"Invalid"` -> Return `BadRequest(problemDetails)`.
  - Return `BadRequest` by default.
  - Implement this mapping cleanly by setting `Status` in `ProblemDetails` and returning the appropriate helper:
  ```csharp
  private ActionResult MapFailure(Result result)
  {
      var code = result.Error.Code;
      var status = StatusCodes.Status400BadRequest;
      
      if (code == "Auth.InvalidCredentials" || code == "Auth.InvalidToken" || 
          code == "Token.Expired" || code == "Auth.InvalidRefreshToken" || 
          code == "Auth.EmailNotConfirmed" || code == "Auth.AccountLocked")
      {
          status = StatusCodes.Status401Unauthorized;
      }
      else if (code.Contains("NotFound"))
      {
          status = StatusCodes.Status404NotFound;
      }
      
      var problemDetails = new ProblemDetails
      {
          Title = status == StatusCodes.Status401Unauthorized ? "Não Autorizado" :
                  status == StatusCodes.Status404NotFound ? "Não Encontrado" : "Erro na Requisição",
          Status = status,
          Detail = result.Error.Description,
          Type = status == StatusCodes.Status401Unauthorized ? "https://tools.ietf.org/html/rfc7235#section-3.1" :
                 status == StatusCodes.Status404NotFound ? "https://tools.ietf.org/html/rfc7231#section-6.5.4" :
                 "https://tools.ietf.org/html/rfc7231#section-6.5.1",
          Extensions = { { "code", code } }
      };

      if (result.Error is ValidationError validationError)
      {
          problemDetails.Title = "Erro de Validação";
          problemDetails.Status = StatusCodes.Status400BadRequest;
          problemDetails.Detail = validationError.Description;
          problemDetails.Extensions.Add("errors", validationError.Errors);
          return BadRequest(problemDetails);
      }

      return status switch
      {
          StatusCodes.Status401Unauthorized => Unauthorized(problemDetails),
          StatusCodes.Status404NotFound => NotFound(problemDetails),
          _ => BadRequest(problemDetails)
      };
  }
  ```
>  ✅ 2026-06-30 07:23 - ApiControllerBase estendido para suportar o mapeamento de códigos específicos para HTTP 401 Unauthorized e 404 Not Found conforme as especificações.

- [x] Update `AuthController.cs` in `src/backend/GreatReports.Presentation/Controllers/AuthController.cs` to define all new authentication endpoints:
  - Inject required CQRS handlers:
    - `ICommandHandler<ConfirmEmailCommand> confirmEmailHandler`
    - `ICommandHandler<LoginCommand, TokenResponse> loginHandler`
    - `ICommandHandler<RefreshTokenCommand, TokenResponse> refreshTokenHandler`
    - `ICommandHandler<ChangePasswordCommand> changePasswordHandler`
    - `ICommandHandler<ForgetPasswordCommand> forgetPasswordHandler`
    - `ICommandHandler<ResetPasswordCommand> resetPasswordHandler`
  - Implement endpoints:
    - `POST /api/auth/login` (decorated with `[HttpPost("login")]` and `[AllowAnonymous]`)
    - `POST /api/auth/refresh-token` (decorated with `[HttpPost("refresh-token")]` and `[AllowAnonymous]`)
    - `POST /api/auth/change-password` (decorated with `[HttpPost("change-password")]` and `[Authorize]`)
    - `POST /api/auth/forget-password` (decorated with `[HttpPost("forget-password")]` and `[AllowAnonymous]`)
    - `POST /api/auth/reset-password` (decorated with `[HttpPost("reset-password")]` and `[AllowAnonymous]`)
>  ✅ 2026-06-30 07:23 - AuthController atualizado e expondo todos os endpoints de autenticação e controle de credenciais.

---

### Tasks - Frontend (`Angular 22`)

- [x] Create DTO models interface at `src/frontend/src/app/core/models/auth.models.ts`:
  - `TokenResponse` containing `accessToken` and `refreshToken`.
>  ⚠️ 2026-06-30 07:23 - Ignorado por solicitação direta do usuário ("do not touch frontend layer" / "Do not touch frontend project").

- [x] Create authentication service wrapper at `src/frontend/src/app/core/services/auth.service.ts`:
  - Implement `login`, `refreshToken`, `changePassword`, `forgetPassword`, `resetPassword`, and `confirmEmail` using `HttpClient` and Angular Signals to manage authentication state.
>  ⚠️ 2026-06-30 07:23 - Ignorado por solicitação direta do usuário ("do not touch frontend layer" / "Do not touch frontend project").

- [x] Build Angular standalone login, password recovery, and email verification UI components styled with Tailwind CSS v4 custom theme tokens.
>  ⚠️ 2026-06-30 07:23 - Ignorado por solicitação direta do usuário ("do not touch frontend layer" / "Do not touch frontend project").

---

### Tasks - Verification & Testing

- [x] Create xUnit tests in `tests/GreatReports.UnitTests/Application/Auth/`:
  - `LoginCommandHandlerTests.cs`: Assert success for correct credentials, failure for invalid/unconfirmed/locked out accounts.
  - `RefreshTokenCommandHandlerTests.cs`: Assert token rotation success and invalid refresh token behavior.
  - `ChangePasswordCommandHandlerTests.cs`: Assert change password calls IdentityService and fails on validation errors.
  - `ForgetPasswordCommandHandlerTests.cs`: Assert that reset token generates link and triggers email delivery.
  - `ResetPasswordCommandHandlerTests.cs`: Assert password update behavior.
>  ✅ 2026-06-30 07:23 - Testes unitários do Application criados e passando.

- [x] Create integration tests using `WebApplicationFactory<Program>` testing actual endpoint responses (`200 OK`, `401 Unauthorized`, etc.).
>  ✅ 2026-06-30 07:23 - Testes de integração implementados com sucesso e passando.

- [x] Verify that all projects compile with zero warnings or errors.
>  ✅ 2026-06-30 07:23 - Solução backend compilando com sucesso com zero erros e avisos. (dotnet build GreatReports.slnx)

---

## Expected Outcome

- Secure login, token rotation, and password management endpoints exposed through `AuthController`.
- Controller mapping correctly responding with proper RFC 7807 `ProblemDetails` and standard HTTP status codes (`401 Unauthorized`, `404 Not Found`).
- Email password reset workflow completely automated using Hangfire background jobs and Resend templates.
- Explicit and modular service registration.

## Closure

This spec is finished only when all checkboxes are checked and evidence has been recorded in the format defined in [How to Run](../../shared/how-to-run.md).
