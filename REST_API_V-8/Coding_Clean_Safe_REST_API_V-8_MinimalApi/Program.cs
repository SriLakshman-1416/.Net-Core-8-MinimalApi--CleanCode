using Asp.Versioning;
using Asp.Versioning.Conventions;
using BusinessLogicLayer.Services;
using Coding_Clean_Safe_REST_API_V_8_MinimalApi.Endpoints;
using Coding_Clean_Safe_REST_API_V_8_MinimalApi.Enums;
using Coding_Clean_Safe_REST_API_V_8_MinimalApi.GroupEndpoints;
using Coding_Clean_Safe_REST_API_V_8_MinimalApi.Interfaces;
using Coding_Clean_Safe_REST_API_V_8_MinimalApi.Middlewares;
using Coding_Clean_Safe_REST_API_V_8_MinimalApi.Models;
using Coding_Clean_Safe_REST_API_V_8_MinimalApi.Services;
using Coding_Clean_Safe_REST_API_V_8_MinimalApi.Utility;
using Domain.CustomParamModels;
using Domain.MappingInterface;
using Domain.MappingService;
using Domain.Models;
using Domain.Repositories;
using FluentValidation;
using Infrastructure.SQL.Database;
using Infrastructure.SQL.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Refit;
using System.Diagnostics;
using System.Net;
using System.Threading.RateLimiting;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle

#region Sql Server Conn String Config
var dbConnection = builder.Configuration.GetConnectionString("DemoDb");
builder.Services.AddDbContextPool<DemoContext>(options => options.UseSqlServer(dbConnection,
    sqlServerOptionsAction: sqlOptions => { sqlOptions.EnableRetryOnFailure(maxRetryCount: 3); }));

#endregion

#region Service Registration Black

#region Services

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddValidatorsFromAssemblyContaining<Program>();
builder.Services.AddScoped<ICountryMapper, CountryMapper>();
builder.Services.AddScoped<ICountryService, CountryService>();
builder.Services.AddScoped<IStreamingService, StreamingService>();
builder.Services.AddScoped<IPricingTierService, PricingTierService>();
builder.Services.AddScoped<ICountryRepository, CountryRepository>();
builder.Services.AddScoped<ICountryService_V2, CountryService_V2>();
builder.Services.AddHttpClient();
builder.Services.AddRefitClient<IMediaRepository>()
                .ConfigureHttpClient(c => c.BaseAddress = new Uri("https://anthonygiretti.blob.core.windows.net"))
                .AddFaultHandlingPolicy();

#endregion

#region Rate Limiting

#region Fixed Window
#region Fixed Window Model
builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = (int)HttpStatusCode.TooManyRequests;
    options.OnRejected = async (context, token) =>
    {
        await context.HttpContext.Response.WriteAsync("Too many requests. Please try again later.");
    };
    options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(httpContext =>
      RateLimitPartition.GetFixedWindowLimiter(partitionKey: httpContext.Connection.RemoteIpAddress.ToString(),
          factory: _ => new FixedWindowRateLimiterOptions
          {
              QueueLimit = 10,
              PermitLimit = 50,
              Window = TimeSpan.FromSeconds(15)
          }));
});
#endregion

#region Fixed Window Model - The global rate limiter and the ShortLimit policy combined 

builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = (int)HttpStatusCode.TooManyRequests;
    options.OnRejected = async (context, token) =>
    {
        await context.HttpContext.Response.WriteAsync("Too many requests. Please try again later.");
    };
    options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(httpContext =>
    RateLimitPartition.GetFixedWindowLimiter(partitionKey: httpContext.Connection.RemoteIpAddress.ToString(),
        factory: _ => new FixedWindowRateLimiterOptions
        {
            QueueLimit = 10,
            PermitLimit = 50,
            Window = TimeSpan.FromSeconds(15)
        }));

    options.AddPolicy(policyName: "ShortLimit", context =>
    {
        return RateLimitPartition.GetFixedWindowLimiter(context.Connection.RemoteIpAddress.ToString(),
      _ => new FixedWindowRateLimiterOptions
      {
          PermitLimit = 10,
          Window = TimeSpan.FromSeconds(15)
      });
    });
});

