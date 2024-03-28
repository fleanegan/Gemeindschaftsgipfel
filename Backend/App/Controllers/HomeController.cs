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
            key_information_detail_coordinates = Environment.GetEnvironmentVariable("LANDING_PAGE_DETAIL_COORDINATES") ?? "LANDING_PAGE_DETAIL_COORDINATES",
            key_information_detail_dates = Environment.GetEnvironmentVariable("LANDING_PAGE_DETAIL_DATES") ?? "LANDING_PAGE_DETAIL_DATES",
            key_information_detail_accomodation = Environment.GetEnvironmentVariable("LANDING_PAGE_DETAIL_ACCOMODATION" ?? "LANDING_PAGE_DETAIL_ACCOMODATION"),
            key_information_detail_wifi_ssid = Environment.GetEnvironmentVariable("LANDING_PAGE_DETAIL_WIFI_SSID") ?? "LANDING_PAGE_DETAIL_WIFI_SSID",
            key_information_detail_wifi_password = Environment.GetEnvironmentVariable("LANDING_PAGE_DETAIL_WIFI_PASSWORD") ?? "LANDING_PAGE_DETAIL_WIFI_PASSWORD",
            first_section_title = "Geimeinschaft",
            first_section_content =
                "Alle im selben Boot sein, aber kein Boot brauchen. Alle an einem Strang ziehen, ohne jemandem einen Strick draus zu drehen.",
            second_section_title = "Gipfel",
            second_section_content =
                "Das ist ja die Hoehe! - Tante Ulrike<br><br>Hier wächst zusammen, was zusammen gehört. Wir. Zusammen. Miteinander."
        });
    }
}