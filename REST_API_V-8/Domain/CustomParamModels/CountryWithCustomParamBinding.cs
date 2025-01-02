using Microsoft.AspNetCore.Http;
using System.Reflection;
using System.Text.Json;

namespace Domain.CustomParamModels;

public class CountryWithCustomParamBinding
{
    /// <summary>
    /// The country Id
    /// </summary>
    public int? Id { get; set; }

    /// <summary>
    /// The country name
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// The country description
    /// </summary>
    public string Description { get; set; }

    /// <summary>
    /// The country flag URI
    /// </summary>
    public string FlagUri { get; set; }

    public static ValueTask<CountryWithCustomParamBinding> BindAsync(HttpContext context, ParameterInfo parameter)
    {
        var countryFromValue = context.Request.Form["CountryWithCustomParamBinding"];
        var result = JsonSerializer.Deserialize<CountryWithCustomParamBinding>(countryFromValue);

        return ValueTask.FromResult(result);
    }
}
