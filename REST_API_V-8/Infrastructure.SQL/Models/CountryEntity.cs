﻿namespace Infrastructure.SQL.Models;

public class CountryEntity
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
    public string? FlagUri { get; set; }
}
