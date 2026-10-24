using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using MicroProjectApplication.Controllers;
using MicroProjectApplication.Data;
using MicroProjectApplication.Models;
using MicroProjectApplication.Repositories;
using MicroProjectApplication.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Xunit;

namespace MicroProjectApplication.Api.Tests
{
    // Characterization tests for story b41c16c1-0016-5b8d-8f70-29e608d4c11f (S1-AC1..S1-AC5, IMPACT).
    public class ProjectConfigurationTests
    {
        private static readonly string RepoRoot = FindRepoRoot();
        private static readonly string NewProjectPath =
            Path.Combine(RepoRoot, "src", "MicroProjectApplication.Api", "MicroProjectApplication.Api.csproj");
        private static readonly string LegacyProjectPath = Path.Combine(RepoRoot, "MicroProjectApplication.csproj");
        private static readonly Assembly ApiAssembly = typeof(Program).Assembly;

        private static string FindRepoRoot()
        {
            var dir = new DirectoryInfo(AppContext.BaseDirectory);
            while (dir != null && !File.Exists(Path.Combine(dir.FullName, "MicroProjectApplication.sln")))
                dir = dir.Parent;
            return dir?.FullName ?? throw new InvalidOperationException("MicroProjectApplication.sln not found.");
        }

        private static XElement NewProject() => XDocument.Load(NewProjectPath).Root;

        private static string[] PackageReferences() =>
            NewProject().Descendants("PackageReference").Select(e => (string)e.Attribute("Include")).ToArray();

        [Fact]
        public void S1_AC1_NewProject_IsSdkStyleWebSdkProject()
        {
            Assert.Equal("Microsoft.NET.Sdk.Web", (string)NewProject().Attribute("Sdk"));
        }

        [Fact]
        public void S1_AC1_NewProject_TargetsNet8()
        {
            Assert.Equal("net8.0", NewProject().Descendants("TargetFramework").Single().Value);
            Assert.Empty(NewProject().Descendants("TargetFrameworks"));
        }

        [Fact]
        public void S1_AC1_NewProject_UsesPackageReferenceNotPackagesConfig()
        {
            Assert.NotEmpty(PackageReferences());
            Assert.False(File.Exists(Path.Combine(Path.GetDirectoryName(NewProjectPath), "packages.config")));
            Assert.Empty(NewProject().Descendants("Reference"));
        }

        [Fact]
        public void S1_AC1_NewProject_IsRegisteredInSolution()
        {
            var sln = File.ReadAllText(Path.Combine(RepoRoot, "MicroProjectApplication.sln"));
            Assert.Contains("MicroProjectApplication.csproj", sln);
            Assert.Contains(Path.Combine("src", "MicroProjectApplication.Api", "MicroProjectApplication.Api.csproj").Replace('/', '\\'), sln);
        }

        [Fact]
        public void S1_AC1_LegacyFrameworkProject_RemainsPresent()
        {
            Assert.True(File.Exists(LegacyProjectPath));
            Assert.True(File.Exists(Path.Combine(RepoRoot, "Global.asax.cs")));
            Assert.True(File.Exists(Path.Combine(RepoRoot, "packages.config")));
            foreach (var file in new[] { "WebApiConfig.cs", "UnityConfig.cs", "SwaggerConfig.cs" })
                Assert.True(File.Exists(Path.Combine(RepoRoot, "App_Start", file)), file);

            var legacy = File.ReadAllText(LegacyProjectPath);
            Assert.Contains("<TargetFrameworkVersion>v4.8</TargetFrameworkVersion>", legacy);
            Assert.Contains("<Compile Include=\"Global.asax.cs\">", legacy);
        }

        [Fact]
        public void S1_AC2_NewProjectFile_HasNoSystemWebOrFrameworkOnlyReferences()
        {
            var project = NewProject();
            var referenceIds = project.Descendants()
                .Where(e => e.Name.LocalName.EndsWith("Reference", StringComparison.Ordinal))
                .Select(e => (string)e.Attribute("Include") ?? string.Empty)
                .ToArray();
            Assert.DoesNotContain(referenceIds, id => id.StartsWith("System.Web", StringComparison.OrdinalIgnoreCase));
            Assert.Empty(project.Descendants("TargetFrameworkVersion"));
            Assert.Empty(project.Descendants("HintPath"));
        }

