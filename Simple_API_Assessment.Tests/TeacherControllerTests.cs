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
    public class TeacherControllerTests : IDisposable
    {
        private const string Endpoint = "/api/teacher";

        private readonly TeacherApiFactory _factory;
        private readonly HttpClient _client;

        public TeacherControllerTests()
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

        private List<Teacher> StoredTeachers()
        {
            using var scope = _factory.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<DataContext>();
            return context.Teachers.ToList();
        }

        private static async Task AssertValidationErrorFor(HttpResponseMessage response, string field)
        {
            Assert.Equal(HttpStatusCode.UnprocessableEntity, response.StatusCode);
            using var body = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
            var errors = body.RootElement.GetProperty("errors");
            Assert.True(errors.TryGetProperty(field, out _), $"Expected a validation error for '{field}' but got: {errors}");
        }

        [Fact]
        public async Task Register_WithAllAttributes_Returns201AndCreatesTeacher()
        {
            var response = await PostJson("""{"name":"Ana","discipline":"Math","age":35}""");

            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
            Assert.Single(StoredTeachers());
        }

        [Fact]
        public async Task Register_WithAllAttributes_StoresSentValues()
        {
            var response = await PostJson("""{"name":"Ana","discipline":"Math","age":35}""");

            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
            var teacher = Assert.Single(StoredTeachers());
            Assert.Equal("Ana", teacher.Name);
            Assert.Equal("Math", teacher.Discipline);
            Assert.Equal(35, teacher.Age);

            var returned = await response.Content.ReadFromJsonAsync<Teacher>();
            Assert.NotNull(returned);
            Assert.Equal(teacher.Id, returned!.Id);
            Assert.Equal("Ana", returned.Name);
            Assert.Equal("Math", returned.Discipline);
            Assert.Equal(35, returned.Age);
        }

        [Fact]
        public async Task Register_WithoutName_Returns422AndCreatesNothing()
        {
            var response = await PostJson("""{"discipline":"Math","age":35}""");

            await AssertValidationErrorFor(response, "Name");
            Assert.Empty(StoredTeachers());
        }

        [Fact]
        public async Task Register_WithoutDiscipline_Returns422AndCreatesNothing()
        {
            var response = await PostJson("""{"name":"Ana","age":35}""");

            await AssertValidationErrorFor(response, "Discipline");
            Assert.Empty(StoredTeachers());
        }

        [Fact]
        public async Task Register_WithoutAge_Returns422AndCreatesNothing()
        {
            var response = await PostJson("""{"name":"Ana","discipline":"Math"}""");

            await AssertValidationErrorFor(response, "Age");
            Assert.Empty(StoredTeachers());
        }

        [Fact]
        public async Task Register_WithEmptyBody_Returns422AndCreatesNothing()
        {
            var response = await PostJson("{}");

            await AssertValidationErrorFor(response, "Name");
            await AssertValidationErrorFor(response, "Discipline");
            await AssertValidationErrorFor(response, "Age");
            Assert.Empty(StoredTeachers());
        }

        [Fact]
        public async Task Register_WithNoBodyAtAll_Returns422AndCreatesNothing()
        {
            var response = await _client.PostAsync(Endpoint, new StringContent(string.Empty, Encoding.UTF8, "application/json"));

            Assert.Equal(HttpStatusCode.UnprocessableEntity, response.StatusCode);
            Assert.Empty(StoredTeachers());
        }

        [Theory]
        [InlineData("""{"name":"","discipline":"Math","age":35}""", "Name")]
        [InlineData("""{"name":"   ","discipline":"Math","age":35}""", "Name")]
        [InlineData("""{"name":"Ana","discipline":"","age":35}""", "Discipline")]
        [InlineData("""{"name":"Ana","discipline":"  ","age":35}""", "Discipline")]
        [InlineData("""{"name":null,"discipline":"Math","age":35}""", "Name")]
        [InlineData("""{"name":"Ana","discipline":"Math","age":null}""", "Age")]
        public async Task Register_WithBlankOrNullAttribute_Returns422AndCreatesNothing(string json, string field)
        {
            var response = await PostJson(json);

            await AssertValidationErrorFor(response, field);
            Assert.Empty(StoredTeachers());
        }

        [Theory]
        [InlineData("""{"name":"Ana","discipline":"Math","age":"abc"}""")]
        [InlineData("""{"name":"Ana","discipline":"Math","age":35.5}""")]
        public async Task Register_WithNonIntegerAge_Returns422AndCreatesNothing(string json)
        {
            var response = await PostJson(json);

            Assert.Equal(HttpStatusCode.UnprocessableEntity, response.StatusCode);
            Assert.Empty(StoredTeachers());
        }
    }
}
