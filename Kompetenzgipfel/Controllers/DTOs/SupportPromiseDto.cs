using System.ComponentModel.DataAnnotations;

namespace Kompetenzgipfel.Controllers.DTOs;

public class SupportPromiseDto
{
    [Required] public string SupportTaskId { get; set; }
}