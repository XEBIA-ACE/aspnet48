using Microsoft.AspNetCore.Builder;

// b41c16c1-0016-5b8d-8f70-29e608d4c11f / FR-S1-008: minimal hosting entry point required by the Web SDK.
// Assumption: the mapping of Global.asax.cs / App_Start (WebApiConfig, UnityConfig, SwaggerConfig) to
// ASP.NET Core hosting, DI, routing, serialization and Swagger is an open question in plan.md and is left
// to later stories; no services, middleware or endpoints are registered here.
var builder = WebApplication.CreateBuilder(args);

var app = builder.Build();

app.Run();

public partial class Program { }
