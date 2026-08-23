using SourceLib.Core.Engine;
using SourceLib.Core.Formats.KeyValues3;
using SourceLib.Tests.GameData;

namespace SourceLib.Tests.Formats;

[Collection("Game")]
public class KeyValues3FormatTests
{
    private readonly IGameDataProvider _games;

    public KeyValues3FormatTests(GameDataFixture games)
    {
        _games = games.Provider;
    }

    [Fact]
    public void Test_Parses_Model()
    {
        var fixturePath = TestFixtures.GetPath("kv3", "omni.vmdl");
        var fixtureContent = File.ReadAllText(fixturePath);
        var parser = new KeyValues3FormatParser();
        var document = parser.Parse(fixtureContent);
        Console.WriteLine(document);
        var rootNode = document.Body.FirstOrDefault(pair => pair.Key == "rootNode");
        Assert.NotNull(rootNode);
        Assert.NotNull(rootNode.Object);
        var rootNodeChildren = rootNode.Object.FirstOrDefault(pair => pair.Key == "children");
        Assert.NotNull(rootNodeChildren);
        Assert.NotNull(rootNodeChildren.Array);
        var physicsShapeList = rootNodeChildren.Array.FirstOrDefault(arrValue =>
            (
                (EngineString)
                    arrValue.Children?.FirstOrDefault(pair => pair.Key == "_class")!.Value!
            ).Value! == "PhysicsShapeList"
        );
        Assert.NotNull(physicsShapeList);
    }

    [Fact]
    public void Test_Roundtrips_Example()
    {
        var fixturePath = TestFixtures.GetPath("kv3", "example.kv3");
        var fixtureContent = File.ReadAllText(fixturePath);
        var parser = new KeyValues3FormatParser();
        var document = parser.Parse(fixtureContent);
        Assert.Equal(
            "<!-- kv3 encoding:text:version{e21c7f3c-8a33-41c5-9977-a76d3a32aa0d} format:generic:version{7412167c-06e9-4698-aff2-e63eb59037e7} -->",
            document.Header
        );
        var writer = new StringWriter();
        var serializer = new KeyValues3FormatSerializer();
        serializer.Serialize(document, writer);
        var serialized = writer.ToString();
        var reparsedDocument = parser.Parse(serialized);
        Assert.Equivalent(document, reparsedDocument);
    }

    [Fact]
    public void Test_Roundtrips_Model()
    {
        var fixturePath = TestFixtures.GetPath("kv3", "omni.vmdl");
        var fixtureContent = File.ReadAllText(fixturePath);
        var parser = new KeyValues3FormatParser();
        var document = parser.Parse(fixtureContent);
        var writer = new StringWriter();
        var serializer = new KeyValues3FormatSerializer();
        serializer.Serialize(document, writer);
        var serialized = writer.ToString();
        var reparsedDocument = parser.Parse(serialized);
        Assert.Equivalent(document, reparsedDocument);
    }
}
