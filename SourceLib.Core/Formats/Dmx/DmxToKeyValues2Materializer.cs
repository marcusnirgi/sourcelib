using System.Collections.Immutable;
using SourceLib.Core.Engine;
using SourceLib.Core.Formats.KeyValues2;

namespace SourceLib.Core.Formats.Dmx;

public sealed class DmxToKeyValues2Materializer
{
    public KeyValues2Document Materialize(DmxDocument dmxDocument)
    {
        var header = dmxDocument.Header;

        var headerText =
            $"<!-- dmx encoding keyvalues2 {header.EncodingVersion} "
            + $"format {header.Format} {header.FormatVersion} -->";

        var root = dmxDocument.Elements.First();
        var references = CountReferences(dmxDocument);

        var body = dmxDocument
            .Elements.Where(element =>
                element == root || references.TryGetValue(element.Id, out var count) && count > 1
            )
            .Select(element =>
                MaterializeElement(dmxDocument, element, references, new HashSet<Guid>())
            )
            .ToImmutableList();

        return new KeyValues2Document { Header = headerText, Body = body };
    }

    private KeyValues2Pair MaterializeElement(
        DmxDocument dmxDocument,
        DmxElement element,
        IReadOnlyDictionary<Guid, int> references,
        HashSet<Guid> path
    )
    {
        path = new HashSet<Guid>(path) { element.Id };

        var children = new List<KeyValues2Pair>
        {
            new("id", new EngineGuid(element.Id), KeyValues2TypeHint.ElementId),
            new("name", new EngineString(element.Name), KeyValues2TypeHint.String),
        };

        foreach (var attribute in element.Attributes)
        {
            children.Add(MaterializeAttribute(dmxDocument, attribute, references, path));
        }

        return new KeyValues2Pair(element.ClassName, null, null, children.ToImmutableList());
    }

    private KeyValues2Pair MaterializeAttribute(
        DmxDocument dmxDocument,
        DmxAttribute attribute,
        IReadOnlyDictionary<Guid, int> references,
        HashSet<Guid> path
    )
    {
        switch (attribute.Value)
        {
            case EngineGuid elementReference:
            {
                if (elementReference.Value == Guid.Empty)
                {
                    return new KeyValues2Pair(
                        attribute.Key,
                        elementReference,
                        KeyValues2TypeHint.Element
                    );
                }

                var referencedElement = FindElement(dmxDocument, elementReference.Value);

                if (
                    IsReferencedElsewhere(references, referencedElement.Id)
                    || path.Contains(referencedElement.Id)
                )
                {
                    return new KeyValues2Pair(
                        attribute.Key,
                        elementReference,
                        KeyValues2TypeHint.Element
                    );
                }

                return MaterializeInlineElement(
                    dmxDocument,
                    attribute.Key,
                    referencedElement,
                    references,
                    path
                );
            }

            case EngineArray<EngineGuid> elementArray:
                return new KeyValues2Pair(
                    attribute.Key,
                    null,
                    KeyValues2TypeHint.ElementArray,
                    array: elementArray
                        .Values.Select(value =>
                            MaterializeElementArrayItem(dmxDocument, value, references, path)
                        )
                        .ToImmutableList()
                );

            case EngineArray<EngineInt> values:
                return MaterializeArray(attribute.Key, KeyValues2TypeHint.IntArray, values.Values);

            case EngineArray<EngineFloat> values:
                return MaterializeArray(
                    attribute.Key,
                    KeyValues2TypeHint.FloatArray,
                    values.Values
                );

            case EngineArray<EngineBool> values:
                return MaterializeArray(attribute.Key, KeyValues2TypeHint.BoolArray, values.Values);

            case EngineArray<EngineString> values:
                return MaterializeArray(
                    attribute.Key,
                    KeyValues2TypeHint.StringArray,
                    values.Values
                );

            case EngineArray<EngineTime> values:
                return MaterializeArray(attribute.Key, KeyValues2TypeHint.TimeArray, values.Values);

            case EngineArray<EngineColor4> values:
                return MaterializeArray(
                    attribute.Key,
                    KeyValues2TypeHint.ColorArray,
                    values.Values
                );

            case EngineArray<EngineVector2> values:
                return MaterializeArray(
                    attribute.Key,
                    KeyValues2TypeHint.Vector2Array,
                    values.Values
                );

            case EngineArray<EngineVector3> values:
                return MaterializeArray(
                    attribute.Key,
                    KeyValues2TypeHint.Vector3Array,
                    values.Values
                );

            case EngineArray<EngineVector4> values:
                return MaterializeArray(
                    attribute.Key,
                    KeyValues2TypeHint.Vector4Array,
                    values.Values
                );

            case EngineArray<EngineAngle> values:
                return MaterializeArray(
                    attribute.Key,
                    KeyValues2TypeHint.AngleArray,
                    values.Values
                );

            case EngineArray<EngineQuaternion> values:
                return MaterializeArray(
                    attribute.Key,
                    KeyValues2TypeHint.QuaternionArray,
                    values.Values
                );

            case EngineArray<EngineMatrix> values:
                return MaterializeArray(
                    attribute.Key,
                    KeyValues2TypeHint.MatrixArray,
                    values.Values
                );

            case EngineByteArray values:
                return new KeyValues2Pair(
                    attribute.Key,
                    null,
                    KeyValues2TypeHint.BinaryArray,
                    array: values
                        .Values.Select(value => new KeyValues2ArrayItem(new EngineBytes(value)))
                        .ToImmutableList()
                );

            default:
                return new KeyValues2Pair(
                    attribute.Key,
                    attribute.Value,
                    GetTypeHint(attribute.TypeIndex)
                );
        }
    }

