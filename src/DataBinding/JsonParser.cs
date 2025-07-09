namespace LostTech.Stack.Widgets.DataBinding;

using System.Dynamic;
using System.Globalization;
using System.Text;
using System.Windows.Data;
using Newtonsoft.Json;

public sealed class JsonParser : IValueConverter {
    public static object? GetObject(string json, string? path) {
        dynamic? dynamicObject = JsonConvert.DeserializeObject<ExpandoObject>(json);
        if (path is not null) {
            var currentPath = new StringBuilder();
            foreach (string part in SplitPath(path)) {
                currentPath.Append(part);
                if (dynamicObject is null)
                    throw new NullReferenceException($"Object '{currentPath}' is null");

                if (part.StartsWith('[') && part.EndsWith(']')
                    && int.TryParse(part[1..^1], NumberStyles.Integer, CultureInfo.InvariantCulture, out int index)) {
                    dynamicObject = ((IEnumerable<object>)dynamicObject).ElementAt(index);
                } else {
                    dynamicObject = ((IDictionary<string, object>)dynamicObject)[part];
                }
                currentPath.Append('.');
            }
        }
        return dynamicObject;
    }

    static string[] SplitPath(string path) {
        var result = new List<string>();
        var currentPart = new StringBuilder();

        for (int i = 0; i < path.Length; i++) {
            if (path[i] == '.') {
                // Check if this is an escaped dot (double dot)
                if (i + 1 < path.Length && path[i + 1] == '.') {
                    // This is an escaped dot, add a single dot to the current part
                    currentPart.Append('.');
                    i++; // Skip the next dot
                } else {
                    // This is a path separator, finish the current part
                    result.Add(currentPart.ToString());
                    currentPart.Clear();
                }
            } else {
                currentPart.Append(path[i]);
            }
        }

        // Add the last part if there's anything
        if (currentPart.Length > 0) {
            result.Add(currentPart.ToString());
        }

        return [.. result];
    }

    public object? Convert(object value, Type targetType, object parameter, CultureInfo culture) {
        return value switch {
            null => null,
            string json => GetObject(json, parameter as string),
            _ => throw new NotSupportedException(),
        };
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();
}
