using Domain.Models;
using FluentValidation;
using FluentValidation.Results;
using System.Text.RegularExpressions;

namespace Coding_Clean_Safe_REST_API_V_8_MinimalApi.Validator;

public class CountryFileUploadValidator : AbstractValidator<CountryFileUpload>
{
    public CountryFileUploadValidator()
    {
        RuleFor(x => x.File).Must((file, context) =>
        {
            return file.File.ContentType == "text/csv";
        }).WithMessage("ContentType is not valid");

        RuleFor(x => x.File).Must((file, context) =>
        {
            return file.File.FileName.EndsWith(".csv");
        }).WithMessage("The file extension is not valid");

        RuleFor(x => x.File.FileName).Matches("^[A-Za-z0-9_\\-.]*$").WithMessage("The file name is not valid");

        RuleFor(x => x).Must((file, context) =>
        {
            // string representation of hexadecimal signature of an execute file
            var exeSignatures = new List<string> {
                                  "4D-5A",
                                  "5A 4D"
                               };
            BinaryReader binary = new BinaryReader(file.File.OpenReadStream());
            byte[] bytes = binary.ReadBytes(2); // reading first two bytes
            string fileSequenceHex = BitConverter.ToString(bytes);
            foreach (var exeSignature in exeSignatures)
                if (exeSignature.Equals(fileSequenceHex, StringComparison.OrdinalIgnoreCase))
                    return false;
            return true;
        }).WithName("FileContent")
          .WithMessage("The file content is not valid");

        RuleFor(x => x.AuthorName)
        .NotEmpty()
        .WithMessage("{PropertyName} is required")
        .Custom((authorName, context) =>
        {
            Regex rg = new Regex("<.*?>"); // Matches HTML tags
            if (rg.Matches(authorName).Count > 0)
            {
                // Raises an error
                context.AddFailure(
                   new ValidationFailure(
                      "AuthorName",
                      "The AuthorName parameter has invalid content"));
            }
        });

        RuleFor(x => x.Description)
        .NotEmpty()
        .WithMessage("{PropertyName} is required")
        .Custom((name, context) =>
        {
            Regex rg = new Regex("<.*?>"); // Matches HTML tags
            if (rg.Matches(name).Count > 0)
            {
                // Raises an error
                context.AddFailure(new
                   ValidationFailure(
                      "Name",
                      "The AuthorName parameter has invalid content"));
            }
        });
    }
}