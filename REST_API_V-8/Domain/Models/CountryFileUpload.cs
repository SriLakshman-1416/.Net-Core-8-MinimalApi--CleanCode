using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models;

public class CountryFileUpload
{
    public IFormFile File { get; set; }

    public string AuthorName { get; set; }
    public string Description { get; set; }
}
