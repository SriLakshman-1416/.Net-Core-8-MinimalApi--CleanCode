﻿namespace Coding_Clean_Safe_REST_API_V_8_MinimalApi.GroupEndpoints;

public static class ApiVersionGroup
{
    public static RouteGroupBuilder GroupVersion1(this RouteGroupBuilder group)
    {
        group.MapGet("/version", () => $"Hello version 1");
        return group;
    }

    public static RouteGroupBuilder GroupVersion2(this RouteGroupBuilder group)
    {
        group.MapGet("/version", () => $"Hello version 2");
        group.MapGet("/version2only", () => $"Hello version 2 only");
        return group;
    }
}
