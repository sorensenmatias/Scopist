# Scopist

Make resolving scoped services safe, simple, and explicit.

Scopist gives you a tiny abstraction `IScopedResolver<T>` so you can resolve a scoped service from an `IServiceScope` in places where constructor-injection isn’t available (background jobs, timers, event handlers, etc.).

It also ships with a startup validator (ScopistChecker) that ensures every `IScopedResolver<T>` you use targets a service that is actually registered as Scoped — preventing subtle lifetime bugs at runtime.

Why you’ll love it:
- Minimal API surface, zero magic.
- Clear intent: “I need T from this scope.”
- Fast feedback: validation at startup or on-demand.
- Works with the standard Microsoft.Extensions.DependencyInjection stack.

---

## Install

```bash
dotnet add package Scopist
```

---

## TL;DR

1) Register Scopist:

```csharp
var services = new ServiceCollection()
    .AddScopist(); // includes startup validation

services.AddScoped<MyScopedService>();
services.AddSingleton<MyWorker>();
```

2) Inject `IScopedResolver<T>` where you need it, and resolve within a scope:

```csharp
public sealed class MyWorker(IScopedResolver<MyScopedService> scoped, IServiceScopeFactory scopes)
{
    public async Task RunAsync()
    {
        using var scope = scopes.CreateScope();
        var s = scoped.Resolve(scope);
        await s.DoWorkAsync();
    }
}
```

That’s it. If you mistakenly register `MyScopedService` as Singleton/Transient, Scopist will fail fast with a clear error.

---

## The problem Scopist solves

In real apps, not everything can be constructor-injected. Background tasks, queues, timers, and some event-driven flows need to create a scope and resolve services later.

Common pitfalls without Scopist:
- Grabbing `IServiceProvider` everywhere and calling `GetRequiredService<T>()` (hard to track, easy to misuse).
- Accidentally resolving a scoped dependency from a root provider (lifetime bugs, memory leaks, data corruption).

Scopist fixes this by:
- Making scope usage explicit: `IScopedResolver<T>.Resolve(IServiceScope scope)`.
- Validating that every `T` you resolve is actually registered as Scoped.

---

## Quick start

### 1) Register

```csharp
var services = new ServiceCollection()
    .AddScopist(); // Registers IScopedResolver<T> and the startup checker

services.AddScoped<MyScopedService>();
services.AddSingleton<MyBackgroundJob>();

var provider = services.BuildAndValidateServiceProvider();
```

### 2) Use it safely

```csharp
public sealed class MyBackgroundJob(IScopedResolver<MyScopedService> scoped, IServiceScopeFactory scopes)
{
    public int Execute()
    {
        using var scope = scopes.CreateScope();
        var svc = scoped.Resolve(scope);
        return svc.GetValue();
    }
}

public sealed class MyScopedService
{
    public int GetValue() => 42;
}
```

---

## Validation (ScopistChecker)

Scopist validates two ways:

- Startup validation (default): when you call `services.AddScopist()`, Scopist wires a checker that runs on provider activation. If an `IScopedResolver<T>` is used but `T` isn’t registered as Scoped, the app fails fast with a detailed message.

- Manual validation (on-demand): after building the provider, you can call `provider.ValidateScopist()` to run the same checks when it suits you (useful for tests or custom bootstraps).

### Example: Startup validation (recommended)

```csharp
var services = new ServiceCollection()
    .AddScopist();

// OOPS: Wrong lifetime below — this will fail validation
services.AddSingleton<MyScopedService>();

var provider = services.BuildAndValidateServiceProvider();
// Throws ScopistValidationException: "Service MyScopedService must be registered as Scoped..."
```

### Example: Manual validation

```csharp
var services = new ServiceCollection()
    .AddScopist();

services.AddSingleton<MyScopedService>();

var provider = services.BuildServiceProvider();
provider.ValidateScopist(); // Throws ScopistValidationException if misconfigured
```

Error type: `ScopistValidationException` (inherits `InvalidOperationException`) with all messages aggregated.

---

## Design notes

- `IScopedResolver<T>` is intentionally tiny. It only resolves within a provided `IServiceScope`.
- The checker reflects over your registered services, inspects constructors for `IScopedResolver<T>`, and verifies `T` has a Scoped registration.
- No runtime proxies; no container replacement. Works with `Microsoft.Extensions.DependencyInjection`.

---

## FAQ

**Can I still use constructor injection?** Yes — Scopist is for the cases where you can’t (or shouldn’t) inject a scoped service directly.

**What if I need multiple scoped services?** Inject multiple `IScopedResolver<T>`s and resolve each within the same `IServiceScope`.

**Does it create instances eagerly?** No. Resolution happens only when you call `Resolve(scope)`.

**ASP.NET Core compatible?** Yes. Use in hosted services, background jobs, minimal APIs, etc.

---

## License

MIT
