﻿namespace Domain.Models;

public class Response
{
    public int StatusCode { get; set; }
    public string Message { get; set; } = string.Empty;
}