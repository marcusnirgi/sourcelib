namespace SourceLib.Core;

public interface IKeyValuePair
{
    string Key { get; }
    ValuePrimitive Value { get; }
}
