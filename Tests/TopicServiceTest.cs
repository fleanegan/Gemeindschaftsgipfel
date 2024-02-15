using Kompetenzgipfel.Models;
using Kompetenzgipfel.Services;

namespace Tests;

public class TopicServiceTest
{
    private static TopicService GetService(TopicRepository? topicRepository = null)
    {
        if (topicRepository == null)
        {
            using var dbContext = TestHelper.GetDbContext();
            topicRepository = new TopicRepository(dbContext);
        }

        return new TopicService(topicRepository);
    }

    [Theory]
    [InlineData("Correct title", "")]
    [InlineData("Correct title", "Non empty but also correct title")]
    public void Test_add_GIVEN_correct_input_THEN_store_in_db(string title, string description)
    {
        using var dbContext = TestHelper.GetDbContext();
        var repository = new TopicRepository(dbContext);
        var service = GetService(repository);

        service.AddTopic(Topic.Create(title, description));

        var result = repository.GetAll().FirstOrDefault(topic => topic.Title == title);
        Assert.NotNull(result);
        Assert.Equal(title, result.Title);
        Assert.Equal(description, result.Description);
    }
}