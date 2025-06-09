using System.Text.RegularExpressions;

namespace RfCodeGen.ProjectConfigs.Utils.Pluralizer.Rules;

internal static partial class SingularRules
{
    public static Dictionary<Regex, string> GetRules()
    {
        return new Dictionary<Regex, string>
        {
            {MyRegex(), ""},
            {MyRegex1(), "$1"},
            {MyRegex2(), "$1sis"},
            {MyRegex3(), "$1sis"},
            {MyRegex4(), "$1fe"},
            {MyRegex5(), "$1f"},
            {MyRegex6(), "y"},
            {MyRegex7(), "$1ie"},
            {MyRegex8(), "$1ey"},
            {MyRegex9(), "$1ouse"},
            {MyRegex10(), "$1"},
            {MyRegex11(), "$1"},
            {MyRegex12(), "$1"},
            {MyRegex13(), "$1"},
            {MyRegex14(), "$1is"},
            {MyRegex15(), "$1us"},
            {MyRegex16(), "$1um"},
            {MyRegex17(), "$1on"},
            {MyRegex18(), "$1a"},
            {MyRegex19(), "$1ex"},
            {MyRegex20(), "$1ix"},
            {MyRegex21(), "$1rson"},
            {MyRegex22(), "$1"},
            {MyRegex23(), "$1"},
            {MyRegex24(), "man" },

            {MyRegex25(), "$0"},
            {MyRegex26(), "$0"},
            {MyRegex27(), "$0"},
            {MyRegex28(), "$0"},
            {MyRegex29(), "$0"},
            {MyRegex30(), "$0"},
            {MyRegex31(), "$0"}
        };
    }

    [GeneratedRegex("s$", RegexOptions.IgnoreCase, "en-US")]
    private static partial Regex MyRegex();
    [GeneratedRegex("(ss)$", RegexOptions.IgnoreCase, "en-US")]
    private static partial Regex MyRegex1();
    [GeneratedRegex("((a)naly|(b)a|(d)iagno|(p)arenthe|(p)rogno|(s)ynop|(t)he)(?:sis|ses)$", RegexOptions.IgnoreCase, "en-US")]
    private static partial Regex MyRegex2();
    [GeneratedRegex("(^analy)(?:sis|ses)$", RegexOptions.IgnoreCase, "en-US")]
    private static partial Regex MyRegex3();
    [GeneratedRegex("(wi|kni|(?:after|half|high|low|mid|non|night|[^\\w]|^)li)ves$", RegexOptions.IgnoreCase, "en-US")]
    private static partial Regex MyRegex4();
    [GeneratedRegex("(ar|(?:wo|[ae])l|[eo][ao])ves$", RegexOptions.IgnoreCase, "en-US")]
    private static partial Regex MyRegex5();
    [GeneratedRegex("ies$", RegexOptions.IgnoreCase, "en-US")]
    private static partial Regex MyRegex6();
    [GeneratedRegex("\\b([pl]|zomb|(?:neck|cross)?t|coll|faer|food|gen|goon|group|lass|talk|goal|cut)ies$", RegexOptions.IgnoreCase, "en-US")]
    private static partial Regex MyRegex7();
    [GeneratedRegex("\\b(mon|smil)ies$", RegexOptions.IgnoreCase, "en-US")]
    private static partial Regex MyRegex8();
    [GeneratedRegex("(m|l)ice$", RegexOptions.IgnoreCase, "en-US")]
    private static partial Regex MyRegex9();
    [GeneratedRegex("(seraph|cherub)im$", RegexOptions.IgnoreCase, "en-US")]
    private static partial Regex MyRegex10();
    [GeneratedRegex("(x|ch|ss|sh|zz|tto|go|cho|alias|[^aou]us|tlas|gas|(?:her|at|gr)o|ris)(?:es)?$", RegexOptions.IgnoreCase, "en-US")]
    private static partial Regex MyRegex11();
    [GeneratedRegex("(e[mn]u)s?$", RegexOptions.IgnoreCase, "en-US")]
    private static partial Regex MyRegex12();
    [GeneratedRegex("(movie|twelve)s$", RegexOptions.IgnoreCase, "en-US")]
    private static partial Regex MyRegex13();
    [GeneratedRegex("(cris|test|diagnos)(?:is|es)$", RegexOptions.IgnoreCase, "en-US")]
    private static partial Regex MyRegex14();
    [GeneratedRegex("(alumn|syllab|octop|vir|radi|nucle|fung|cact|stimul|termin|bacill|foc|uter|loc|strat)(?:us|i)$", RegexOptions.IgnoreCase, "en-US")]
    private static partial Regex MyRegex15();
    [GeneratedRegex("(agend|addend|millenni|dat|extrem|bacteri|desiderat|strat|candelabr|errat|ov|symposi|curricul|quor)a$", RegexOptions.IgnoreCase, "en-US")]
    private static partial Regex MyRegex16();
    [GeneratedRegex("(apheli|hyperbat|periheli|asyndet|noumen|phenomen|criteri|organ|prolegomen|hedr|automat)a$", RegexOptions.IgnoreCase, "en-US")]
    private static partial Regex MyRegex17();
    [GeneratedRegex("(alumn|alg|vertebr)ae$", RegexOptions.IgnoreCase, "en-US")]
    private static partial Regex MyRegex18();
    [GeneratedRegex("(cod|mur|sil|vert|ind)ices$", RegexOptions.IgnoreCase, "en-US")]
    private static partial Regex MyRegex19();
    [GeneratedRegex("(matr|append)ices$", RegexOptions.IgnoreCase, "en-US")]
    private static partial Regex MyRegex20();
    [GeneratedRegex("(pe)(rson|ople)$", RegexOptions.IgnoreCase, "en-US")]
    private static partial Regex MyRegex21();
    [GeneratedRegex("(child)ren$", RegexOptions.IgnoreCase, "en-US")]
    private static partial Regex MyRegex22();
    [GeneratedRegex("(eau)x?$", RegexOptions.IgnoreCase, "en-US")]
    private static partial Regex MyRegex23();
    [GeneratedRegex("men$", RegexOptions.IgnoreCase, "en-US")]
    private static partial Regex MyRegex24();

    [GeneratedRegex("pox$", RegexOptions.IgnoreCase, "en-US")]
    private static partial Regex MyRegex25();
    [GeneratedRegex("ois$", RegexOptions.IgnoreCase, "en-US")]
    private static partial Regex MyRegex26();
    [GeneratedRegex("deer$", RegexOptions.IgnoreCase, "en-US")]
    private static partial Regex MyRegex27();
    [GeneratedRegex("fish$", RegexOptions.IgnoreCase, "en-US")]
    private static partial Regex MyRegex28();
    [GeneratedRegex("sheep$", RegexOptions.IgnoreCase, "en-US")]
    private static partial Regex MyRegex29();
    [GeneratedRegex("measles$/", RegexOptions.IgnoreCase, "en-US")]
    private static partial Regex MyRegex30();
    [GeneratedRegex("[^aeiou]ese$", RegexOptions.IgnoreCase, "en-US")]
    private static partial Regex MyRegex31();
}
