namespace LostTech.Stack.Widgets.DataBinding;

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows.Data;
using System.Windows.Markup;

public class ConverterPipeline : List<ParametrizedConverter>, IValueConverter {
    public object? Convert(object? value, Type targetType, object parameter, CultureInfo culture)
        => this.Aggregate(value, (current, converter) => converter.Converter?.Convert(current, targetType, converter.Parameter, culture));

    public object? ConvertBack(object? value, Type targetType, object parameter, CultureInfo culture)
        => this.Reversed()
               .Aggregate(value, (current, converter) => converter.Converter?.ConvertBack(current, targetType, converter.Parameter, culture));

    IEnumerable<ParametrizedConverter> Reversed() {
        for (int i = this.Count - 1; i >= 0; i--)
            yield return this[i];
    }
}

[ContentProperty(nameof(Converter))]
public sealed class ParametrizedConverter {
    public IValueConverter? Converter { get; set; }
    public object? Parameter { get; set; }
}