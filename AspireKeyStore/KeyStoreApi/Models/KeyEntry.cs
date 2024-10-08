﻿namespace KeyStoreApi.Models;

public class KeyEntry : BaseEntity
{
    public required string Name { get; set; }
    public string? Username { get; set; } = string.Empty;
    public string? Password { get; set; } = string.Empty;
    public string? Description { get; set; } = string.Empty;
}