using System;
using System.Collections.Generic;

namespace ExplorerTabUtility.Helpers;

/// <summary>
/// Lightweight fuzzy matcher for tab search. Dependency-free (no COM/WPF) so it
/// can be unit tested on any target framework.
/// Matches when every whitespace-separated query token matches the text, either as an
/// ordered subsequence (e.g. "dwnlds" matches "Downloads") or — for tokens of length
/// 3+ — as a near-miss of a single path segment (Levenshtein distance 1-2, e.g.
/// "Downlaods" matches "Downloads"). Higher score means a better match.
/// </summary>
public static class FuzzyMatcher
{
    private const int NoMatch = -1;

    /// <summary>
    /// Scores a tab candidate against a query across name, location and display location.
    /// Returns <see cref="NoMatch"/> when any query token matches none of the fields.
    /// An empty query matches everything with score 0.
    /// </summary>
    public static int ScoreCandidate(string? name, string? location, string? displayLocation, string? query)
    {
        if (query is null || query.Trim().Length == 0)
            return 0;

        var tokens = query.Split((char[])null!, StringSplitOptions.RemoveEmptyEntries);
        var total = 0;

        foreach (var token in tokens)
        {
            var best = NoMatch;
            best = Math.Max(best, ScoreText(name, token));
            best = Math.Max(best, ScoreText(location, token));
            best = Math.Max(best, ScoreText(displayLocation, token));

            if (best == NoMatch)
                return NoMatch;

            total += best;
        }

        // Slight preference for shorter names when scores tie upstream is handled by
        // the caller; keep the raw sum here.
        return total;
    }

    public static bool IsMatch(string? name, string? location, string? displayLocation, string? query)
    {
        return ScoreCandidate(name, location, displayLocation, query) != NoMatch;
    }

    private static int ScoreText(string? text, string token)
    {
        if (text is null || text.Length == 0)
            return NoMatch;

        if (TryScoreSubsequence(text, token, out var score))
            return score;

        // Typo fallback only for meaningful tokens to avoid noise.
        if (token.Length >= 3 && TryScoreTypo(text, token, out var typoScore))
            return typoScore;

        return NoMatch;
    }

    private static bool TryScoreSubsequence(string text, string token, out int score)
    {
        score = NoMatch;

        var textLower = text.ToLowerInvariant();
        var tokenLower = token.ToLowerInvariant();

        var scoreSum = 0;
        var textIndex = 0;
        var prevMatch = -2; // -2 = no previous match yet
        var firstMatch = -1;

        for (var i = 0; i < tokenLower.Length; i++)
        {
            var found = textLower.IndexOf(tokenLower[i], textIndex);
            if (found < 0)
                return false;

            if (firstMatch < 0)
                firstMatch = found;

            var charScore = 10;

            if (found == 0)
                charScore += 25; // match at start
            else if (prevMatch == found - 1)
                charScore += 15; // consecutive run
            else if (IsSeparator(text[found - 1]))
                charScore += 12; // word boundary after separator

            // camelCase boundary: previous char lowercase, current uppercase.
            if (found > 0 && char.IsLower(text[found - 1]) && char.IsUpper(text[found]))
                charScore += 8;

            scoreSum += charScore;
            prevMatch = found;
            textIndex = found + 1;
        }

        // Earlier first match and shorter text rank higher.
        scoreSum -= firstMatch * 2;
        scoreSum -= text.Length / 50;

        score = scoreSum;
        return true;
    }

    private static bool TryScoreTypo(string text, string token, out int score)
    {
        score = NoMatch;

        var maxDistance = token.Length <= 4 ? 1 : 2;
        var best = NoMatch;

        foreach (var segment in SplitSegments(text))
        {
            if (segment.Length < 3)
                continue;

            // Skip segments whose length differs too much to ever match.
            if (Math.Abs(segment.Length - token.Length) > maxDistance)
                continue;

            var distance = LevenshteinDistance(
                segment.ToLowerInvariant(), token.ToLowerInvariant(), maxDistance);
            if (distance < 0)
                continue;

            // Typo matches intentionally score below subsequence matches so exact and
            // partial matches always rank first.
            var candidate = 20 - distance * 8 - Math.Abs(segment.Length - token.Length);
            if (candidate > best)
                best = candidate;
        }

        if (best == NoMatch)
            return false;

        score = best;
        return true;
    }

    private static IEnumerable<string> SplitSegments(string text)
    {
        return text.Split(
            new[] { '\\', '/', ' ', '-', '_', '.', ':', '(', ')', '[', ']' },
            StringSplitOptions.RemoveEmptyEntries);
    }

    private static bool IsSeparator(char c)
    {
        return c == '\\' || c == '/' || c == ' ' || c == '-' || c == '_' ||
               c == '.' || c == ':' || c == '(' || c == ')' || c == '[' || c == ']';
    }

    /// <summary>
    /// Levenshtein distance with early exit. Returns -1 when the distance exceeds
    /// <paramref name="maxDistance"/>.
    /// </summary>
    internal static int LevenshteinDistance(string a, string b, int maxDistance)
    {
        if (Math.Abs(a.Length - b.Length) > maxDistance)
            return -1;

        // Ensure b is the shorter string to minimize working memory.
        if (a.Length < b.Length)
        {
            var temp = a;
            a = b;
            b = temp;
        }

        var previous = new int[b.Length + 1];
        var current = new int[b.Length + 1];

        for (var j = 0; j <= b.Length; j++)
            previous[j] = j;

        for (var i = 1; i <= a.Length; i++)
        {
            current[0] = i;
            var rowMin = current[0];

            for (var j = 1; j <= b.Length; j++)
            {
                var cost = a[i - 1] == b[j - 1] ? 0 : 1;
                var deletion = previous[j] + 1;
                var insertion = current[j - 1] + 1;
                var substitution = previous[j - 1] + cost;

                var value = deletion;
                if (insertion < value) value = insertion;
                if (substitution < value) value = substitution;

                current[j] = value;
                if (value < rowMin) rowMin = value;
            }

            if (rowMin > maxDistance)
                return -1;

            var swap = previous;
            previous = current;
            current = swap;
        }

        return previous[b.Length] <= maxDistance ? previous[b.Length] : -1;
    }
}
