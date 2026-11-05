using APIExample.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Simple_API_Assessment.Tests
{
    //Hosts the API in memory and swaps PostgreSQL for an isolated EF Core in-memory database,
    //so the tests run without any external service.
    public class TeacherApiFactory : WebApplicationFactory<Program>
    {
        private readonly string _databaseName = $"TeacherTests-{Guid.NewGuid()}";

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.UseEnvironment("Testing");
            builder.ConfigureServices(services =>
            {
                var descriptor = services.Single(d => d.ServiceType == typeof(DbContextOptions<DataContext>));
                services.Remove(descriptor);
                services.AddDbContext<DataContext>(options => options.UseInMemoryDatabase(_databaseName));
            });
        }
    }
}
