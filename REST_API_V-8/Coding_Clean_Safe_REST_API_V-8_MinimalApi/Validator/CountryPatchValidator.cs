using Domain.Models;
using FluentValidation;
using FluentValidation.Results;
using System.Text.RegularExpressions;

namespace Coding_Clean_Safe_REST_API_V_8_MinimalApi.Validator;

    public class CountryPatchValidator : AbstractValidator<CountryPatch>
    {
        public CountryPatchValidator()
        {
            RuleFor(x => x.Description)
            .NotEmpty()
            .WithMessage("{ParameterName} cannot be empty")
            .Custom((name, context) =>
            {
                Regex rg = new Regex("<.*?>"); // Matches HTML tags
                if (rg.Matches(name).Count > 0)
                {
                    // Raises an error
                    context.AddFailure(new ValidationFailure(
                "Description",
                "The description has invalid content")
                );
                }
            });
        }
    }