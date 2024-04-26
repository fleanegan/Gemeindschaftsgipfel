using System.ComponentModel.DataAnnotations;
using Constants = Gemeinschaftsgipfel.Properties.Constants;

namespace Gemeinschaftsgipfel.Controllers.DTOs;

public class TopicUpdateDto(string id, string title, int presentationTimeInMinutes, string? description)
{
    [Required(ErrorMessage = Constants.EmptyIdErrorMessage)]
    public string Id { get; } = id;

    public string? Description { get; } = description;

    [Required(ErrorMessage = Constants.EmptyTitleErrorMessage)]
    public string Title { get; } = title;

    [Required] public int PresentationTimeInMinutes {get;} = presentationTimeInMinutes;
}