#endregion

#region Fixed Window Model - The global rate limiter updated with the pricing tier (based on user IP Address)

builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = (int)HttpStatusCode.TooManyRequests;
    options.OnRejected = async (context, token) =>
    {
        await context.HttpContext.Response.WriteAsync("Too many requests. Please try again later.");
    };
    options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(httpContext =>
    {
        var priceTierService = httpContext.RequestServices.GetRequiredService<IPricingTierService>();
        var ip = httpContext.Connection.RemoteIpAddress.ToString();
        var priceTier = priceTierService.GetPricingTier(ip);

        return priceTier switch
        {
            PricingTier.Paid => RateLimitPartition.GetFixedWindowLimiter(
                ip,
                _ => new FixedWindowRateLimiterOptions
                {
                    QueueLimit = 10,
                    PermitLimit = 50,
                    Window = TimeSpan.FromSeconds(15)
                }),
            PricingTier.Free => RateLimitPartition.GetFixedWindowLimiter(
                ip,
                _ => new FixedWindowRateLimiterOptions
                {
                    PermitLimit = 1,
                    Window = TimeSpan.FromSeconds(15)
                })
        };
    });
});

#endregion

#endregion

#region Sliding Window Model

#region The global rate limiter set with the Sliding window model

builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = (int)HttpStatusCode.TooManyRequests;
    options.OnRejected = async (context, token) =>
    {
        await context.HttpContext.Response.WriteAsync("Too many requests. Please try again later.");
    };

    options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(httpContext =>
    {
        var priceTierService = httpContext.RequestServices.GetRequiredService<IPricingTierService>();
        var ip = httpContext.Connection.RemoteIpAddress.ToString();
        var priceTier = priceTierService.GetPricingTier(ip);

        return priceTier switch
        {
            PricingTier.Paid => RateLimitPartition.GetSlidingWindowLimiter(
                ip,
                _ => new SlidingWindowRateLimiterOptions
                {
                    QueueLimit = 10,
                    PermitLimit = 50,
                    SegmentsPerWindow = 2,
                    Window = TimeSpan.FromSeconds(15)
                }),
            PricingTier.Free => RateLimitPartition.GetSlidingWindowLimiter(
                ip,
                _ => new SlidingWindowRateLimiterOptions
                {
                    PermitLimit = 2,
                    SegmentsPerWindow = 2,
                    Window = TimeSpan.FromSeconds(15)
                })
        };
    });
});

#endregion



#endregion


#region The Token Bucket Model

/*
   *  TokenLimit: Defines the maximum available tokens

   *  TokensPerPeriod: Defines the replenished number of tokens per period

   *  ReplenishmentPeriod: Period where tokens will get replenished
  
 */

#region The global rate limiter set with the Token bucket model

builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = (int)HttpStatusCode.TooManyRequests;
    options.OnRejected = async (context, token) =>
    {
        await context.HttpContext.Response.WriteAsync("Too many requests. Please try again later.");
    };

    options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(httpContext =>
    {
        var priceTierService = httpContext.RequestServices.GetRequiredService<IPricingTierService>();
        var ip = httpContext.Connection.RemoteIpAddress.ToString();
        var priceTier = priceTierService.GetPricingTier(ip);

        return priceTier switch
        {
            PricingTier.Paid => RateLimitPartition.GetTokenBucketLimiter(
                ip,
                _ => new TokenBucketRateLimiterOptions
                {
                    TokenLimit = 50,
                    TokensPerPeriod = 25,
                    ReplenishmentPeriod = TimeSpan.FromSeconds(15)
                }),
            PricingTier.Free => RateLimitPartition.GetTokenBucketLimiter(
                ip,
                _ => new TokenBucketRateLimiterOptions
                {
                    TokenLimit = 10,
                    TokensPerPeriod = 5,
                    ReplenishmentPeriod = TimeSpan.FromSeconds(15)
                })
        };
    });
});

