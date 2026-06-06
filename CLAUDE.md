# CLAUDE.md

> Coding conventions for this repository. These are **rules**, not project
> context — domain, topic, and strategy belong in the agent/prompt, not here.

## Project Overview
- **Type:** ASP.NET (Web) API
- **Architecture:** Clean Architecture — `Api` → `Application` → `Domain` → `Persistence`
- **Data:** EF Core with a Unit of Work
- **Result type:** every endpoint returns `CustomJsonResult<T>`

**IMPORTANT:** Stick to the existing patterns. Do **not** introduce CQRS, MediatR,
Event Sourcing, or any new architectural pattern unless explicitly requested.

## Solution Structure
- `src/Api`         — Controllers, Middleware, Swagger only (no business logic)
- `src/Application` — Services, DTOs, Validators, business logic
- `src/Domain`      — Entities, Enums, domain rules
- `src/Persistence` — EF Core, Repositories, Unit of Work, interceptors
- `tests/`          — unit / integration tests

## Commands
```bash
dotnet build
dotnet test
dotnet run --project src/Api

# EF Core migrations — always target these projects:
dotnet ef migrations add <Name> -p src/Persistence -s src/Api
dotnet ef database update -p src/Persistence -s src/Api
```

## Architecture Rules
- Respect SOLID and DRY.
- Controllers contain **no** business logic and call services only; services hold business logic.
- Use Dependency Injection — never manually instantiate services or `DbContext`.
- Services depend on interfaces, never concrete implementations, and must stay testable.
- Services must not touch `HttpContext`.

## Controllers
- RESTful attribute routing with the correct verb: `GET` read, `POST` create, `PUT`/`PATCH` update, `DELETE` delete.
- **Every endpoint returns `CustomJsonResult<T>`** — never `IActionResult`, `ActionResult`, `Ok()`, `BadRequest()`, or `NotFound()`.
  Non-200 outcomes (e.g. created, no content, not found, validation failed) are expressed
  through `CustomJsonResult<T>` itself (status/code carried in the envelope), not via raw action results.
- Declare a `ProducesResponseType` for **every** status code an endpoint can return — success **and** errors
  (e.g. 200/201, 400, 401/403, 404) — not just the happy path.
- No `try/catch` for cross-cutting error handling — the global exception middleware handles it.
  A local `try/catch` is allowed only for a genuinely recoverable, endpoint-specific case.

## Validation
- Every **request/input** DTO has a FluentValidation validator registered in DI. No manual or inline validation.
- Response/output DTOs are not validated.

## Authorization
- Check permissions with `PermissionService` **before** any business action.
- Centralize permission logic; never duplicate it in controllers or services.

## EF Core & Data Access
- Read-only queries use `AsNoTracking()`.
- Use `AsSplitQuery()` for queries with **multiple collection `Include()`s** (to avoid Cartesian explosion).
  Do not apply it blindly to reference-only includes; ensure a stable `OrderBy` when splitting.
- Project only the columns you need; use pagination for collections (never return unbounded lists).
- Always `async/await`; avoid N+1 queries.
- Access data through `DbContext` (it already implements Unit of Work). Add a repository
  abstraction only where it adds clear value — do not force a generic repository over everything.
- **Persistence happens only through `UnitOfWork`** — services never call `DbContext.SaveChanges()`.
  (A separate `UnitOfWork` is justified only by real coordination needs; otherwise the `DbContext` is the UoW.)
- Use transactions only when multiple writes must succeed or fail together; avoid unnecessary scopes.

## Async & Nullability
- All I/O-bound methods are `async` and named `...Async`.
- Public async methods accept and honor a `CancellationToken`.
- **Never** use `.Result` or `.Wait()` (no sync-over-async).
- Enable and respect nullable reference types; avoid `null!` and unnecessary `!`.

## Soft Delete & Audit
- No physical deletes — set the `IsDeleted` flag. (Hard delete only when law/compliance, e.g. data erasure, requires it.)
- Soft-deleted rows must be excluded automatically via an EF **global query filter**
  (`HasQueryFilter(e => !e.IsDeleted)`), not by adding `Where(!IsDeleted)` in each query.
- Audit fields (`CreatedDate`, `CreatedBy`, `ModifiedDate`, `ModifiedBy`) are populated **automatically**
  by a `SaveChangesInterceptor` (or a `SaveChangesAsync` override) — not by hand in each service.

## Mapping
- Use one consistent mapping approach project-wide.
- **Prefer** manual mapping or a source generator (**Mapster** / **Mapperly**) over AutoMapper.
  (AutoMapper v15+ is dual-licensed and requires a paid license for larger orgs; avoid new dependence on it.)
- Keep mapping in the Application layer — never in controllers.

## Naming
- DTOs end with `Dto` · Validators end with `Validator` · Services end with `Service` · Interfaces start with `I`.

## Logging
- Log unexpected exceptions using structured logging. Never log sensitive information.

## Caching
- Cache where appropriate; invalidate the cache after updates.

## Exceptions
- Services throw business exceptions; never swallow errors. A global exception middleware is assumed.

## Testing
<!-- Adjust if your framework is not xUnit -->
- xUnit, Arrange-Act-Assert. Mock dependencies via interfaces; never hit a real database in unit tests.
- Name tests `Method_Scenario_ExpectedResult`.

## Canonical Patterns
Follow these exactly.

### Endpoint — returns `CustomJsonResult<T>`, documents all outcomes
```csharp
[HttpGet("{id:guid}")]
[ProducesResponseType(typeof(CustomJsonResult<ProductDto>), StatusCodes.Status200OK)]
[ProducesResponseType(typeof(CustomJsonResult<ProductDto>), StatusCodes.Status404NotFound)]
public async Task<CustomJsonResult<ProductDto>> GetById(Guid id, CancellationToken ct)
{
    return await _productService.GetByIdAsync(id, ct);
}
```

### Permission check — before the business action
```csharp
await _permissionService.CheckAccessAsync(Permissions.ProductEdit);
```

### Persistence — through UnitOfWork
```csharp
_dbContext.Products.Add(entity);
await _unitOfWork.SaveChangesAsync(ct);
```

### Soft delete + global query filter
```csharp
// In the entity configuration (Persistence):
builder.HasQueryFilter(e => !e.IsDeleted);

// In the service:
entity.IsDeleted = true;
await _unitOfWork.SaveChangesAsync(ct);
```

### Audit fields — set automatically in an interceptor
```csharp
// Persistence/Interceptors/AuditInterceptor.cs
public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
    DbContextEventData eventData, InterceptionResult<int> result, CancellationToken ct = default)
{
    foreach (var entry in eventData.Context!.ChangeTracker.Entries<IAuditable>())
    {
        if (entry.State == EntityState.Added)
        {
            entry.Entity.CreatedDate = DateTime.UtcNow;
            entry.Entity.CreatedBy = _currentUser.Id;
        }
        else if (entry.State == EntityState.Modified)
        {
            entry.Entity.ModifiedDate = DateTime.UtcNow;
            entry.Entity.ModifiedBy = _currentUser.Id;
        }
    }
    return base.SavingChangesAsync(eventData, result, ct);
}
```

## Mandatory Self-Review
After generating code, run the self-review in `.claude/skills/code-review.md`
and include the Rule Compliance Report it defines. If review finds a violation,
refactor and rerun until none remain. **Never deliver code with known violations.**
