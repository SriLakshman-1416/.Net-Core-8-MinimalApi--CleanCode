using Coding_Clean_Safe_REST_API_V_8_MinimalApi.Models;
using FluentValidation;
using FluentValidation.Results;
using System.Text.RegularExpressions;

namespace Coding_Clean_Safe_REST_API_V_8_MinimalApi.Validator;

public class CountryValidator: AbstractValidator<Country>
{
    public CountryValidator()
    {
        RuleFor(c=> c.Name)
            .NotEmpty()
            .WithMessage("{PropertyName} is Required")
            .Custom((name, context) =>
            {
                Regex rg = new Regex("<.*?>"); // Matches HTML tags
                if (rg.Matches(name).Count > 0)
                {
                    // Raises an error
                    context.AddFailure(
                       new ValidationFailure(
                       "Name",
                       "The parameter has invalid content"
                       )
                    );
                }
            });

        RuleFor(x => x.FlagUri)
        .NotEmpty()
        .WithMessage("{PropertyName} is required")
        .Matches("^(https:\\/\\/.)[-a-zA-Z0-9@:%._\\+~#=]{2,256}\\.[a-z]{2,6}\\b([-a-zA-Z0-9@:%_\\+.~#?&//=]*)$")
        .WithMessage("{PropertyName} must match an HTTPS URL");
    }
}
