using Coding_Clean_Safe_REST_API_V_8_MinimalApi.Endpoints;

namespace Coding_Clean_Safe_REST_API_V_8_MinimalApi.GroupEndpoints;

public static class CountryCurdOperationGroup
{
    public static void AddCountryEndpoints(this WebApplication app)
    {
        var crudCountryGroup = app.MapGroup("/countriescurdoperation");

        crudCountryGroup.MapPost("/", CountryCurdOperationEndPoints.PostCountry);
        crudCountryGroup.MapGet("/{id}", CountryCurdOperationEndPoints.GetCountry);
        crudCountryGroup.MapGet("/", CountryCurdOperationEndPoints.GetCountries);
        crudCountryGroup.MapPut("/", CountryCurdOperationEndPoints.UpdateCountry);
        crudCountryGroup.MapPatch("/{id}", CountryCurdOperationEndPoints.PatchUpdateCountry);
        crudCountryGroup.MapDelete("/{id}", CountryCurdOperationEndPoints.DeleteCountry);

    }
}