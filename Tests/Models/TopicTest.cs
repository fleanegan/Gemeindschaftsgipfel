using System.ComponentModel.DataAnnotations;
using Kompetenzgipfel.Models;

namespace Tests.Models;

public class TopicTest
{
    [Fact]
    public void Test_instantiation_GIVEN_empty_title_THEN_throw_error()
    {
        Assert.Throws<ValidationException>(() => Topic.Create("", "Non-empty description", new User { Id = "testId" }));
    }

    [Fact]
    public void Test_instantiation_GIVEN_too_long_title_THEN_throw_error()
    {
        Assert.Throws<ValidationException>(() =>
            Topic.Create(
                "this title is too looooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooong",
                "Non-empty description",
                new User { Id = "testId" }
            )
        );
    }

    [Fact]
    public void Test_instantiation_GIVEN_correct_input_THEN_create_instance()
    {
        Topic.Create("title", "description", new User { Id = "testId" });
    }
}