using SourceLib.Core.Engine;

namespace SourceLib.Core.Formats.KeyValues2;

public sealed class KeyValues2Pair : IKeyValuePair
{
    public string Key { get; }
    public EngineValue? Value { get; }
    public string? TypeHint { get; }
    public IReadOnlyList<KeyValues2Pair>? Object { get; }
    public IReadOnlyList<KeyValues2ArrayItem>? Array { get; }

    public KeyValues2Pair(
        string key,
        EngineValue? value,
        string? typeHint = null,
        IReadOnlyList<KeyValues2Pair>? obj = null,
        IReadOnlyList<KeyValues2ArrayItem>? array = null
    )
    {
        Key = key;
        Value = value;
        TypeHint = typeHint;
        Object = obj;
        Array = array;
    }
}

public sealed class KeyValues2ArrayItem
{
    public EngineValue? Value { get; }
    public string? TypeHint { get; }
    public IReadOnlyList<KeyValues2Pair>? Children { get; }
    public IReadOnlyList<KeyValues2ArrayItem>? Array { get; }

    public KeyValues2ArrayItem(
        EngineValue? value,
        string? typeHint = null,
        IReadOnlyList<KeyValues2Pair>? obj = null,
        IReadOnlyList<KeyValues2ArrayItem>? array = null
    )
    {
        Value = value;
        TypeHint = typeHint;
        Children = obj;
        Array = array;
    }
}
