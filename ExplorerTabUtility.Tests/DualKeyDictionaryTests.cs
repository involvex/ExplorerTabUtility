using Xunit;
using ExplorerTabUtility.Models;

namespace ExplorerTabUtility.Tests;

public class DualKeyDictionaryTests
{
    [Fact]
    public void Add_And_Lookup_By_Both_Keys()
    {
        var dict = new DualKeyDictionary<string, int, string>();
        dict.Add("primary", "value", 42);

        Assert.True(dict.TryGetByPrimary("primary", out var byPrimary, out _));
        Assert.Equal("value", byPrimary);

        Assert.True(dict.TryGetByOptional(42, out _, out var byOptional));
        Assert.Equal("value", byOptional);
    }

    [Fact]
    public void Optional_Key_Conflict_Moves_Key_To_New_Primary()
    {
        var dict = new DualKeyDictionary<string, int, string>();
        dict.Add("a", "va", 1);
        dict.Add("b", "vb", 2);

        // Overwrite "b" to reuse optional key 1 (OverwriteExisting behavior
        // steals the optional key from "a").
        dict["b"] = new DualKeyEntry<string, int, string>("b", "vb2", 1);

        Assert.True(dict.TryGetByOptional(1, out var primary, out var current));
        Assert.Equal("b", primary);
        Assert.Equal("vb2", current);
        Assert.False(dict.ContainsOptional(2));
    }

    [Fact]
    public void Remove_By_Primary_Clears_Optional_Mapping()
    {
        var dict = new DualKeyDictionary<string, int, string>();
        dict.Add("a", "va", 1);

        Assert.True(dict.Remove("a"));
        Assert.False(dict.ContainsOptional(1));
        Assert.Empty(dict);
    }
}
