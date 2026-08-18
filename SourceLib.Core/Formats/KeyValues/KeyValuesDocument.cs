namespace SourceLib.Core.Formats.KeyValues;

public sealed class KeyValuesDocument
{
    public IReadOnlyList<string> Macros { get; set; } = [];
    public IReadOnlyList<KeyValuePair> Body { get; set; } = [];
}
