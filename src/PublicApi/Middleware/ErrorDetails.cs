﻿namespace Microsoft.eShopWeb.PublicApi.Middleware;

internal class ErrorDetails
{
    public ErrorDetails()
    {
    }

    public int StatusCode { get; set; }
    public string Message { get; set; }
}