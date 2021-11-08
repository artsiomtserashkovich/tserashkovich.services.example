using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using NUnit.Framework;
using TaaS.Services.Example.Api.Storage.Persistence.Database;
using TaaS.Services.Example.Api.Storage.Persistence.Model;
using TaaS.Services.Example.Api.Storage.ViewModels.Write;

namespace TaaS.Services.Example.Api.Integration.Tests
{
    public class ApiPrintersControllerTests
    {
        private WebApplicationFactory<Startup> _factory;
        
        [SetUp]
        public void Setup()
        {
            _factory = new WebApplicationFactory<Startup>()
                .WithWebHostBuilder(builder =>
                {
                    builder.ConfigureServices(services =>
                    {
                        var databaseDescriptor = services.SingleOrDefault(
                            d => d.ServiceType == typeof(DbContextOptions<PrintersDatabaseContext>));
                        
                        services.Remove(databaseDescriptor);

                        services.AddDbContext<PrintersDatabaseContext>(options =>
                        {
                            options.UseInMemoryDatabase("InMemoryDbForTesting");
                        });
                    });
                });
            
        }

        [Test]
        public async Task GetPrinter_ExistInDatabase_OKWithJsonContent()
        {
            var client = _factory.CreateClient();
            var provider = _factory.Services;
            using var scope = provider.CreateScope();
            var context = GetContext(scope.ServiceProvider);
            var printer = new Printer
            {
                Id = Guid.NewGuid(), 
                Name = "test", 
                Description = "test", 
                Type = PrinterType.FDM,
                OwnerEmail = "test@test.com", 
                XSize = 100, 
                YSize = 200, 
                ZSize = 300
            };
            await AddPrinter(context, printer);

            var response = await client.GetAsync($"/api/printers/{printer.Id}");

            response.EnsureSuccessStatusCode();
            var stringContent = await response.Content.ReadAsStringAsync();
            stringContent.Should().ContainAll(
                printer.Id.ToString(),
                printer.Name,
                printer.Description,
                printer.OwnerEmail
            );
        }
        
        [Test]
        public async Task GetPrinter_NotExistInDatabase_NotFound()
        {
            var client = _factory.CreateClient();
            
            var response = await client.GetAsync($"/api/printers/{Guid.NewGuid()}");

            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }
        
        [Test]
        public async Task CreatePrinter_ValidRequestBody_ReturnNewId()
        {
            var client = _factory.CreateClient();
            var provider = _factory.Services;
            using var scope = provider.CreateScope();
            var context = GetContext(scope.ServiceProvider);
            var printer = new WritePrinter
            {
                Name = "test",
                Description = "test",
                Type = PrinterType.FDM,
                OwnerEmail = "test@test.com",
                XSize = 100,
                YSize = 200,
                ZSize = 300
            };
            
            var response = await client.PostAsync(
                "/api/printers", 
                new StringContent(
                    JsonConvert.SerializeObject(printer),
                    Encoding.UTF8,
                    MediaTypeNames.Application.Json));

            response.EnsureSuccessStatusCode();
            var stringContent = await response.Content.ReadAsStringAsync();
            var printerId = (await context.Printers.SingleOrDefaultAsync()).Id;
            stringContent.Should().Contain(printerId.Value.ToString());
        }
        
        [Test]
        public async Task CreatePrinter_InvalidRequestBody_ReturnBadRequest()
        {
            var client = _factory.CreateClient();
            
            var response = await client.PostAsync(
                "/api/printers", 
                new StringContent(
                    JsonConvert.SerializeObject("{ 'id' = '123131'}"),
                    Encoding.UTF8,
                    MediaTypeNames.Application.Json));

            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }
        
