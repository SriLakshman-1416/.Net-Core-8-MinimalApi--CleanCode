using System.ComponentModel.DataAnnotations;

namespace Coding_Clean_Safe_REST_API_V_8_MinimalApi.Models;

public class Country
{
    [Required]
    [RegularExpression("^[a-zA-Z0-9]+$")]
    public string Name { get; set; }

    public string Description { get; set; }

    [Required]
    [RegularExpression("^(https:\\/\\/.)[-a-zA-Z0-9@:%._\\+~#=]{2,256}\\.[a-z]{2,6}\\b([-a-zA-Z0-9@:%_\\+.~#?&//=]*)$")]
    public string FlagUri { get; set; }
}
