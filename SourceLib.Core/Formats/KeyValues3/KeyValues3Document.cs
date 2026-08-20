namespace SourceLib.Core.Formats.KeyValues3;

public sealed class KeyValues3Document
{
    public string Header { get; set; } = string.Empty;
    public IReadOnlyList<KeyValues3Pair> Body { get; set; } = [];
}