#endregion


#endregion


#region The Concurrency Model

#region The global rate limiter set with the Concurrency model
builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = (int)HttpStatusCode.TooManyRequests;
    options.OnRejected = async (context, token) =>
    {
        await context.HttpContext.Response.WriteAsync("Too many requests. Please try again later.");
    };

    options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(httpContext =>
    {
        var priceTierService = httpContext.RequestServices.GetRequiredService<IPricingTierService>();
        var ip = httpContext.Connection.RemoteIpAddress.ToString();
        var priceTier = priceTierService.GetPricingTier(ip);

        return priceTier switch
        {
            PricingTier.Paid => RateLimitPartition.GetConcurrencyLimiter(
                ip,
                _ => new ConcurrencyLimiterOptions
                {
                    QueueLimit = 10,
                    PermitLimit = 50
                }),
            PricingTier.Free => RateLimitPartition.GetConcurrencyLimiter(
                ip,
                _ => new ConcurrencyLimiterOptions
                {
                    QueueLimit = 0,
                    PermitLimit = 10
                })
        };
    });
});
#endregion

#endregion

#endregion

#region Cors
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        builder =>
        {
            builder.AllowAnyHeader()
                   .AllowAnyMethod()
                   .AllowAnyOrigin();
        });

    #region Restricted Cors
    //options.AddPolicy("Restricted",
    //    builder =>
    //    {
    //        builder.AllowAnyHeader()
    //               .WithMethods("GET", "POST", "PUT", "DELETE")
    //               .WithOrigins("https://mydomain.com", "https://myotherdomain.com")
    //               .AllowCredentials();
    //    });
    #endregion
});
#endregion

#region Api verioning Configuration
builder.Services.AddApiVersioning(options =>
{
    options.DefaultApiVersion = new ApiVersion(1, 0);
    options.ReportApiVersions = true;
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.ApiVersionReader = new HeaderApiVersionReader("api-version");
});
#endregion

#endregion

var app = builder.Build();

#region Sql Server Conn string Config

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<DemoContext>();
    db.Database.SetConnectionString(dbConnection);
    db.Database.Migrate();
}
#endregion

app.UseCors("AllowAll");

var versionSet = app.NewApiVersionSet()
                    .HasApiVersion(1.0)
                    .HasApiVersion(2.0)
                    .Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

#region Fundamental Concepts EndPoints
app.MapGet("/weatherforecast", () =>
{
    var forecast = Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
})
.WithName("GetWeatherForecast")
.WithOpenApi();


app.MapGroup("/countriesgroup").GroupCountries();

app.MapGet("/countries/{countryId}", CountryFundamentalsEndpoints.CountryId);

app.MapGet("/countries", CountryFundamentalsEndpoints.CountryNames);

app.MapMethods("users/{userId}", new List<string> { "PUT", "PATCH" }, (int userId, HttpRequest request) =>
{
    var id = request.RouteValues["id"];
    var lastActivationDate = request.Form["lastactivatedate"];
});

app.MapPost("/Addresses", ([FromBody] Address address) =>
{
    return Results.Created();
});

app.MapPut("/Addresses/{addressId}", ([FromRoute] int addressId, [FromForm] Address address) =>
{
    return Results.NoContent();
}).DisableAntiforgery();

app.MapPut("/AddressesPutMethod/{addressId}", ([FromForm] Address address) =>
{
    return Results.NoContent();
}).DisableAntiforgery();

// Represents ?id=1&id=2
app.MapGet("/Ids", ([FromQuery] int[] id) =>
{
    return Results.Ok();
});

app.MapGet("/Languages", ([FromHeader(Name = "lng")] string[] lng) =>
{
    return Results.Ok();
});

#endregion


#region Country EndPoints With Fluent Validation

app.MapPost("/countrieswithvalidation", CountryWithFluentValidationEndPoints.CountryWithValidation);
#endregion


