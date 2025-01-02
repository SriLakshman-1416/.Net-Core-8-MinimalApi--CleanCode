

namespace Coding_Clean_Safe_REST_API_V_8_MinimalApi.Models;

public class Address
{
  
    public int AddressId { get; set; }
    
    public int StreetNumber { get; set; }

    public string StreetName { get; set; }

    public string StreetType { get; set; }

    public string City { get; set; }

    public string Country { get; set; }

    public int PostalCode { get; set; }
   
    public Address AlternateAddress { get; set; }
}
