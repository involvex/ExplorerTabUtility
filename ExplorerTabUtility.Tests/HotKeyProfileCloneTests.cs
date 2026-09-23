using H.Hooks;
using Xunit;
using ExplorerTabUtility.Models;

namespace ExplorerTabUtility.Tests;

public class HotKeyProfileCloneTests
{
    [Fact]
    public void Clone_Deep_Copies_HotKeys_And_Preserves_CloseTab()
    {
        var original = new HotKeyProfile(
            "CloseTab",
            [Key.Ctrl, Key.W],
            HotKeyAction.CloseTab,
            scope: HotkeyScope.FileExplorer);

        var clone = original.Clone();

        Assert.Equal(original.Id, clone.Id);
        Assert.Equal(HotKeyAction.CloseTab, clone.Action);
        Assert.Equal(original.HotKeys, clone.HotKeys);
        Assert.NotSame(original.HotKeys!, clone.HotKeys!);
    }

    [Fact]
    public void Clone_Preserves_Mouse_Flags()
    {
        var original = new HotKeyProfile
        {
            Name = "MMB Close",
            Action = HotKeyAction.CloseTab,
            Scope = HotkeyScope.FileExplorer,
            IsMouse = true,
            IsDoubleClick = false,
            HotKeys = [Key.MouseMiddle],
        };

        var clone = original.Clone();

        Assert.True(clone.IsMouse);
        Assert.False(clone.IsDoubleClick);
        Assert.Equal([Key.MouseMiddle], clone.HotKeys);
    }
}
