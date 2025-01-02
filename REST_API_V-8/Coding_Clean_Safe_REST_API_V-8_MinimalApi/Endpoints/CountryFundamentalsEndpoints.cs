namespace Coding_Clean_Safe_REST_API_V_8_MinimalApi.Endpoints;

public static class CountryFundamentalsEndpoints
{
    public static IResult CountryId(int countryId)
    {
        return Results.Ok($"Country Id -> {countryId}");
    }

    public static IResult CountryNames()
    {
        return Results.Ok(new List<string> { "India", "Germany", "Italy", "US" });
    }
}
