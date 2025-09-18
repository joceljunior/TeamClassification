using System.ComponentModel;
using System.Globalization;

namespace TeamClassification.Converters
{
    public class TimeSpanConverter : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext? context, Type sourceType)
        {
            return sourceType == typeof(string) || base.CanConvertFrom(context, sourceType);
        }

        public override object? ConvertFrom(ITypeDescriptorContext? context, CultureInfo? culture, object value)
        {
            if (value is string stringValue)
            {
                if (TimeSpan.TryParse(stringValue, out TimeSpan result))
                {
                    return result;
                }
            }
            return base.ConvertFrom(context, culture, value);
        }
    }
}