        [Fact]
        public void S1_AC2_BuiltAssembly_DoesNotReferenceSystemWeb()
        {
            var referenced = ApiAssembly.GetReferencedAssemblies().Select(a => a.Name).ToArray();
            Assert.DoesNotContain(referenced, n => n.StartsWith("System.Web", StringComparison.Ordinal));
            Assert.DoesNotContain(referenced, n => n.StartsWith("Unity", StringComparison.Ordinal));
            Assert.DoesNotContain(referenced, n => n == "mscorlib");
        }

        [Fact]
        public void S1_AC3_NewProject_CompilesAgainstAspNetCoreMvcAndHosting()
        {
            var referenced = ApiAssembly.GetReferencedAssemblies().Select(a => a.Name).ToArray();
            Assert.Contains("Microsoft.AspNetCore.Mvc.Core", referenced);
            Assert.Contains(typeof(WebApplication).Assembly.GetName().Name, referenced);
        }

        [Fact]
        public void S1_AC4_NewProject_ReferencesSwashbuckleAspNetCore()
        {
            var swashbuckle = NewProject().Descendants("PackageReference")
                .Single(e => (string)e.Attribute("Include") == "Swashbuckle.AspNetCore");
            var version = Version.Parse((string)swashbuckle.Attribute("Version"));
            Assert.True(version.Major >= 6, $"Swashbuckle.AspNetCore {version} is not net8.0-compatible.");
        }

        [Fact]
        public void S1_AC4_NewProject_HasNoLegacySwaggerOrWebApiPackages()
        {
            var legacy = new[] { "Swashbuckle", "Swashbuckle.Core", "Unity.WebAPI", "WebActivatorEx" };
            var packages = PackageReferences();
            foreach (var id in legacy)
                Assert.DoesNotContain(id, packages, StringComparer.OrdinalIgnoreCase);
            Assert.DoesNotContain(packages, p => p.StartsWith("Microsoft.AspNet.", StringComparison.OrdinalIgnoreCase));
        }

        [Theory]
        [InlineData("MicroProjectApplication.Controllers")]
        [InlineData("MicroProjectApplication.Services")]
        [InlineData("MicroProjectApplication.Services.Interfaces")]
        [InlineData("MicroProjectApplication.Repositories")]
        [InlineData("MicroProjectApplication.Repositories.Interfaces")]
        [InlineData("MicroProjectApplication.Data")]
        [InlineData("MicroProjectApplication.Models")]
        public void S1_AC5_LayeredCode_IsCompiledIntoNewProject(string ns)
        {
            Assert.Contains(ApiAssembly.GetTypes(), t => t.Namespace == ns);
        }

        [Fact]
        public void S1_AC5_LegacyLayeredSources_AreLinkedUnchanged()
        {
            var includes = NewProject().Descendants("Compile").Select(e => (string)e.Attribute("Include")).ToArray();
            foreach (var dir in new[] { "Models", "Data", "Repositories", "Services" })
                Assert.Contains($"..\\..\\{dir}\\**\\*.cs", includes);
        }

        [Fact]
        public void S1_AC5_Controllers_AreAspNetCoreControllers()
        {
            var controllers = new[]
            {
                typeof(CustomersController), typeof(HealthController), typeof(OrdersController), typeof(ProductsController)
            };
            foreach (var controller in controllers)
                Assert.True(typeof(ControllerBase).IsAssignableFrom(controller), controller.Name);
        }

        [Fact]
        public void IMPACT_LinkedServices_BehaveAsInLegacyProjectUnderNet8()
        {
            var service = new ProductService(new ProductRepository());
            Assert.Equal("Pen", service.GetById(1).Name);
            Assert.Null(service.Create(null));
            Assert.Same(InMemoryDataStore.Products, service.GetAll());
        }

        [Fact]
        public void IMPACT_HealthController_ReturnsSameResultAsLegacy()
        {
            var result = Assert.IsType<OkObjectResult>(new HealthController().Get());
            Assert.Equal("Service is running.", result.Value);
        }

        [Fact]
        public void IMPACT_OrdersController_RejectsInvalidOrderAsLegacy()
        {
            var controller = new OrdersController(
                new OrderService(new OrderRepository(), new CustomerRepository(), new ProductRepository()));
            var result = Assert.IsType<BadRequestObjectResult>(
                controller.Create(new Order { CustomerId = 1, ProductId = 1, Quantity = 0 }));
            Assert.Equal("Invalid order. Check CustomerId, ProductId, and Quantity.", result.Value);
        }
    }
}
