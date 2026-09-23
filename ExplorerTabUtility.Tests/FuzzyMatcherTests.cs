using Xunit;
using ExplorerTabUtility.Helpers;

namespace ExplorerTabUtility.Tests;

public class FuzzyMatcherTests
{
    [Fact]
    public void Empty_Query_Matches_Everything_With_Zero_Score()
    {
        Assert.Equal(0, FuzzyMatcher.ScoreCandidate("Downloads", "file:///C:/Users/x/Downloads", null, ""));
        Assert.Equal(0, FuzzyMatcher.ScoreCandidate("Downloads", "file:///C:/Users/x/Downloads", null, "   "));
        Assert.Equal(0, FuzzyMatcher.ScoreCandidate(null, null, null, null));
    }

    [Fact]
    public void Exact_Substring_Matches()
    {
        var score = FuzzyMatcher.ScoreCandidate("Downloads", "file:///C:/Users/x/Downloads", null, "down");
        Assert.True(score > 0);
    }

    [Fact]
    public void Matching_Is_Case_Insensitive()
    {
        var lower = FuzzyMatcher.ScoreCandidate("Downloads", null, null, "down");
        var upper = FuzzyMatcher.ScoreCandidate("DOWNLOADS", null, null, "down");
        Assert.True(lower > 0);
        Assert.True(upper > 0);
    }

    [Fact]
    public void Subsequence_Partial_Matches()
    {
        // "dwnlds" is not a substring of "Downloads" but matches as subsequence.
        Assert.True(FuzzyMatcher.IsMatch("Downloads", null, null, "dwnlds"));
    }

    [Fact]
    public void Exact_Match_Ranks_Above_Scattered_Match()
    {
        var exact = FuzzyMatcher.ScoreCandidate("Downloads", null, null, "down");
        var scattered = FuzzyMatcher.ScoreCandidate("Documents And New Stuff", null, null, "down");
        Assert.True(exact > scattered);
    }

    [Fact]
    public void Multi_Token_Query_Requires_All_Tokens()
    {
        const string location = """C:\Users\lukas\Documents\Reports""";

        Assert.True(FuzzyMatcher.IsMatch("Reports", location, null, "user rep"));
        Assert.False(FuzzyMatcher.IsMatch("Reports", location, null, "user zzz"));
    }

    [Fact]
    public void Typo_Tolerance_Matches_Near_Miss_Segment()
    {
        // Transposed letters: not a subsequence, matched via edit distance.
        Assert.True(FuzzyMatcher.IsMatch("Downloads", null, null, "Downlaods"));
        Assert.False(FuzzyMatcher.IsMatch("Downloads", null, null, "zzz"));
    }

    [Fact]
    public void Short_Tokens_Do_Not_Use_Typo_Fallback()
    {
        // "dx" is neither subsequence nor eligible for typo fallback.
        Assert.False(FuzzyMatcher.IsMatch("Downloads", null, null, "dx"));
    }

    [Fact]
    public void Location_Field_Is_Searched()
    {
        const string location = """file:///C:/Program Files/MyApp""";
        Assert.True(FuzzyMatcher.IsMatch("MyApp", location, null, "program"));
    }
}
