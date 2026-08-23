namespace SourceLib.Core.Engine;

public static class EngineValueInference
{
    public static EngineValue FromString(string value)
    {
        if (value.Length == 0)
            return new EngineString(value);

        if (int.TryParse(value, out var intValue))
            return new EngineInt(intValue);

        if (float.TryParse(value, out var floatValue))
            return new EngineFloat(floatValue);

        if (bool.TryParse(value, out var boolValue))
            return new EngineBool(boolValue);

        return new EngineString(value);
    }
}
