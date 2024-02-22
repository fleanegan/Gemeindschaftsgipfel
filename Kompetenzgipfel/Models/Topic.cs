using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Kompetenzgipfel.Properties;

namespace Kompetenzgipfel.Models;

public class Topic
{
    internal Topic()
    {
    }

    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public string Id { get; set; }

    [StringLength(Constants.MaxLengthTitle, ErrorMessage = Constants.MaxLengthTitleErrorMessage)]
    [Required(ErrorMessage = Constants.EmptyTitleErrorMessage)]
    public string Title { get; set; }

    [StringLength(Constants.MaxLengthDescription, ErrorMessage = Constants.MaxLengthDescriptionErrorMessage)]
    public string? Description { get; set; }

    public User Presenter { get; set; }

    public static Topic Create(string title, string description, User user)
    {
        var model = new Topic { Title = title, Description = description, Presenter = user };
        Validator.ValidateObject(model, new ValidationContext(model), true);
        return model;
    }
}