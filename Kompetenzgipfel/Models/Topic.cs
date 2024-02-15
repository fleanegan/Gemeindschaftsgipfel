using System.ComponentModel.DataAnnotations;
using Kompetenzgipfel.Properties;

namespace Kompetenzgipfel.Models;

public class Topic
{
    internal Topic()
    {
    }

    public int Id { get; init; }

    [StringLength(Constants.MaxLengthTitle, ErrorMessage = Constants.MaxLengthTitleErrorMessage)]
    [Required(ErrorMessage = Constants.EmptyTitleErrorMessage)]
    public string Title { get; set; }

    public string? Description { get; set; }

    public static Topic Create(string title, string description)
    {
        var model = new Topic { Title = title, Description = description };
        Validator.ValidateObject(model, new ValidationContext(model), true);
        return model;
    }
}