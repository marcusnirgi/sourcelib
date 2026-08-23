using SourceLib.Core.Engine;

namespace SourceLib.Core;

public interface IKeyValuePair
{
    string Key { get; }
    EngineValue? Value { get; }
}
