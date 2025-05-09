using System.ComponentModel.DataAnnotations;

namespace Gemeinschaftsgipfel.Controllers.DTOs;

public class SupportPromiseDto(string supportTaskId)
{
    [Required] public string SupportTaskId { get; } = supportTaskId;
}