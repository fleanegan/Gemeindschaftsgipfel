using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;

namespace Kompetenzgipfel.Controllers.Helpers;

public abstract class AbstractController : Controller
{
    public string GetUserNameFromAuthorization()
    {
        return HttpContext.User.FindFirst(c => c.Type == ClaimTypes.Name)?.Value!;
    }
}