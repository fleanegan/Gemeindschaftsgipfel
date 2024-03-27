using System.ComponentModel.DataAnnotations;

namespace Kompetenzgipfel.Controllers.DTOs;

public class SupportPromiseDto(string supportTaskId)
{
    [Required] public string SupportTaskId { get; init; } = supportTaskId;
}