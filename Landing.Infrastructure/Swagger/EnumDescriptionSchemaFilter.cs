using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.ComponentModel;
using System.Reflection;

public class EnumDescriptionSchemaFilter : ISchemaFilter
{
    public void Apply(OpenApiSchema schema, SchemaFilterContext context)
    {
        if (!context.Type.IsEnum) return;

        var lines = Enum.GetValues(context.Type)
            .Cast<Enum>()
            .Select(value =>
            {
                var member = value.GetType().GetField(value.ToString());
                var descAttr = member?.GetCustomAttribute<DescriptionAttribute>();
                var desc = descAttr?.Description ?? value.ToString();

                
                var formattedName = value.ToString().Replace("_", " ");

                return $"{(int)(object)value} = {formattedName} — {desc}";
            });

        schema.Description = string.Join("\n", lines);
    }
}

