public static class TestFixtures
{
    private static readonly string Root = Path.Combine(AppContext.BaseDirectory, "Fixtures");

    public static string GetPath(params string[] parts)
    {
        return Path.Join(new[] { Root }.Concat(parts).ToArray());
    }
}
