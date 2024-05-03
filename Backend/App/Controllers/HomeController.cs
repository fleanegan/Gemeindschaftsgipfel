using Gemeinschaftsgipfel.Controllers.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Gemeinschaftsgipfel.Controllers;

public class HomeController : AbstractController
{
    [HttpGet]
    [AllowAnonymous]
    public IActionResult GetImpressum()
    {
        return Ok(Environment.GetEnvironmentVariable("LOGIN_PAGE_LEGAL") ?? "bana");
    }

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
            key_information_detail_fee_receiver = Environment.GetEnvironmentVariable("LANDING_PAGE_DETAIL_FEE_RECEIVER") ?? "LANDING_PAGE_DETAIL_FEE_RECEIVER",
            key_information_detail_fee_iban = Environment.GetEnvironmentVariable("LANDING_PAGE_DETAIL_FEE_IBAN") ?? "LANDING_PAGE_DETAIL_FEE_IBAN",
            key_information_detail_fee_deadline = Environment.GetEnvironmentVariable("LANDING_PAGE_DETAIL_FEE_DEADLINE") ?? "LANDING_PAGE_DETAIL_FEE_DEADLINE",
            key_information_detail_fee_amount = Environment.GetEnvironmentVariable("LANDING_PAGE_DETAIL_FEE_AMOUNT") ?? "LANDING_PAGE_DETAIL_FEE_AMOUNT",
            key_information_detail_fee_reason = Environment.GetEnvironmentVariable("LANDING_PAGE_DETAIL_FEE_REASON") ?? "LANDING_PAGE_DETAIL_FEE_REASON",
            content = Environment.GetEnvironmentVariable("LANDING_PAGE_CONTENT") ?? "LANDING_PAGE_CONTENT"
        });
    }
}
