# Code Review Skill

Mandatory **self-review** to run after generating any code, before returning the
final answer. Verify the generated code against **every rule in `CLAUDE.md`**.
This checklist must stay in sync with `CLAUDE.md`; if a rule changes there,
change it here too. If any rule is violated, automatically refactor and re-run
the review before producing the final answer.

## Review Checklist

### Architecture
- [ ] SOLID principles respected.
- [ ] DRY principles respected.
- [ ] Controllers contain no business logic; services contain the business logic.
- [ ] Dependency Injection used correctly (no manual instantiation of services / `DbContext`).

### Service Layer
- [ ] Services depend on interfaces, not concrete implementations.
- [ ] Services do not access `HttpContext` directly.
- [ ] Services remain testable.

### Controllers
- [ ] RESTful attribute routing with the correct verb (`GET` read, `POST` create, `PUT`/`PATCH` update, `DELETE` delete).
- [ ] All endpoints return `CustomJsonResult<T>` — never `IActionResult` / `ActionResult` / `Ok()` / `BadRequest()` / `NotFound()`.
- [ ] Non-200 outcomes are expressed through `CustomJsonResult<T>`, not raw action results.
- [ ] A `ProducesResponseType` is declared for every status code the endpoint can return (success **and** errors).
- [ ] Controllers only call services.
- [ ] No `try/catch` for cross-cutting error handling (global middleware handles it); a local `try/catch` only for a genuinely recoverable, endpoint-specific case.

### Validation
- [ ] Every **request/input** DTO has a FluentValidation validator (response DTOs are not validated).
- [ ] No manual/inline validation exists.
- [ ] Validators are registered in DI.

### EF Core & Data Access
- [ ] Read-only queries use `AsNoTracking()`.
- [ ] Queries with **multiple collection `Include()`s** use `AsSplitQuery()` (not applied blindly to reference includes; stable `OrderBy` present when splitting).
- [ ] Projections used where appropriate (only required columns selected).
- [ ] `async/await` used for all I/O; no N+1 queries.
- [ ] Data accessed through `DbContext`; a repository abstraction added **only where it adds clear value** (no generic repository forced over everything).

### Async & Nullability
- [ ] Async method names end with `Async`.
- [ ] Public async methods accept/honor a `CancellationToken`.
- [ ] Nullable reference types respected; no needless `null!` / `!`.
- [ ] No `.Result` or `.Wait()` (no sync-over-async).

### Unit of Work
- [ ] `SaveChanges` executed only through `UnitOfWork`.
- [ ] Services do not call `DbContext.SaveChanges()` directly.

### Transactions
- [ ] Transactions used only for multi-write operations that must succeed/fail together.
- [ ] No unnecessary transaction scopes.

### Soft Delete
- [ ] No physical delete operations (use `IsDeleted`; hard delete only on a legal/compliance requirement).
- [ ] Soft-deleted rows excluded automatically via an EF **global query filter** (`HasQueryFilter`), not per-query `Where`.

### Audit Fields
- [ ] `CreatedDate`, `CreatedBy`, `ModifiedDate`, `ModifiedBy` populated **automatically** via a `SaveChangesInterceptor` (or `SaveChangesAsync` override) — not by hand in services.

### Permissions / Authorization
- [ ] `PermissionService` is used; no duplicated permission logic.
- [ ] Permission checks are centralized.
- [ ] Permissions checked before business actions.

### Mapping
- [ ] One consistent mapping approach project-wide.
- [ ] Manual mapping or a source generator (Mapster / Mapperly) preferred; no new dependence on AutoMapper.
- [ ] Mapping kept in the Application layer (never in controllers).

### Naming Conventions
- [ ] DTOs end with `Dto`.
- [ ] Validators end with `Validator`.
- [ ] Services end with `Service`.
- [ ] Interfaces start with `I`.

### Logging
- [ ] Unexpected exceptions are logged.
- [ ] No sensitive information is logged.
- [ ] Structured logging is used.

### Cache
- [ ] Cache strategy applied where appropriate.
- [ ] Cache invalidation exists after updates.

### Exception Handling
- [ ] Services throw business exceptions; errors are not swallowed.
- [ ] Global exception middleware is assumed.

### Performance
- [ ] No unnecessary database queries.
- [ ] Only required columns selected.
- [ ] N+1 query problems avoided.
- [ ] Pagination used for collections; no unbounded lists.

### Testing
- [ ] Tests use xUnit and the AAA pattern.
- [ ] Test names follow `Method_Scenario_ExpectedResult`.
- [ ] Dependencies mocked via interfaces; no real DB in unit tests.

## Final Output: Rule Compliance Report

End the response with a section titled **"Rule Compliance Report"** listing:

- ✔ Rules followed
- ⚠ Potential improvements
- ❌ Violations found and fixed

If any rule was violated, refactor automatically and re-run this review.
Repeat until no violations remain. Never deliver code with known violations.
