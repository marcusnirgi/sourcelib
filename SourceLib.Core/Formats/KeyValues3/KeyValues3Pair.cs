using SourceLib.Core.Engine;

namespace SourceLib.Core.Formats.KeyValues3;

public sealed class KeyValues3Pair : IKeyValuePair
{
    public string Key { get; }
    public EngineValue? Value { get; }
    public IReadOnlyList<KeyValues3Pair>? Object { get; }
    public IReadOnlyList<KeyValues3ArrayValue>? Array { get; }

    public KeyValues3Pair(
        string key,
        EngineValue? value = null,
        IReadOnlyList<KeyValues3Pair>? obj = null,
        IReadOnlyList<KeyValues3ArrayValue>? array = null
    )
    {
        Key = key;
        Value = value;
        Object = obj;
        Array = array;
    }
}

public sealed class KeyValues3ArrayValue
{
    public EngineValue? Value { get; }
    public IReadOnlyList<KeyValues3Pair>? Children { get; }
    public IReadOnlyList<KeyValues3ArrayValue>? Array { get; }

    public KeyValues3ArrayValue(
        EngineValue? value,
        IReadOnlyList<KeyValues3Pair>? children = null,
        IReadOnlyList<KeyValues3ArrayValue>? array = null
    )
    {
        Value = value;
        Children = children;
        Array = array;
    }
}
