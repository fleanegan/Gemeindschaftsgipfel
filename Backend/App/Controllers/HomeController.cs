using Gemeinschaftsgipfel.Controllers.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Gemeinschaftsgipfel.Controllers;

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
            content = Environment.GetEnvironmentVariable("LANDING_PAGE_CONTENT") ?? "LANDING_PAGE_CONTENT"
        });
    }
}
