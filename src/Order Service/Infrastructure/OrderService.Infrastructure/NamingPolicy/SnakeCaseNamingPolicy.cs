using System.Text.Json;

namespace OrderService.Infrastructure.NamingPolicy;

public class SnakeCaseNamingPolicy : JsonNamingPolicy
{
    public override string ConvertName(string name)
    {
        if (string.IsNullOrEmpty(name))
            return name;

        var buffer = new List<char>(name.Length + 5);
        for (int i = 0; i < name.Length; i++)
        {
            var c = name[i];
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

