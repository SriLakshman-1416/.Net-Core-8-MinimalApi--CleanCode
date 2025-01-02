using Coding_Clean_Safe_REST_API_V_8_MinimalApi.Models;
using Domain.MappingInterface;
using Domain.Models;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace Coding_Clean_Safe_REST_API_V_8_MinimalApi.Endpoints;

public static class CountryCurdOperationEndPoints
{
    public static async Task<IResult> PostCountry([FromBody] Country country, IValidator<Country> validator, ICountryMapper mapper, ICountryService_V2 countryService)
    {
        var validationResult = validator.Validate(country);

        if (validationResult.IsValid)
        {
            var countryDto = mapper.MapModelToDto(country);
            return Results.CreatedAtRoute("countryById", new { Id = await countryService.CreateOrUpdateAsync(countryDto) });
        }
        return Results.ValidationProblem(validationResult.ToDictionary());
    }

    public static async Task<IResult> GetCountry(int id, ICountryMapper mapper, ICountryService_V2 countryService)
    {
        var country = await countryService.RetrieveAsync(id);

        if (country is null)
            return Results.NotFound();

        return Results.Ok(mapper.MapDtoToModel(country));
    }

    public static async Task<IResult> GetCountries(ICountryMapper mapper, ICountryService_V2 countryService)
    {
        var countries = await countryService.GetAllAsync();
        return Results.Ok(mapper.MapListOfDtoToModel(countries));
    }

    public static async Task<IResult> UpdateCountry([FromBody] Country country, IValidator<Country> validator, ICountryMapper mapper, ICountryService_V2 countryService)
    {
        var validationResult = validator.Validate(country);

        if (validationResult.IsValid)
        {
            if (country.Id is null)
                return Results.CreatedAtRoute("countryById", new { Id = await countryService.CreateOrUpdateAsync(mapper.MapModelToDto(country)) });
            return Results.NoContent();
        }
        return Results.ValidationProblem(validationResult.ToDictionary());
    }

    public static async Task<IResult> PatchUpdateCountry(int id, [FromBody] CountryPatch countryPatch, IValidator<CountryPatch> validator,
             ICountryMapper mapper, ICountryService_V2 countryService)
    {
        var validationResult = validator.Validate(countryPatch);

        if (validationResult.IsValid)
        {
            if (await countryService.UpdateDescriptionAsync(id, countryPatch.Description))
                return Results.NoContent();
            return Results.NotFound();
        }
        return Results.ValidationProblem(validationResult.ToDictionary());
    }

    public static async Task<IResult> DeleteCountry(int id, ICountryService_V2 countryService)
    {
        if (await countryService.DeleteAsync(id))
            return Results.NoContent();

        return Results.NotFound();
    }
}
