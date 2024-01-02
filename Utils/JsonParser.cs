using Newtonsoft.Json;

namespace dkapi;

public class JsonParser
{
    public static string? Parse<T>(T obj)
    {
        var jsonSetting = new JsonSerializerSettings
        {
            Formatting = Formatting.Indented,
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore
        };
        try
        {
            var json = JsonConvert.SerializeObject(obj, jsonSetting);
            return json;
        }
        catch (JsonException)
        {
            // Handle any potential serialization errors
            return null;
        }
    }
}
