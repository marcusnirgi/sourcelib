namespace SourceLib.Core.Formats.KeyValues2;

public sealed class KeyValues2Document
{
    public string Header { get; set; } = string.Empty;
    public IReadOnlyList<KeyValues2Pair> Body { get; set; } = [];
}
