using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using APIExample.Data;
using APIExample.Models;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Simple_API_Assessment.Tests
{
    public class DisciplineControllerTests : IDisposable
    {
        private const string Endpoint = "/api/discipline";

        private readonly TeacherApiFactory _factory;
        private readonly HttpClient _client;

        public DisciplineControllerTests()
        {
            _factory = new TeacherApiFactory();
            _client = _factory.CreateClient(new() { BaseAddress = new Uri("https://localhost") });
        }

        public void Dispose()
        {
            _client.Dispose();
            _factory.Dispose();
        }

        private Task<HttpResponseMessage> PostJson(string json) =>
            _client.PostAsync(Endpoint, new StringContent(json, Encoding.UTF8, "application/json"));

        private List<Discipline> StoredDisciplines()
        {
            using var scope = _factory.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<DataContext>();
            return context.Disciplines.ToList();
        }

        private static async Task AssertValidationErrorFor(HttpResponseMessage response, string field)
        {
            Assert.Equal(HttpStatusCode.UnprocessableEntity, response.StatusCode);
            using var body = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
            var errors = body.RootElement.GetProperty("errors");
            Assert.True(errors.TryGetProperty(field, out _), $"Expected a validation error for '{field}' but got: {errors}");
        }

        [Fact]
        public async Task Register_WithAllAttributes_Returns201AndStoresSentValues()
        {
            var response = await PostJson("""{"name":"Math","numberOfAlumns":30}""");

            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
            var discipline = Assert.Single(StoredDisciplines());
            Assert.Equal("Math", discipline.Name);
            Assert.Equal(30, discipline.NumberOfAlumns);

            var returned = await response.Content.ReadFromJsonAsync<Discipline>();
            Assert.NotNull(returned);
            Assert.Equal(discipline.Id, returned!.Id);
            Assert.Equal("Math", returned.Name);
            Assert.Equal(30, returned.NumberOfAlumns);
        }

        [Fact]
        public async Task Register_WithoutName_Returns422AndCreatesNothing()
        {
            var response = await PostJson("""{"numberOfAlumns":30}""");

            await AssertValidationErrorFor(response, "Name");
            Assert.Empty(StoredDisciplines());
        }

        [Fact]
        public async Task Register_WithoutNumberOfAlumns_Returns422AndCreatesNothing()
        {
            var response = await PostJson("""{"name":"Math"}""");

            await AssertValidationErrorFor(response, "NumberOfAlumns");
            Assert.Empty(StoredDisciplines());
        }

        [Fact]
        public async Task Register_WithEmptyBody_Returns422AndCreatesNothing()
        {
            var response = await PostJson("{}");

            await AssertValidationErrorFor(response, "Name");
            await AssertValidationErrorFor(response, "NumberOfAlumns");
            Assert.Empty(StoredDisciplines());
        }

        [Fact]
        public async Task Register_WithNoBodyAtAll_Returns422AndCreatesNothing()
        {
            var response = await _client.PostAsync(Endpoint, new StringContent(string.Empty, Encoding.UTF8, "application/json"));

            Assert.Equal(HttpStatusCode.UnprocessableEntity, response.StatusCode);
            Assert.Empty(StoredDisciplines());
        }

        [Theory]
        [InlineData("""{"name":"","numberOfAlumns":30}""", "Name")]
        [InlineData("""{"name":"   ","numberOfAlumns":30}""", "Name")]
        [InlineData("""{"name":null,"numberOfAlumns":30}""", "Name")]
        [InlineData("""{"name":"Math","numberOfAlumns":null}""", "NumberOfAlumns")]
        public async Task Register_WithBlankOrNullAttribute_Returns422AndCreatesNothing(string json, string field)
        {
            var response = await PostJson(json);

            await AssertValidationErrorFor(response, field);
            Assert.Empty(StoredDisciplines());
        }

        [Theory]
        [InlineData("""{"name":"Math","numberOfAlumns":""}""")]
        [InlineData("""{"name":"Math","numberOfAlumns":"abc"}""")]
        [InlineData("""{"name":"Math","numberOfAlumns":30.5}""")]
        public async Task Register_WithNonIntegerNumberOfAlumns_Returns422AndCreatesNothing(string json)
        {
            var response = await PostJson(json);

            Assert.Equal(HttpStatusCode.UnprocessableEntity, response.StatusCode);
            Assert.Empty(StoredDisciplines());
        }
    }
}
