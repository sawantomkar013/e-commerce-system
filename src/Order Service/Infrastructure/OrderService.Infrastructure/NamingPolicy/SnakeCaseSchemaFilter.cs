using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace OrderService.Infrastructure.NamingPolicy;

public class SnakeCaseSchemaFilter : ISchemaFilter
{
    public void Apply(OpenApiSchema schema, SchemaFilterContext context)
    {
        if (schema.Properties != null)
        {
            var keys = schema.Properties.Keys.ToList();
            foreach (var key in keys)
            {
                var value = schema.Properties[key];
                schema.Properties.Remove(key);
                schema.Properties.Add(ToSnakeCase(key), value);
            }
        }
    }

    private static string ToSnakeCase(string input)
    {
        if (string.IsNullOrEmpty(input))
            return input;

        var buffer = new List<char>(input.Length + 5);
        for (int i = 0; i < input.Length; i++)
        {
            var c = input[i];
            if (char.IsUpper(c))
            {
                if (i > 0)
                    buffer.Add('_');
                buffer.Add(char.ToLowerInvariant(c));
            }
            else
            {
                buffer.Add(c);
            }
        }
        return new string(buffer.ToArray());
    }
}

