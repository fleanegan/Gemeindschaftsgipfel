using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Gemeinschaftsgipfel.Properties;

namespace Gemeinschaftsgipfel.Models;

public class Topic
{
    internal Topic()
    {
        Title = "";
        Description = "";
        User = new User();
    }

    public Topic(string description, string title, User user, ICollection<Vote> votes)
    {
        Description = description;
        Title = title;
        User = user;
        Votes = votes;
    }

    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public string Id { get; set; } = null!;

    [StringLength(Constants.MaxLengthTitle, ErrorMessage = Constants.MaxLengthTitleErrorMessage)]
    [Required(ErrorMessage = Constants.EmptyTitleErrorMessage)]
    public string Title { get; set; }

    [StringLength(Constants.MaxLengthDescription, ErrorMessage = Constants.MaxLengthDescriptionErrorMessage)]
    public string Description { get; set; }

    [Required(ErrorMessage = Constants.MissingPresentationTimeErrorMessage)] public int PresentationTimeInMinutes { get; set; }

    [Required] public User User { get; init; }

    public ICollection<Vote> Votes { get; set; } = [];

    public static Topic Create(string title, int presentationTimeInMinutes, string description, User user)
    {
        var model = new Topic { Title = title, Description = description, PresentationTimeInMinutes = presentationTimeInMinutes, User = user };
        Validator.ValidateObject(model, new ValidationContext(model), true);
        return model;
    }
}
