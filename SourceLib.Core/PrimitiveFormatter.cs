using System.Globalization;

namespace SourceLib.Core;

public static class PrimitiveFormatter
{
    public static string FormatFloat(double value)
    {
        var text = value.ToString("R", CultureInfo.InvariantCulture);

        if (!text.Contains('.') && !text.Contains('e') && !text.Contains('E'))
        {
            text += ".0";
        }

        return text;
    }
}