#region Model Object to Dto Mapping Sample End Point
app.MapPost("/countriesMapping", ([FromBody] Country country, IValidator<Country> validator, ICountryMapper mapper) =>
{
    var validationResult = validator.Validate(country);

    if (validationResult.IsValid)
    {
        var countryDto = mapper.MapModelToDto(country);

        //Do some work here
        return Results.Created();
    }
    return Results.ValidationProblem(validationResult.ToDictionary());
});


#endregion


#region Countries CRUD Operation Operation With Static End points Class
//// Create
//app.MapPost("/countriescurdoperation", CountryCurdOperationEndPoints.PostCountry);

//// Retrieve
//app.MapGet("/countriescurdoperation/{id}", CountryCurdOperationEndPoints.GetCountry).WithName("countryById");

//// Retrieve
//app.MapGet("/countriescurdoperation", CountryCurdOperationEndPoints.GetCountries);

//// Update
//app.MapPut("/countriescurdoperation", CountryCurdOperationEndPoints.UpdateCountry);

//// Update
//app.MapPatch("/countriescurdoperation/{id}", CountryCurdOperationEndPoints.PatchUpdateCountry);

//// Delete
//app.MapDelete("/countriescurdoperation/{id}", CountryCurdOperationEndPoints.DeleteCountry);

// Countries Routes Moved to Separate Class Below
app.AddCountryEndpoints();

#endregion


#region File Upload Process Endpoint
app.MapGet("/countries/download", (
           ICountryService countryService) =>
{

    (
        byte[] fileContent,
        string mimeType,
        string fileName) = countryService.GetFile();

    if (fileContent is null || mimeType is null)
        return Results.NotFound();

    return Results.File(fileContent, mimeType, fileName);
});


app.MapPost("/countries/upload", (IFormFile file) =>
{
    return Results.Created();
});

app.MapPost("/countries/uploadmany", (IFormFileCollection files) =>
{
    return Results.Created();
});

#region File Upload With Meta Data

app.MapPost("/countries/uploadwithmetadata", ([FromForm] CountryMetaData countryMetaData, IFormFile file) =>
{
    return Results.Created();
}).DisableAntiforgery();

app.MapPost("/countries/uploadmanywithmetadata", ([FromForm] CountryMetaData countryMetaData, IFormFileCollection files) =>
{
    return Results.Created();
}).DisableAntiforgery();


#endregion

#endregion


#region Stream End Point

app.MapGet("/streaming", async (IStreamingService streamingService) =>
{
    (Stream stream, string mimeType) = await streamingService.GetFileStream();
    return Results.Stream(stream, mimeType, enableRangeProcessing: true);
});

#endregion


#region Custom Parameter Binding End point 

app.MapGet("/countriesforcustomparambinding/ids", ([FromHeader] CountryIds ids) =>
{
    Results.NoContent();
});

app.MapPost("/countriesforcustomparambinding/upload", (IFormFile file, CountryWithCustomParamBinding country) =>
{
    Results.NoContent();
}).DisableAntiforgery();

#endregion


#region Api version Endpoint
app.MapGroup("/v1").GroupVersion1();
app.MapGroup("/v2").GroupVersion2();
#endregion


#region Action Filter Sample End points

app.MapGet("/longrunning", async () =>
{
    await Task.Delay(5000);
    return Results.Ok();
}).AddEndpointFilter(async (filterContext, next) =>
{
    long startTime = Stopwatch.GetTimestamp();
    var result = await next(filterContext);
    TimeSpan elapsedTime = Stopwatch.GetElapsedTime(startTime);
    app.Logger.LogInformation($"GET /longrunning endpoint took {elapsedTime.TotalSeconds} to execute");
    return result;
});

app.MapGet("/longrunningV2", async () =>
{
    await Task.Delay(5000);
    return Results.Ok();
}).AddEndpointFilter<LogPerformanceFilter>();


app.MapPost("/countries", ([FromBody] Country country) =>
{

    return Results.CreatedAtRoute("countryById", new { Id = 1 });
}).AddEndpointFilter<InputValidatorFilter<Country>>();


#endregion

