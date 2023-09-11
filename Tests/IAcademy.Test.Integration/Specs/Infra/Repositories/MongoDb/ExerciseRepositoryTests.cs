using CrossCutting.Enums;
using Domain.Entities;
using Domain.Infra;
using FluentAssertions;
using IAcademy.Test.Integration.Base;
using IAcademy.Test.Integration.Configuration;
using IAcademy.Test.Shared.Builders;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;

namespace IAcademy.Test.Integration.Specs.Infra.Repositories.MongoDb;

[Collection(Constants.WEB_API_TEST_COLLECTION_NAME)]
public class ExerciseRepositoryTests : IntegrationTestBase
{
    private readonly IntegrationTestsFixture _fixture;

    public ExerciseRepositoryTests(IntegrationTestsFixture fixture)
        : base(fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task ShouldGetExercise()
    {
        var exercise = new ExerciseBuilder().Build();

        _fixture.DbContext.Exercise.InsertOne(exercise);

        var result = await _fixture.serviceProvider.GetRequiredService<IExerciseRepository>().Get(exercise.Id);

        result.Should().NotBeNull();
        result.Should().BeEquivalentTo(exercise);
    }

    [Fact]
    public async Task ShouldGetAllExercises()
    {
        var firstExercise = new ExerciseBuilder().Build();
        var secondExercise = new ExerciseBuilder().Build();

        _fixture.DbContext.Exercise.InsertMany(new List<Exercise>()
        {
            firstExercise,
            secondExercise
        });

        var result = await _fixture.serviceProvider.GetRequiredService<IExerciseRepository>().GetAllByIds(new List<string>()
        {
            firstExercise.Id,
            secondExercise.Id
        });

        result.Any().Should().BeTrue();
        result.Count().Should().Be(2);
    }

    [Fact]
    public async Task ShouldGetAllExercisesByOwnerIdAndType()
    {
        var ownerId = Guid.NewGuid().ToString();
        var exerciseType = ExerciseType.Default;

        var firstExercise = new ExerciseBuilder().WithOwnerId(ownerId).WithType(exerciseType).Build();
        var secondExercise = new ExerciseBuilder().WithOwnerId(ownerId).WithType(exerciseType).Build();

        await _fixture.DbContext.Exercise.InsertManyAsync(new List<Exercise>
        {
            firstExercise,
            secondExercise
        });

        var result = await _fixture.serviceProvider.GetRequiredService<IExerciseRepository>().GetAllByOwnerIdAndType(ownerId, exerciseType);

        result.Should().NotBeEmpty();
        result.Count.Should().Be(2);
    }

    [Fact]
    public async Task ShouldSaveExercise()
    {
        var exercise = new ExerciseBuilder().Build();

        var isSuccess = await _fixture.serviceProvider.GetRequiredService<IExerciseRepository>().Save(exercise);

        isSuccess.Should().BeTrue();

        var filter = Builders<Exercise>.Filter.Eq(s => s.Id, exercise.Id);
        var savedExercise = _fixture.DbContext.Exercise.FindSync(filter).FirstOrDefault();

        savedExercise.Should().NotBeNull();
        savedExercise.Should().BeEquivalentTo(exercise);
    }

    [Fact]
    public async Task ShouldUpdateExercise()
    {
        var oldExercise = new ExerciseBuilder().Build();

        _fixture.DbContext.Exercise.InsertOne(oldExercise);

        var updatedExercise = new Exercise
        {
            OwnerId = "New",
            CorrectionId = "New",
            ConfigurationId = "New",
            Status = ExerciseStatus.Finished,
            Type = ExerciseType.Pendency,
            SendedAt = DateTime.MaxValue,
            TopicIndex = "New",
            Title = "New",
            Exercises = new()
            {
                new ActivityBuilder().Build()
            }
        };

        var result = await _fixture.serviceProvider.GetRequiredService<IExerciseRepository>().Update(oldExercise.Id, updatedExercise);

        result.Should().BeTrue();

        var retrievedExercise = _fixture.DbContext.Exercise.FindSync(e => e.Id == oldExercise.Id).FirstOrDefault();

        retrievedExercise.Should().NotBeNull();
        retrievedExercise.OwnerId.Should().Be(updatedExercise.OwnerId);
        retrievedExercise.ConfigurationId.Should().Be(updatedExercise.ConfigurationId);
        retrievedExercise.CorrectionId.Should().Be(updatedExercise.CorrectionId);
        retrievedExercise.Status.Should().Be(updatedExercise.Status);
        retrievedExercise.Type.Should().Be(updatedExercise.Type);
        retrievedExercise.SendedAt.Should().Be(updatedExercise.SendedAt);
        retrievedExercise.TopicIndex.Should().Be(updatedExercise.TopicIndex);
        retrievedExercise.Title.Should().Be(updatedExercise.Title);
        retrievedExercise.Exercises.Should().BeEquivalentTo(updatedExercise.Exercises);
    }
}
