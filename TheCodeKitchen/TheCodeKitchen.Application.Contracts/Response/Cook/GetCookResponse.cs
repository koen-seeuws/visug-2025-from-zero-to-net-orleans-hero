namespace TheCodeKitchen.Application.Contracts.Response.Cook;

[GenerateSerializer]
public record GetCookResponse(string Username, string PasswordHash);