#region IAsyncEnumerable - JSON Streaming Sample End point

app.MapGet("/countriesWithAsyncEnumerable", async (
           int? pageIndex,
           int? pageSize,
           ICountryMapper mapper,
           ICountryService_V2 countryService) => await Task.Run(() =>
           {
               async IAsyncEnumerable<Country> StreamCountriesAsync()
               {
                   var countries = await countryService
                   .GetAllAsync();
                   var mappedCountries = mapper.MapListOfDtoToModel(countries);
                   foreach (var country in mappedCountries)
                   {
                       yield return country;
                   }
               }
               return StreamCountriesAsync();
           }));
#endregion


#region Rate Limiting

#region Fixed Window Model - Disabling the global Rate Limiting feature with the DisableRateLimiting extension method

app.MapGet("/notlimited", () => { return Results.Ok(); }).DisableRateLimiting();

#endregion

#region Fixed Window Model - The RequireRateLimiting extension method

app.MapGet("/limited", () => { return Results.Ok(); }).RequireRateLimiting("ShortLimit");

#endregion

#endregion

#region Middelware and Sample End point

#region // 1. The GET /test endpoint positioned before the MapWhen middleware

app.MapGet("/test", () =>
{
    return Results.Ok("Test endpoint has been executed");
});

app.MapWhen(ctx => !string.IsNullOrEmpty(ctx.Request.Query["q"].ToString()),
builder =>
{
    builder.Use(async (context, next) =>
    {
        app.Logger.LogInformation("New middleware pipeline has been invoked");
        await next();
    });
    builder.Run(async context =>
    {
        app.Logger.LogInformation("New pipeline initiated will end here");
        await Task.CompletedTask;
    });
});// Stops the execution if the condition is met because the new branch contains Run Middleware
#endregion


#region // 2.The Map middleware

app.Map(new PathString("/test"),
builder =>
{
    builder.Use(async (context, next) =>
    {
        app.Logger.LogInformation("New middleware pipeline branch has been initiated");
        await next();
    });
    builder.Run(async context =>
    {
        app.Logger.LogInformation("New middleware pipeline will end here");
        await Task.CompletedTask;
    });
});// Stops execution

#endregion

#region // 3. The GET /test endpoint positioned between the Use middlewares and before the UseWhen middleware


app.Use(async (context, next) =>
{
    app.Logger.LogInformation("Middleware 1 executed");
    await next();
});

app.MapGet("/testV2", () =>
{
    app.Logger.LogInformation("Endpoint GET /test has been invoked");
    return Results.Ok();
});

app.Use(async (context, next) =>
{
    app.Logger.LogInformation("Middleware 2 executed");
    await next();
});

app.UseWhen(ctx => !string.IsNullOrEmpty(ctx.Request.Query["p"].ToString()),
builder =>
{
    builder.Use(async (context, next) =>
    {
        app.Logger.LogInformation("Nested middleware executed");
        await next();
    });

    builder.Run(async (context) =>
    {
        app.Logger.LogInformation("End of the pipeline end");
        await Task.CompletedTask;
    });

});
// Stops the execution if the condition is met because the UseWhen contains Run Middleware

#endregion


#region // 4. The UseWhen middleware without its nested Run middleware

app.UseWhen(ctx => !string.IsNullOrEmpty(ctx.Request.Query["p"].ToString()),
builder =>
{
    builder.Use(async (context, next) =>
    {
        app.Logger.LogInformation("Nested middleware executed");
        await next();
    });
});

#endregion


#region // 5. The LoggingMiddleware registered in the pipeline with the UseMiddleware middleware

app.Use(async (context, next) =>
{
    app.Logger.LogInformation("Middleware 1 executed");
    await next();
});// Don't stop the execution

app.MapGet("/testV3", () =>
{
    app.Logger.LogInformation("Endpoint GET /test has been invoked");
    return Results.Ok();
});

app.UseMiddleware<LoggingMiddleware>();// Doesn't stop the execution


#endregion


#endregion

app.Run();

internal record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
