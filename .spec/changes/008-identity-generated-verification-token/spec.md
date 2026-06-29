# 008-identity-generated-verification-token

## Objective

Remove manual verification token generation from domain entities, remove `EmailConfirmed` and `VerificationToken` properties from the `ClientContact` entity, and utilize ASP.NET Core Identity's built-in, cryptographically signed email confirmation token generation and validation.

## Technical Context

Using ASP.NET Core Identity's built-in `GenerateEmailConfirmationTokenAsync` and `ConfirmEmailAsync` is much more secure than storing plain-text GUIDs in the domain database. By removing authentication-related properties (`EmailConfirmed` and `VerificationToken`) from `ClientContact`, we prevent duplicate state and delegate confirmation state 100% to ASP.NET Core Identity.

> [!IMPORTANT]
> - Delete `VerificationToken` property and `GenerateVerificationToken()` method from the `User` domain entity. Keep `EmailConfirmed` in `User`.
> - Delete both `EmailConfirmed` and `VerificationToken` properties, as well as `ConfirmEmail()` and `GenerateVerificationToken()` methods, from the `ClientContact` domain entity.
> - Generate the confirmation token cryptographically inside `IdentityService.CreateUserAsync` using `UserManager<Account>.GenerateEmailConfirmationTokenAsync`.
> - URL-encode the generated token in the confirmation link.
> - Verify the token in `IdentityService.ConfirmEmailAsync(Guid accountId, string token)` using `UserManager<Account>.ConfirmEmailAsync`.
> - Create and apply an EF Core migration to drop the `VerificationToken` column from the `Users` table, and the `VerificationToken` and `EmailConfirmed` columns from the `ClientContacts` table.

## Project References

- [Product Definition](../../memory/product.md)
- [Global Technical Context](../../memory/technical-context.md)
- [Repository Structure](../../memory/structure.md)

## Shared References

- [How to Run](../../shared/how-to-run.md)
- [Naming Conventions](../../shared/naming-conventions.md)

---

## Tasks

### Tasks - Domain Layer (`GreatReports.Domain`)
- [x] Remove `VerificationToken` property and `GenerateVerificationToken()` from `User.cs` in `src/backend/GreatReports.Domain/Entities/User.cs`. Update `ConfirmEmail()` to not nullify `VerificationToken`.
>  ✅ 2026-06-29 03:08 - Removed VerificationToken from User.
- [x] Remove `EmailConfirmed` and `VerificationToken` properties, and methods `GenerateVerificationToken()` and `ConfirmEmail()`, from `ClientContact.cs` in `src/backend/GreatReports.Domain/Entities/ClientContact.cs`.
>  ✅ 2026-06-29 03:08 - Removed EmailConfirmed and VerificationToken from ClientContact.

### Tasks - Application Layer (`GreatReports.Application`)
- [x] Update `IIdentityService.cs` in `src/backend/GreatReports.Application/Common/Interfaces/IIdentityService.cs`:
  - `Task<bool> CreateUserAsync(Guid id, string email, string password, IEnumerable<string> roles);` (remove token parameter).
  - `Task<bool> ConfirmEmailAsync(Guid accountId, string token);` (add token parameter).
>  ✅ 2026-06-29 03:08 - Updated IIdentityService signatures.
- [x] Update `RegisterUserCommandHandler` in `src/backend/GreatReports.Application/UseCases/Users/Commands/RegisterUserCommand.cs` to remove entity token generation and update invocation to `CreateUserAsync`.
>  ✅ 2026-06-29 03:09 - Updated RegisterUserCommandHandler.
- [x] Update `AddClientContactCommandHandler` in `src/backend/GreatReports.Application/UseCases/ClientContacts/Commands/AddClientContactCommand.cs` to remove entity token generation and update invocation to `CreateUserAsync`.
>  ✅ 2026-06-29 03:09 - Updated AddClientContactCommandHandler.
- [x] Update `ConfirmEmailCommandHandler` in `src/backend/GreatReports.Application/UseCases/Auth/Commands/ConfirmEmailCommand.cs`:
  - Verify tokens by calling `IIdentityService.ConfirmEmailAsync(id, token)`.
  - For client contacts, do not update the domain entity since confirmation state is managed entirely by Identity.
>  ✅ 2026-06-29 03:09 - Refactored ConfirmEmailCommandHandler to verify tokens directly from Identity.

### Tasks - Infrastructure Layer (`GreatReports.Infrastructure`)
- [x] Update `IdentityService.cs` in `src/backend/GreatReports.Infrastructure/Identity/IdentityService.cs`:
  - Update `CreateUserAsync` signature, generate the token using `_userManager.GenerateEmailConfirmationTokenAsync(account)`, URL-encode it, build the verification link, and send the email.
  - Update `ConfirmEmailAsync` to accept `token` and call `_userManager.ConfirmEmailAsync(account, token)`.
>  ✅ 2026-06-29 03:08 - Implemented IdentityService token generation and validation.
- [x] Remove `VerificationToken` mapping in `UserConfiguration.cs`.
>  ✅ 2026-06-29 03:10 - Removed mapping in UserConfiguration.
- [x] Remove `VerificationToken` mapping in `ClientContactConfiguration.cs`.
>  ✅ 2026-06-29 03:10 - Removed mapping in ClientContactConfiguration.
- [x] Create and apply an EF Core migration to drop `VerificationToken` from `Users`, and `VerificationToken` and `EmailConfirmed` from `ClientContacts`.
>  ✅ 2026-06-29 03:12 - Applied DB migration `20260629061224_RemoveVerificationTokenFromEntities`.

### Tasks - Verification & Testing
- [x] Update unit tests in `tests/GreatReports.UnitTests/Application/RegisterUserCommandHandlerTests.cs` to match the updated `CreateUserAsync` signature.
>  ✅ 2026-06-29 03:13 - Updated RegisterUserCommandHandlerTests.
- [x] Update unit tests in `tests/GreatReports.UnitTests/Application/AddClientContactCommandHandlerTests.cs` to match the updated `CreateUserAsync` signature.
>  ✅ 2026-06-29 03:13 - Updated AddClientContactCommandHandlerTests.
- [x] Update unit tests in `tests/GreatReports.UnitTests/Application/ConfirmEmailCommandHandlerTests.cs` to match the new `ConfirmEmailAsync` signature and remove contact update assertions where appropriate.
>  ✅ 2026-06-29 03:13 - Updated ConfirmEmailCommandHandlerTests.
- [x] Verify that all unit tests build and pass successfully.
>  ✅ 2026-06-29 03:14 - All 135 unit tests pass successfully.

---

## Expected Outcome

- Clean, decoupled domain model where Identity manages all credentials and confirmation state.
- Cryptographically secure token generation and validation.
- Complete warning-free solution compilation.

## Closure

This spec is finished only when all checkboxes are checked and evidence has been recorded in the format defined in [How to Run](../../shared/how-to-run.md).
