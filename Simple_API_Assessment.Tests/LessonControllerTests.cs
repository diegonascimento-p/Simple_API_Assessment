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
    public class LessonControllerTests : IDisposable
    {
        private const string Endpoint = "/api/lesson";

        private readonly TeacherApiFactory _factory;
        private readonly HttpClient _client;

        public LessonControllerTests()
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

        private void SeedTeacher(string name, string discipline, int age)
        {
            using var scope = _factory.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<DataContext>();
            context.Teachers.Add(new Teacher { Name = name, Discipline = discipline, Age = age });
            context.SaveChanges();
        }

        private List<Lesson> StoredLessons()
        {
            using var scope = _factory.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<DataContext>();
            return context.Lessons.ToList();
        }

        private List<Teacher> StoredTeachers()
        {
            using var scope = _factory.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<DataContext>();
            return context.Teachers.ToList();
        }

        private static async Task<JsonElement> AssertValidationErrorFor(HttpResponseMessage response, string field)
        {
            Assert.Equal(HttpStatusCode.UnprocessableEntity, response.StatusCode);
            using var body = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
            var errors = body.RootElement.GetProperty("errors");
            Assert.True(errors.TryGetProperty(field, out var fieldErrors), $"Expected a validation error for '{field}' but got: {errors}");
            return fieldErrors.Clone();
        }

        [Fact]
        public async Task Register_WithDisciplineOfExistingTeacher_Returns201AndCreatesLesson()
        {
            SeedTeacher("Ana", "Geography", 35);

            var response = await PostJson("""{"disciplineId":"Geography","date":"2026-11-05","numberPresent":20,"numberAbsent":3}""");

            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
            Assert.Single(StoredLessons());
        }

        [Fact]
        public async Task Register_StoresAndReturnsSentValues()
        {
            SeedTeacher("Ana", "Geography", 35);

            var response = await PostJson("""{"disciplineId":"Geography","date":"2026-11-05","numberPresent":20,"numberAbsent":3}""");

            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
            var lesson = Assert.Single(StoredLessons());
            Assert.Equal("Geography", lesson.DisciplineId);
            Assert.Equal(new DateOnly(2026, 11, 5), lesson.Date);
            Assert.Equal(20, lesson.NumberPresent);
            Assert.Equal(3, lesson.NumberAbsent);

            var returned = await response.Content.ReadFromJsonAsync<Lesson>();
            Assert.NotNull(returned);
            Assert.Equal(lesson.Id, returned!.Id);
            Assert.Equal("Geography", returned.DisciplineId);
            Assert.Equal(new DateOnly(2026, 11, 5), returned.Date);
            Assert.Equal(20, returned.NumberPresent);
            Assert.Equal(3, returned.NumberAbsent);
        }

        [Fact]
        public async Task Register_StoredDisciplineIdMatchesTeachersDiscipline()
        {
            SeedTeacher("Ana", "Geography", 35);
            SeedTeacher("Bruno", "Math", 41);

            var response = await PostJson("""{"disciplineId":"Math","date":"2026-11-05","numberPresent":18,"numberAbsent":0}""");

            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
            var lesson = Assert.Single(StoredLessons());
            var teacher = Assert.Single(StoredTeachers(), t => t.Name == "Bruno");
            Assert.Equal(teacher.Discipline, lesson.DisciplineId);
        }

        [Fact]
        public async Task Register_WithDisciplineNotLinkedToAnyTeacher_Returns422WithMessage()
        {
            SeedTeacher("Ana", "Geography", 35);

            var response = await PostJson("""{"disciplineId":"History","date":"2026-11-05","numberPresent":20,"numberAbsent":3}""");

            var errors = await AssertValidationErrorFor(response, "DisciplineId");
            var message = errors[0].GetString();
            Assert.Equal("The discipline ID 'History' is not linked to any teacher.", message);
        }

        [Theory]
        [InlineData("History")]
        [InlineData("geography")]
        public async Task Register_WithDisciplineNotLinkedToAnyTeacher_CreatesNothing(string disciplineId)
        {
            SeedTeacher("Ana", "Geography", 35);

            var response = await PostJson($$"""{"disciplineId":"{{disciplineId}}","date":"2026-11-05","numberPresent":20,"numberAbsent":3}""");

            Assert.Equal(HttpStatusCode.UnprocessableEntity, response.StatusCode);
            Assert.Empty(StoredLessons());
        }

        [Fact]
        public async Task Register_WhenNoTeachersExist_Returns422AndCreatesNothing()
        {
            var response = await PostJson("""{"disciplineId":"Geography","date":"2026-11-05","numberPresent":20,"numberAbsent":3}""");

            await AssertValidationErrorFor(response, "DisciplineId");
            Assert.Empty(StoredLessons());
        }

        [Theory]
        [InlineData("""{"date":"2026-11-05","numberPresent":20,"numberAbsent":3}""", "DisciplineId")]
        [InlineData("""{"disciplineId":"","date":"2026-11-05","numberPresent":20,"numberAbsent":3}""", "DisciplineId")]
        [InlineData("""{"disciplineId":"   ","date":"2026-11-05","numberPresent":20,"numberAbsent":3}""", "DisciplineId")]
        [InlineData("""{"disciplineId":"Geography","numberPresent":20,"numberAbsent":3}""", "Date")]
        [InlineData("""{"disciplineId":"Geography","date":"2026-11-05","numberAbsent":3}""", "NumberPresent")]
        [InlineData("""{"disciplineId":"Geography","date":"2026-11-05","numberPresent":20}""", "NumberAbsent")]
        [InlineData("""{"disciplineId":"Geography","date":"2026-11-05","numberPresent":-1,"numberAbsent":3}""", "NumberPresent")]
        [InlineData("""{"disciplineId":"Geography","date":"2026-11-05","numberPresent":20,"numberAbsent":-1}""", "NumberAbsent")]
        public async Task Register_WithMissingOrInvalidField_Returns422AndCreatesNothing(string json, string field)
        {
            SeedTeacher("Ana", "Geography", 35);

            var response = await PostJson(json);

            await AssertValidationErrorFor(response, field);
            Assert.Empty(StoredLessons());
        }

        [Theory]
        [InlineData("""{"disciplineId":"Geography","date":"05/11/2026","numberPresent":20,"numberAbsent":3}""")]
        [InlineData("""{"disciplineId":"Geography","date":"2026-13-40","numberPresent":20,"numberAbsent":3}""")]
        [InlineData("""{"disciplineId":"Geography","date":"not a date","numberPresent":20,"numberAbsent":3}""")]
        [InlineData("""{"disciplineId":"Geography","date":"2026-11-05","numberPresent":"abc","numberAbsent":3}""")]
        [InlineData("""{"disciplineId":"Geography","date":"2026-11-05","numberPresent":20,"numberAbsent":1.5}""")]
        public async Task Register_WithMalformedValue_Returns422AndCreatesNothing(string json)
        {
            SeedTeacher("Ana", "Geography", 35);

            var response = await PostJson(json);

            Assert.Equal(HttpStatusCode.UnprocessableEntity, response.StatusCode);
            Assert.Empty(StoredLessons());
        }

        [Fact]
        public async Task Register_WithEmptyBody_Returns422AndCreatesNothing()
        {
            SeedTeacher("Ana", "Geography", 35);

            var response = await PostJson("{}");

            await AssertValidationErrorFor(response, "DisciplineId");
            await AssertValidationErrorFor(response, "Date");
            await AssertValidationErrorFor(response, "NumberPresent");
            await AssertValidationErrorFor(response, "NumberAbsent");
            Assert.Empty(StoredLessons());
        }
    }
}
