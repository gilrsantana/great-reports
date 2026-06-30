# 006-register-client-contact-account

## Objective

Update the ClientContact addition workflow to atomically register an ASP.NET Core Identity authentication account with the `"Stakeholder"` role for the contact, allowing client contacts to log in.

## Technical Context

When a manager adds a client contact, the system must create a corresponding `Account` with the role `"Stakeholder"` in Identity. If the account creation fails, a database rollback deletes the created `ClientContact` entity.

> [!IMPORTANT]
> - Ensure atomic registration: if Identity account creation fails, the corresponding `ClientContact` domain entity must be removed from the database and the changes saved.
> - Follow localization rules (RULE-014) for any error/outcome messages (use Portuguese-BR).
> - Duplicate emails must be checked and rejected with appropriate error codes before creating the entities.

## Project References

- [Product Definition](../../../memory/product.md)
- [Global Technical Context](../../../memory/technical-context.md)
- [Repository Structure](../../../memory/structure.md)

## Shared References

- [How to Run](../../../shared/how-to-run.md)
- [Naming Conventions](../../../shared/naming-conventions.md)

---

## Tasks

### Tasks - Application Layer (`GreatReports.Application`)
- [x] Add `void Delete(ClientContact contact);` declaration to interface `IClientContactRepository` in `src/backend/GreatReports.Application/Common/Interfaces/IClientContactRepository.cs`.
>  ✅ 2026-06-29 02:27 - Added Delete method to the client contact repository interface.
- [x] Refactor `AddClientContactCommandHandler` in `src/backend/GreatReports.Application/UseCases/ClientContacts/Commands/AddClientContactCommand.cs`:
  - Inject `IIdentityService`.
  - Check if a client contact with the same email already exists (using `GetByEmailAsync` and returning error `"ClientContact.EmailAlreadyExists"` / `"Já existe um contato cadastrado com este e-mail."` if found).
  - Register Identity account using `IIdentityService.CreateUserAsync(contact.Id, command.Email, tempPassword, ["Stakeholder"])`.
  - Handle rollback if `CreateUserAsync` returns `false` (remove the added `ClientContact` and call `SaveChangesAsync`).
>  ✅ 2026-06-29 02:27 - Refactored handler implementing atomic registration and rollback.

### Tasks - Infrastructure Layer (`GreatReports.Infrastructure`)
- [x] Implement `void Delete(ClientContact contact)` in `ClientContactRepository` in `src/backend/GreatReports.Infrastructure/Persistence/Repositories/ClientContactRepository.cs` calling `DbContext.Set<ClientContact>().Remove(contact)`.
>  ✅ 2026-06-29 02:27 - Implemented EF Core deletion mapping for rollbacks.

### Tasks - Verification & Testing
- [x] Update unit tests in `tests/GreatReports.UnitTests/Application/AddClientContactCommandHandlerTests.cs`:
  - Add mock setups for `IIdentityService`.
  - Test validation rejection when a contact with the same email already exists.
  - Test registration rollback: if `IIdentityService.CreateUserAsync` fails, the `ClientContact` should be removed from the repository.
  - Test successful account registration with `"Stakeholder"` role when the command is valid.
>  ✅ 2026-06-29 02:28 - Wrote full unit test coverage verifying success, duplicate check, and rollback paths.
- [x] Verify that all 138 unit tests build and pass successfully.
>  ✅ 2026-06-29 02:28 - Ran all 140 unit tests successfully.

---

## Expected Outcome

- Client contacts are atomically registered in both the domain database and Identity Auth system as `"Stakeholder"` accounts.
- Registration errors are handled gracefully with full transactional rollback.
- Complete warning-free solution compilation.

## Closure

This spec is finished only when all checkboxes are checked and evidence has been recorded in the format defined in [How to Run](../../../shared/how-to-run.md).
