using Coding_Clean_Safe_REST_API_V_8_MinimalApi.Models;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Coding_Clean_Safe_REST_API_V_8_MinimalApi.Endpoints;

public static class CountryWithFluentValidationEndPoints
{
    public static IResult CountryWithValidation([FromBody] Country country, IValidator<Country> validator)
    {
        var validationResult = validator.Validate(country);

        if (validationResult.IsValid)
        {
            //Do something
            return Results.Created();
        }
        return Results.ValidationProblem(validationResult.ToDictionary(), statusCode: (int)HttpStatusCode.BadRequest);
    }
}