    private KeyValues2Pair MaterializeInlineElement(
        DmxDocument dmxDocument,
        string key,
        DmxElement element,
        IReadOnlyDictionary<Guid, int> references,
        HashSet<Guid> path
    )
    {
        return new KeyValues2Pair(
            key,
            null,
            element.ClassName,
            MaterializeElementChildren(dmxDocument, element, references, path)
        );
    }

    private KeyValues2ArrayItem MaterializeElementArrayItem(
        DmxDocument dmxDocument,
        EngineGuid reference,
        IReadOnlyDictionary<Guid, int> references,
        HashSet<Guid> path
    )
    {
        if (reference.Value == Guid.Empty)
        {
            return new KeyValues2ArrayItem(reference, KeyValues2TypeHint.Element);
        }

        var element = FindElement(dmxDocument, reference.Value);

        if (IsReferencedElsewhere(references, element.Id) || path.Contains(element.Id))
        {
            return new KeyValues2ArrayItem(reference, KeyValues2TypeHint.Element);
        }

        return new KeyValues2ArrayItem(
            null,
            element.ClassName,
            MaterializeElementChildren(dmxDocument, element, references, path)
        );
    }

    private IReadOnlyList<KeyValues2Pair> MaterializeElementChildren(
        DmxDocument dmxDocument,
        DmxElement element,
        IReadOnlyDictionary<Guid, int> references,
        HashSet<Guid> path
    )
    {
        path = new HashSet<Guid>(path) { element.Id };

        var children = new List<KeyValues2Pair>
        {
            new("id", new EngineGuid(element.Id), KeyValues2TypeHint.ElementId),
            new("name", new EngineString(element.Name), KeyValues2TypeHint.String),
        };

        foreach (var attribute in element.Attributes)
        {
            children.Add(MaterializeAttribute(dmxDocument, attribute, references, path));
        }

        return children.ToImmutableList();
    }

    private static KeyValues2Pair MaterializeArray<T>(
        string key,
        string typeHint,
        IReadOnlyList<T> values
    )
        where T : EngineValue
    {
        return new KeyValues2Pair(
            key,
            null,
            typeHint,
            array: values.Select(value => new KeyValues2ArrayItem(value)).ToImmutableList()
        );
    }

    private static IReadOnlyDictionary<Guid, int> CountReferences(DmxDocument dmxDocument)
    {
        var references = new Dictionary<Guid, int>();

        foreach (var element in dmxDocument.Elements)
        {
            foreach (var attribute in element.Attributes)
            {
                switch (attribute.Value)
                {
                    case EngineGuid reference when reference.Value != Guid.Empty:
                        Increment(references, reference.Value);
                        break;

                    case EngineArray<EngineGuid> array:
                        foreach (var reference in array.Values)
                        {
                            if (reference.Value != Guid.Empty)
                                Increment(references, reference.Value);
                        }
                        break;
                }
            }
        }

        return references;
    }

    private static bool IsReferencedElsewhere(IReadOnlyDictionary<Guid, int> references, Guid id)
    {
        return references.TryGetValue(id, out var count) && count > 1;
    }

    private static void Increment(IDictionary<Guid, int> references, Guid id)
    {
        references[id] = references.TryGetValue(id, out var count) ? count + 1 : 1;
    }

    private static DmxElement FindElement(DmxDocument dmxDocument, Guid id)
    {
        var element = dmxDocument.Elements.FirstOrDefault(element => element.Id == id);

        if (element is null)
        {
            throw new InvalidDataException($"DMX element '{id}' was referenced but was not found.");
        }

        return element;
    }

    private static string GetTypeHint(DmxTypeIndex type)
    {
        return type switch
        {
            DmxTypeIndex.ElementRef => KeyValues2TypeHint.Element,
            DmxTypeIndex.Int => KeyValues2TypeHint.Int,
            DmxTypeIndex.Float => KeyValues2TypeHint.Float,
            DmxTypeIndex.Bool => KeyValues2TypeHint.Bool,
            DmxTypeIndex.String => KeyValues2TypeHint.String,
            DmxTypeIndex.Binary => KeyValues2TypeHint.Binary,
            DmxTypeIndex.Time => KeyValues2TypeHint.Time,
            DmxTypeIndex.Color => KeyValues2TypeHint.Color,
            DmxTypeIndex.Vector2 => KeyValues2TypeHint.Vector2,
            DmxTypeIndex.Vector3 => KeyValues2TypeHint.Vector3,
            DmxTypeIndex.Vector4 => KeyValues2TypeHint.Vector4,
            DmxTypeIndex.Angle => KeyValues2TypeHint.Angle,
            DmxTypeIndex.Quaternion => KeyValues2TypeHint.Quaternion,
            DmxTypeIndex.Matrix => KeyValues2TypeHint.Matrix,

            _ => throw new InvalidOperationException($"Expected scalar DMX type, got {type}."),
        };
    }
}
