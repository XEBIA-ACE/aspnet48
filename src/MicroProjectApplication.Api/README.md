# MicroProjectApplication.Api (.NET 8)

Parallel SDK-style ASP.NET Core Web API project for story `b41c16c1-0016-5b8d-8f70-29e608d4c11f`.
The legacy .NET Framework 4.8 project (`../../MicroProjectApplication.csproj`, `Global.asax.cs`, `App_Start/`) is unchanged (FR-S1-004).

```bash
dotnet build src/MicroProjectApplication.Api/MicroProjectApplication.Api.csproj
dotnet test tests/MicroProjectApplication.Api.Tests/MicroProjectApplication.Api.Tests.csproj
```

## Implementation notes and assumptions

| ID | Decision |
|----|----------|
| FR-S1-001 / FR-S1-002 / T001 / T002 | `Microsoft.NET.Sdk.Web`, `net8.0`, located at `src/MicroProjectApplication.Api` (assumption: folder layout was not specified). |
| FR-S1-003 / T003 | All dependencies via `PackageReference`; no `packages.config`. |
| FR-S1-007 / FR-S1-008 / T007 / T008 | ASP.NET Core comes from the Web SDK's implicit `Microsoft.AspNetCore.App` framework reference; no explicit `Microsoft.AspNetCore.*` packages. |
| FR-S1-009 / FR-S1-010 / T009 / T010 | `Swashbuckle.AspNetCore` 6.9.0 (net8.0-compatible). Legacy `Swashbuckle` 5.x / `Swashbuckle.Core` are not referenced. Not configured in `Program.cs` yet. |
| FR-S1-011 / FR-S1-012 / T011 / T012 | `Models`, `Data`, `Repositories`, `Services` are **linked** from the repo root and compiled unchanged (shared, single source of truth). |
| FR-S1-011 / T011 | **Assumption:** the legacy `Controllers` depend on `System.Web.Http` (`ApiController`, `IHttpActionResult`, `RoutePrefix`) and cannot compile on .NET 8 unchanged, and editing them would modify the legacy project (FR-S1-004). They are therefore **copied** into `Controllers/` with compile-time-only edits (`using`, `ApiController`→`ControllerBase`, `IHttpActionResult`→`IActionResult`, `RoutePrefix`→`Route`). Action logic is identical. |
| FR-S1-008 | **Assumption:** `Program.cs` contains only minimal hosting (`WebApplication.CreateBuilder` / `Build` / `Run`). No controllers, DI registrations, Swagger or other endpoints are wired up, so no new endpoints are exposed by this story. |
| `Nullable` / `ImplicitUsings` | Disabled so linked legacy sources compile exactly as written. |

## Open questions for follow-up stories

- Mapping of `Global.asax.cs` / `App_Start` (`WebApiConfig`, `UnityConfig`, `SwaggerConfig`) to ASP.NET Core hosting, DI, routing and Swagger.
- Long-term strategy for controllers: keep the duplicated ASP.NET Core copies, or move to a shared/multi-targeted project once the legacy project is retired.
- Model binding: the copied controllers do not use `[ApiController]` / `[FromBody]`, so body binding of `Create`/`Update` parameters differs from Web API 2 until the routing/serialization story decides on it.
- JSON serialization: legacy uses Newtonsoft.Json; ASP.NET Core defaults to System.Text.Json (camelCase).
- The solution-level build (`dotnet build MicroProjectApplication.sln`) still includes the legacy .NET Framework project, which requires Windows / Visual Studio MSBuild.
