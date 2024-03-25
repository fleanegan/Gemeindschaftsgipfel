using Kompetenzgipfel.Controllers.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Kompetenzgipfel.Controllers;

public class HomeController : AbstractController
{
    [HttpGet]
    [Authorize]
    public IActionResult GetInfo()
    {
        return Ok(new
        {
            key_information_detail_title = "Das Wichtigste auf einen Blick",
            key_information_detail_coordinates = "Dummykoordinaten",
            key_information_detail_dates = "24.08 - 12.09.2024",
            key_information_detail_accomodation = "Bring dein Zelt mit",
            key_information_detail_wifi_ssid = "sich3rh4it",
            key_information_detail_wifi_password = "1234567890",
            first_section_title = "Geimeinschaft",
            first_section_content =
                "Alle im selben Boot sein, aber kein Boot brauchen. Alle an einem Strang ziehen, ohne jemandem einen Strick draus zu drehen.",
            second_section_title = "Gipfel",
            second_section_content =
                "Das ist ja die Hoehe! - Tante Ulrike<br><br>Hier wächst zusammen, was zusammen gehört. Wir. Zusammen. Miteinander."
        });
    }
}