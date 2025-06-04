using RfCodeGen.ServiceLayer.Utils.Pluralizer.Rules;
using System.Text.RegularExpressions;

namespace RfCodeGen.ServiceLayer.Utils.Pluralizer;

public partial class Pluralizer
{
    private readonly Dictionary<Regex, string> _pluralRules = PluralRules.GetRules();
    private readonly Dictionary<Regex, string> _singularRules = SingularRules.GetRules();
    private readonly List<string> _uncountables = Uncountables.GetUncountables();
    private readonly Dictionary<string, string> _irregularPlurals = IrregularRules.GetIrregularPlurals();
    private readonly Dictionary<string, string> _irregularSingles = IrregularRules.GetIrregularSingulars();
    private readonly Regex replacementRegex = ReplacementRegex();

    public string Pluralize(string word)
    {
        return Transform(word, _irregularSingles, _irregularPlurals, _pluralRules);
    }

    public string Singularize(string word)
    {
        return Transform(word, _irregularPlurals, _irregularSingles, _singularRules);
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1862:Use the 'StringComparison' method overloads to perform case-insensitive string comparisons", Justification = "Using the 'StringComparison' method changes the logic and produces incorrect output.")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("CodeQuality", "IDE0079:Remove unnecessary suppression", Justification = "<Pending>")]
    internal static string RestoreCase(string originalWord, string newWord)
    {
        // Tokens are an exact match.
        if (originalWord == newWord)
            return newWord;

        // Upper cased words. E.g. "HELLO".
        if (originalWord == originalWord.ToUpper())
            return newWord.ToUpper();

        // Title cased words. E.g. "Title".
        if (originalWord[0] == char.ToUpper(originalWord[0]))
            return char.ToUpper(newWord[0]) + newWord[1..];

        // Lower cased words. E.g. "test".
        return newWord.ToLower();
    }

    internal string ApplyRules(string token, string originalWord, Dictionary<Regex, string> rules)
    {
        // Empty string or doesn't need fixing.
        if (string.IsNullOrEmpty(token) || _uncountables.Contains(token))
            return RestoreCase(originalWord, token);

        var length = rules.Count;

        // Iterate over the sanitization rules and use the first one to match.
        while (length-- > 0)
        {
            var rule = rules.ElementAt(length);

            // If the rule passes, return the replacement.
            if (rule.Key.IsMatch(originalWord))
            {
                var match = rule.Key.Match(originalWord);
                var matchString = match.Groups[0].Value;
                if (string.IsNullOrWhiteSpace(matchString))
                    return rule.Key.Replace(originalWord, GetReplaceMethod(originalWord[match.Index-1].ToString(), rule.Value), 1);
                return rule.Key.Replace(originalWord, GetReplaceMethod(matchString, rule.Value), 1);
            }
        }

        return originalWord;
    }

    private MatchEvaluator GetReplaceMethod(string originalWord, string replacement)
    {
        return match =>
        {
            return RestoreCase(originalWord, replacementRegex.Replace(replacement, m=> match.Groups[Convert.ToInt32(m.Groups[1].Value)].Value));
        };
    }

    internal string Transform(string word, Dictionary<string, string> replacables, Dictionary<string, string> keepables, Dictionary<Regex, string> rules)
    {
        var token = word.ToLower();
        if (keepables.ContainsKey(token)) return RestoreCase(word, token);
        if (replacables.TryGetValue(token, out string? value)) return RestoreCase(word, value);
        return ApplyRules(token, word, rules);
    }

    [GeneratedRegex("\\$(\\d{1,2})")]
    private static partial Regex ReplacementRegex();
}

/* Pluralizer original source code from https://github.com/rvegajr/Pluralize.NET.Core
 
The MIT License (MIT)

Copyright (c) 2013 Blake Embrey (hello@blakeembrey.com), Ported to C# by Sarath KCM, Ported to CDMS by Michael Baker

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in
all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
THE SOFTWARE.
*/