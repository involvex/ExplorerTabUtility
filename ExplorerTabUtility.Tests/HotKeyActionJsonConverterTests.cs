using System.Text.Json;
using Xunit;
using ExplorerTabUtility.Helpers;
using ExplorerTabUtility.Models;

namespace ExplorerTabUtility.Tests;

public class HotKeyActionJsonConverterTests
{
    private static JsonSerializerOptions Options()
    {
        var options = new JsonSerializerOptions();
        options.Converters.Add(new HotKeyActionJsonConverter());
        return options;
    }

    [Fact]
    public void Legacy_Numeric_Values_Map_Correctly()
    {
        var options = Options();

        Assert.Equal(HotKeyAction.Open, JsonSerializer.Deserialize<HotKeyAction>("0", options));
        Assert.Equal(HotKeyAction.Duplicate, JsonSerializer.Deserialize<HotKeyAction>("1", options));
        Assert.Equal(HotKeyAction.ReopenClosed, JsonSerializer.Deserialize<HotKeyAction>("2", options));
        Assert.Equal(HotKeyAction.DetachTab, JsonSerializer.Deserialize<HotKeyAction>("9", options));
        Assert.Equal(HotKeyAction.TabSearch, JsonSerializer.Deserialize<HotKeyAction>("14", options));
    }

    [Fact]
    public void String_CloseTab_Round_Trips()
    {
        var options = Options();

        var json = JsonSerializer.Serialize(HotKeyAction.CloseTab, options);
        Assert.Equal("\"CloseTab\"", json);

        var parsed = JsonSerializer.Deserialize<HotKeyAction>("\"CloseTab\"", options);
        Assert.Equal(HotKeyAction.CloseTab, parsed);
    }

    [Fact]
    public void Unknown_Values_Fall_Back_To_Open()
    {
        var options = Options();

        Assert.Equal(HotKeyAction.Open, JsonSerializer.Deserialize<HotKeyAction>("9999", options));
        Assert.Equal(HotKeyAction.Open, JsonSerializer.Deserialize<HotKeyAction>("\"NotARealAction\"", options));
    }
}