        [Test]
        public async Task EditPrinter_ValidRequestBodyAndExistInDatabase_ReturnOK()
        {
            var client = _factory.CreateClient();
            var provider = _factory.Services;
            var printer = new Printer
            {
                Id = Guid.NewGuid(), 
                Name = "test", 
                Description = "test", 
                Type = PrinterType.FDM,
                OwnerEmail = "test@test.com", 
                XSize = 100, 
                YSize = 200, 
                ZSize = 300
            };
            var writePrinter = new WritePrinter
            {
                Name = "test1", 
                Description = "tes1", 
                Type = PrinterType.SLD,
                OwnerEmail = "test@test.com", 
                XSize = 100, 
                YSize = 200, 
                ZSize = 300
            };
            using (var scope = provider.CreateScope())
            {
                var context = GetContext(scope.ServiceProvider);
                await AddPrinter(context, printer);
            }

            var response = await client.PutAsync(
                $"/api/printers/{printer.Id}", 
                new StringContent(
                    JsonConvert.SerializeObject(writePrinter),
                    Encoding.UTF8,
                    MediaTypeNames.Application.Json));

            response.EnsureSuccessStatusCode();
            using (var scope = provider.CreateScope())
            {
                var context = GetContext(scope.ServiceProvider);
                var resultPrinter = await context.Printers
                    .SingleOrDefaultAsync(x => x.Id == printer.Id);
                resultPrinter.Name.Should().Be(writePrinter.Name);
                resultPrinter.Type.Should().Be(writePrinter.Type);
                resultPrinter.Description.Should().Be(writePrinter.Description);
            }
        }
        
        [Test]
        public async Task EditPrinter_ValidRequestBodyAndNotExistInDatabase_ReturnNotFound()
        {
            var client = _factory.CreateClient();
            var printer = new WritePrinter
            {
                Name = "test",
                Description = "test",
                Type = PrinterType.FDM,
                OwnerEmail = "test@test.com",
                XSize = 100,
                YSize = 200,
                ZSize = 300
            };
            
            var response = await client.PutAsync(
                $"/api/printers/{Guid.NewGuid()}", 
                new StringContent(
                    JsonConvert.SerializeObject(printer),
                    Encoding.UTF8,
                    MediaTypeNames.Application.Json));

            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }
        
        [Test]
        public async Task EditPrinter_InvalidRequestBody_ReturnBadRequest()
        {
            var client = _factory.CreateClient();
            
            var response = await client.PutAsync(
                $"/api/printers/{Guid.NewGuid()}", 
                new StringContent(
                    JsonConvert.SerializeObject("{'name' = 'test'}"),
                    Encoding.UTF8,
                    MediaTypeNames.Application.Json));

            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }
        
        [Test]  
        public async Task DeletePrinter_ExistInDatabase_OK()
        {
            var client = _factory.CreateClient();
            var provider = _factory.Services;
            var printer = new Printer
            {
                Id = Guid.NewGuid(),
                Name = "test",
                Description = "test",
                Type = PrinterType.FDM,
                OwnerEmail = "test@test.com",
                XSize = 100,
                YSize = 200,
                ZSize = 300
            };
            using (var scope = provider.CreateScope())
            {
                var context = GetContext(scope.ServiceProvider);
                await AddPrinter(context, printer);
            }

            var response = await client.DeleteAsync($"/api/printers/{printer.Id}");

            response.EnsureSuccessStatusCode();
            using (var scope = provider.CreateScope())
            {
                var context = GetContext(scope.ServiceProvider);
                (await context.Printers.CountAsync(x => x.Id == printer.Id))
                    .Should().Be(0);
            }
        }
        
        [Test]
        public async Task DeletePrinter_NotExistInDatabase_NotFound()
        {
            var client = _factory.CreateClient();

            var response = await client.DeleteAsync($"/api/printers/{Guid.NewGuid()}");

            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        private static PrintersDatabaseContext GetContext(IServiceProvider provider)
        {
            return provider.GetRequiredService<PrintersDatabaseContext>();
        }
        
        private static async Task AddPrinter(PrintersDatabaseContext context, Printer printer)
        {
            await context.Printers.AddAsync(printer);
            await context.SaveChangesAsync();
        }
    }
}