# Implementation Plan: b41c16c1-0016-5b8d-8f70-29e608d4c11f

## Technical Context
- unresolved reference (component:Global.asax.cs)
- unresolved reference (component:WebApi)
- Codebase grounding snapshot (effba6df9ca5a26d)
- Accepted impact analysis (sha256:7f5d581671b1042a89490f8f1f973cb52284188289dbd8666877cc2eae60aea8)
- Implement only against Accepted impact analysis (revision 1) (fingerprint sha256:7f5d581671b1042a89490f8f1f973cb52284188289dbd8666877cc2eae60aea8)
- Resolve or explicitly accept the following implementation-readiness items:
  - Absence of .csproj or solution (.sln) structural data for the new .NET 8 project prevents direct verification of SDK-style configuration, target framework, PackageReference usage, and package inventory.
  - How will existing Global.asax.cs-based startup in WebApi (43476b405d51efb4) map into ASP.NET Core Program.cs/Startup.cs hosting patterns?
  - Impact analysis is limited to the WebApi (43476b405d51efb4) class and the provided inventory; no additional callers, routes, or project files were available in code-insights.
  - No build logs or compilation metadata are provided to confirm successful dotnet build behavior for the new project.
  - No routing/controller or hosting symbols for the new ASP.NET Core application are present to assess readiness for later migration steps.
  - No-match classification (all acceptance criteria unmapped): The requirement is to establish a new .NET 8 SDK-style ASP.NET Core Web API project for MicroProjectApplication, which by definition would not already exist in a legacy codebase tied to classic .NET Framework and System.Web. The empty results for terms like MicroProjectApplication, net8.0, WebApi, and Swashbuckle.AspNetCore are therefore consistent with adding a new project rather than missing existing code.
  - Requirement traceability incomplete for S1-AC1, S1-AC2, S1-AC3, S1-AC4, S1-AC5
  - What is the exact .csproj configuration for the planned .NET 8 SDK-style Web API project (SDK attribute, TargetFramework, PackageReference items)?
  - Will the new .NET 8 project initially duplicate controllers and services or reference them via multi-targeting/shared projects?

## Contract Changes
- No database schemas, ORM models, or data tables are referenced by the WebApi (43476b405d51efb4) class in the measured facts. 

## Security and Quality
- No authentication/authorization or security-sensitive flows were detected for WebApi (43476b405d51efb4) in the structural facts; security impact appears limited for this change unit based on available data.
- Preserve the availability, reliability, and observability constraints grounded by effba6df9ca5a26d

## Verification Plan
- **S1-AC1**: Given the solution currently consists of a classic ASP.NET Web API project with Global.asax.cs and App_Start, when a new project is added for the migration, then that new project is an SDK-style .NET 8 Web API project (using the Web SDK, targeting .NET 8, using PackageReference) and the original .NET Framework project remains unchanged for now.
- **S1-AC2**: Given the new .NET 8 Web API project is configured in the solution, when `dotnet build` is run for that project, then the build succeeds without errors and without referencing System.Web or other .NET Framework-only assemblies.
- **S1-AC3**: Given the modernization objective to use ASP.NET Core, when the new project file is inspected, then it includes the necessary ASP.NET Core Web API framework references (via the appropriate SDK/FrameworkReference or packages) required to compile controllers, routing, and minimal hosting code in later stories.
- **S1-AC4**: Given the need to maintain Swagger documentation in the migrated API, when the new project file is inspected, then it includes a Swashbuckle.AspNetCore package reference suitable for enabling OpenAPI generation and Swagger UI configuration in later stories.
- **S1-AC5**: Given the existing source layout with Controllers, Services, Repositories, Data, and Models directories, when those code files are added or linked into the new .NET 8 project without yet changing their behavior, then the project still builds successfully (allowing for straightforward follow-on work in hosting, DI, routing, and serialization).
- **IMPACT**: Given zero measured callers and transactions, regression risk from changing or deprecating WebApi (43476b405d51efb4) is structurally low, but creation of the new .NET 8 project will require new build and smoke tests that are outside the current symbol-level insights.