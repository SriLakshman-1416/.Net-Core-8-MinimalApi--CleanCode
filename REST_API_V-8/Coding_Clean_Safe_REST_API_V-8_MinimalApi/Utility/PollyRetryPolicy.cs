﻿using Polly;
using Polly.Extensions.Http;

namespace Coding_Clean_Safe_REST_API_V_8_MinimalApi.Utility;

public static class PollyRetryPolicy
{
    public static void AddFaultHandlingPolicy(this IHttpClientBuilder builder)
    {
        var retryPolicy = HttpPolicyExtensions.HandleTransientHttpError() // Handles 5XX and 408
                                              .WaitAndRetryAsync(3, retryDelayInSeconds => TimeSpan.FromSeconds(3));


        var circuitBreakerPolicy = HttpPolicyExtensions.HandleTransientHttpError() // Handles 5XX and 408
                                                       .CircuitBreakerAsync(4, TimeSpan.FromSeconds(15));


        var policy = retryPolicy.WrapAsync(circuitBreakerPolicy);


        builder.AddPolicyHandler(policy);
    }
}
