using System.Reflection;
using Serilog.Core;
using Serilog.Events;

namespace TheCodeKitchen.Infrastructure.Logging.Serilog;

public sealed class MaskPasswordPolicy : IDestructuringPolicy
{
    private const string DefaultMaskValue = "******";

    public bool TryDestructure(object value, ILogEventPropertyValueFactory propertyValueFactory,
        out LogEventPropertyValue result)
    {
        var props = value.GetType().GetTypeInfo().DeclaredProperties;
        
        var logEventProperties = props.Select(propertyInfo =>
        {
            var targetValue = propertyInfo.Name.Contains("Password", StringComparison.OrdinalIgnoreCase)
                ? DefaultMaskValue
                : propertyInfo.GetValue(value);
            var propertyValue = propertyValueFactory.CreatePropertyValue(targetValue);
            return new LogEventProperty(propertyInfo.Name, propertyValue);
        });

        result = new StructureValue(logEventProperties);

        return true;
    }
}