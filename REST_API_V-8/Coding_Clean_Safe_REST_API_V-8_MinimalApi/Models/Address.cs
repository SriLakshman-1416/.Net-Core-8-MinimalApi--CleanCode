using Microsoft.AspNetCore.Mvc;

namespace Coding_Clean_Safe_REST_API_V_8_MinimalApi.Models;

public class Address
{
    [FromRoute]
    public int AddressId { get; set; }
    [FromForm]
    public int StreetNumber { get; set; }
    [FromForm]
    public string StreetName { get; set; }
    [FromForm]
    public string StreetType { get; set; }
    [FromForm]
    public string City { get; set; }
    [FromForm]
    public string Country { get; set; }
    [FromForm]
    public int PostalCode { get; set; }
    [FromForm]
    public Address AlternateAddress { get; set; }
}
