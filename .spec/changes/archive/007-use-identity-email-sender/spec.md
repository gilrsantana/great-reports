# 007-use-identity-email-sender

## Objective

Remove `EmailVerificationService` and its tests, refactoring the verification email dispatch to occur atomically during Identity account creation in `IdentityService.CreateUserAsync` using `IdentityEmailSender` (via `IEmailSender<Account>`). In addition, update `ConfirmEmailCommandHandler` to confirm the Identity account of client contacts.

## Technical Context

By moving the email confirmation dispatch into `IdentityService.CreateUserAsync`, we make the registration flow simpler, reduce database round-trips in command handlers, and completely eliminate the redundant `EmailVerificationService`.

> [!IMPORTANT]
> - Delete `IEmailVerificationService.cs`, `EmailVerificationService.cs` and `EmailVerificationServiceTests.cs`.
> - Add `string verificationToken` parameter to `IIdentityService.CreateUserAsync`.
> - Inside `IdentityService.CreateUserAsync`, construct the verification link using the token and delegate sending to `IEmailSender<Account>.SendConfirmationLinkAsync`.
> - Update `RegisterUserCommandHandler` and `AddClientContactCommandHandler` to generate the verification token before calling `CreateUserAsync` and pass it as an argument.
> - Ensure Identity account email confirmation is also triggered for `ClientContact` instances during email confirmation.

## Project References

- [Product Definition](../../memory/product.md)
- [Global Technical Context](../../memory/technical-context.md)
- [Repository Structure](../../memory/structure.md)

## Shared References

- [How to Run](../../shared/how-to-run.md)
- [Naming Conventions](../../shared/naming-conventions.md)

---

## Tasks

### Tasks - Application Layer (`GreatReports.Application`)
- [x] Delete interface `IEmailVerificationService.cs` in `src/backend/GreatReports.Application/Common/Interfaces/IEmailVerificationService.cs`.
>  ✅ 2026-06-29 02:43 - Deleted the IEmailVerificationService interface file.
- [x] Modify `IIdentityService.cs` in `src/backend/GreatReports.Application/Common/Interfaces/IIdentityService.cs` to add `string verificationToken` parameter to `CreateUserAsync`.
>  ✅ 2026-06-29 02:43 - Added the verificationToken parameter to the interface.
- [x] Refactor `RegisterUserCommandHandler` in `src/backend/GreatReports.Application/UseCases/Users/Commands/RegisterUserCommand.cs`:
  - Remove injection of `IEmailVerificationService`.
  - Generate the token (`user.GenerateVerificationToken()`) before calling `CreateUserAsync`.
  - Pass the token to `CreateUserAsync`.
>  ✅ 2026-06-29 02:43 - Updated handler to perform atomic token generation and dispatch.
- [x] Refactor `AddClientContactCommandHandler` in `src/backend/GreatReports.Application/UseCases/ClientContacts/Commands/AddClientContactCommand.cs`:
  - Remove injection of `IEmailVerificationService`.
  - Generate the token (`contact.GenerateVerificationToken()`) before calling `CreateUserAsync`.
  - Pass the token to `CreateUserAsync`.
>  ✅ 2026-06-29 02:43 - Updated handler to perform atomic token generation and dispatch.
- [x] Refactor `ConfirmEmailCommandHandler` in `src/backend/GreatReports.Application/UseCases/Auth/Commands/ConfirmEmailCommand.cs` to call `await _identityService.ConfirmEmailAsync(contact.Id);` upon successful token verification of a client contact.
>  ✅ 2026-06-29 02:43 - Added Identity account confirmation for client contacts.

### Tasks - Infrastructure Layer (`GreatReports.Infrastructure`)
- [x] Delete `EmailVerificationService.cs` in `src/backend/GreatReports.Infrastructure/Mailer/EmailVerificationService.cs`.
>  ✅ 2026-06-29 02:43 - Deleted the EmailVerificationService implementation file.
- [x] Modify `IdentityService.cs` in `src/backend/GreatReports.Infrastructure/Identity/IdentityService.cs`:
  - Add `string verificationToken` parameter to `CreateUserAsync`.
  - Inject `IEmailSender<Account>`.
  - Inside `CreateUserAsync`, after creating the user and assigning roles, build the link `http://localhost:4200/confirm-email?email={email}&token={verificationToken}` and invoke `_emailSender.SendConfirmationLinkAsync(account, email, link)`.
>  ✅ 2026-06-29 02:44 - Added injected IEmailSender<Account> and updated CreateUserAsync implementation.
- [x] Remove `EmailVerificationService` registration from `DependencyInjection.cs` in `src/backend/GreatReports.Infrastructure/Extensions/DependencyInjection.cs`.
>  ✅ 2026-06-29 02:44 - Removed DI registration for IEmailVerificationService.

### Tasks - Verification & Testing
- [x] Delete `EmailVerificationServiceTests.cs` in `tests/GreatReports.UnitTests/Infrastructure/EmailVerificationServiceTests.cs`.
>  ✅ 2026-06-29 02:44 - Deleted the EmailVerificationServiceTests file.
- [x] Update unit tests in `tests/GreatReports.UnitTests/Application/RegisterUserCommandHandlerTests.cs` to match the new `CreateUserAsync` signature and verify token passing.
>  ✅ 2026-06-29 02:44 - Updated unit tests for RegisterUserCommandHandler.
- [x] Update unit tests in `tests/GreatReports.UnitTests/Application/AddClientContactCommandHandlerTests.cs` to match the new `CreateUserAsync` signature and verify token passing.
>  ✅ 2026-06-29 02:44 - Updated unit tests for AddClientContactCommandHandler.
- [x] Update unit tests in `tests/GreatReports.UnitTests/Application/ConfirmEmailCommandHandlerTests.cs` to verify `ConfirmEmailAsync` is called for both users and client contacts.
>  ✅ 2026-06-29 02:45 - Updated unit tests for ConfirmEmailCommandHandler.
- [x] Verify that all unit tests build and pass successfully.
>  ✅ 2026-06-29 02:45 - Ran and verified all 139 unit tests pass successfully.

---

## Expected Outcome

- Clean repository layout with redundant interface and service files removed.
- Atomic account creation and verification dispatch.
- Complete warning-free solution compilation.

## Closure

This spec is finished only when all checkboxes are checked and evidence has been recorded in the format defined in [How to Run](../../shared/how-to-run.md).
