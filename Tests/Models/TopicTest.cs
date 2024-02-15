using System.ComponentModel.DataAnnotations;
using Kompetenzgipfel.Models;

namespace Tests.Models;

public class TopicTest
{
    [Fact]
    public void Test_instantiation_GIVEN_empty_title_THEN_throw_error()
    {
        Assert.Throws<ValidationException>(() => Topic.Create("", "Non-empty description"));
    }

    [Fact]
    public void Test_instantiation_GIVEN_too_long_title_THEN_throw_error()
    {
        Assert.Throws<ValidationException>(() =>
            Topic.Create(
                "this title is too looooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooong",
                "Non-empty description"
            )
        );
    }
}