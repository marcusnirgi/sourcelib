namespace SourceLib.Core.Formats.DMX;

public enum DmxHeaderBinaryEncodingVersion : int
{
    V1 = 1,
    V2 = 2,
    V3 = 3,
    V4 = 4,
    V5 = 5,
}

public sealed class DmxHeader
{
    public string Encoding { get; }
    public int EncodingVersion { get; }
    public string Format { get; }
    public int FormatVersion { get; }

    public DmxHeader(string encoding, int encodingVersion, string format, int formatVersion)
    {
        Encoding = encoding;
        EncodingVersion = encodingVersion;
        Format = format;
        FormatVersion = formatVersion;
    }

    public static DmxHeader FromString(string value)
    {
        const string prefix = "<!--";
        const string suffix = "-->";

        if (!value.StartsWith(prefix) || !value.EndsWith(suffix))
            throw new FormatException($"Invalid DMX header: '{value}'");

        var parts = value[prefix.Length..^suffix.Length]
            .Trim()
            .Split(' ', StringSplitOptions.RemoveEmptyEntries);

        if (
            parts.Length != 7
            || !parts[0].Equals("dmx", StringComparison.OrdinalIgnoreCase)
            || !parts[1].Equals("encoding", StringComparison.OrdinalIgnoreCase)
            || !parts[4].Equals("format", StringComparison.OrdinalIgnoreCase)
        )
        {
            throw new FormatException($"Invalid DMX header: '{value}'");
        }

        if (
            !int.TryParse(parts[3], out var encodingVersion)
            || !int.TryParse(parts[6], out var formatVersion)
        )
        {
            throw new FormatException($"Invalid DMX header: '{value}'");
        }

        return new DmxHeader(parts[2], encodingVersion, parts[5], formatVersion);
    }
}
