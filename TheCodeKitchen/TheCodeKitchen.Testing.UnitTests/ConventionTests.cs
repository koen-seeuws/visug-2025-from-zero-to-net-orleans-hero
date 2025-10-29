using FluentAssertions;
using TheCodeKitchen.Application.Contracts.Requests.Game;
using TheCodeKitchen.Application.Contracts.Response.Game;

namespace TheCodeKitchen.Testing.UnitTests;

public sealed class ConventionTests
{
    [Theory]
    [InlineData(typeof(CreateGameRequest), "TheCodeKitchen.Application.Contracts.Requests",
        typeof(GenerateSerializerAttribute), "Orleans")]
    [InlineData(typeof(GetGameResponse), "TheCodeKitchen.Application.Contracts.Responses",
        typeof(GenerateSerializerAttribute), "Orleans")]
    public void AllTypesInNamespaceShouldHaveAttributeApplied(Type typeFromAssembly, string typesNamespace,
        Type attribute, string? namespaceExclusion = null)
    {
        // Act
        var typesWithoutAttribute = typeFromAssembly
            .Assembly
            .GetTypes()
            .Where(type => type.IsClass)
            .Where(type => type.Namespace?.Contains(typesNamespace, StringComparison.OrdinalIgnoreCase) ?? false)
            .Where(type => !type.GetCustomAttributes(attribute, inherit: false).Any())
            .ToArray();

        if (!string.IsNullOrWhiteSpace(namespaceExclusion))
            typesWithoutAttribute = typesWithoutAttribute
                .Where(type =>
                    !type.Namespace?.Contains(namespaceExclusion, StringComparison.OrdinalIgnoreCase) ?? true)
                .ToArray();
        
        var typeWithoutAttributeNames = typesWithoutAttribute.Select(type => type.FullName).ToArray();

        // Assert
        typeWithoutAttributeNames
            .Should()
            .BeEmpty("all types in {0} should be decorated with [{1}]", typesNamespace,
                attribute.Name.Replace("Attribute", string.Empty));
    }
}