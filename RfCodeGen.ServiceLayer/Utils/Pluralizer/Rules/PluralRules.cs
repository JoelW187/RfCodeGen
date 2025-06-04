using System.Text.RegularExpressions;

namespace RfCodeGen.ServiceLayer.Utils.Pluralizer.Rules;

internal static partial class PluralRules
{
    public static Dictionary<Regex, string> GetRules()
    {
        return new Dictionary<Regex, string>
        {
            {MyRegex(), "s"},
            {MyRegex1(), "$0"},
            {MyRegex2(), "$1"},
            {MyRegex3(), "$1es"},
            {MyRegex4(), "$1es"},
            {MyRegex5(), "$1s"},
            {MyRegex6(), "$1"},
            {MyRegex7(), "$1i"},
            {MyRegex8(), "$1ae"},
            {MyRegex9(), "$1im"},
            {MyRegex10(), "$1oes"},
            {MyRegex11(), "$1a"},
            {MyRegex12(), "$1a"},
            {MyRegex13(), "ses"},
            {MyRegex14(), "$1$2ves"},
            {MyRegex15(), "$1ies"},
            {MyRegex16(), "$1ies"},
            {MyRegex17(), "$1es"},
            {MyRegex18(), "$1ices"},
            {MyRegex19(), "$1ice"},
            {MyRegex20(), "$1ople"},
            {MyRegex21(), "$1ren"},
            {MyRegex22(), "$0"},
            {MyRegex23(), "men"},
            {MyRegex24(), "you" },


            {MyRegex25(), "$0"},
            {MyRegex26(), "$0"},
            {MyRegex27(), "$0"},
            {MyRegex28(), "$0"},
            {MyRegex29(), "$0"},
            {MyRegex30(), "$0"},
            {MyRegex31(), "$0"}
        };
    }

    [GeneratedRegex("s?$", RegexOptions.IgnoreCase, "en-US")]
    private static partial Regex MyRegex();
    [GeneratedRegex("[^\u0000-\u007F]$", RegexOptions.IgnoreCase, "en-US")]
    private static partial Regex MyRegex1();
    [GeneratedRegex("([^aeiou]ese)$", RegexOptions.IgnoreCase, "en-US")]
    private static partial Regex MyRegex2();
    [GeneratedRegex("(ax|test)is$", RegexOptions.IgnoreCase, "en-US")]
    private static partial Regex MyRegex3();
    [GeneratedRegex("(alias|[^aou]us|tlas|gas|ris)$", RegexOptions.IgnoreCase, "en-US")]
    private static partial Regex MyRegex4();
    [GeneratedRegex("(e[mn]u)s?$", RegexOptions.IgnoreCase, "en-US")]
    private static partial Regex MyRegex5();
    [GeneratedRegex("([^l]ias|[aeiou]las|[emjzr]as|[iu]am)$", RegexOptions.IgnoreCase, "en-US")]
    private static partial Regex MyRegex6();
    [GeneratedRegex("(alumn|syllab|octop|vir|radi|nucle|fung|cact|stimul|termin|bacill|foc|uter|loc|strat)(?:us|i)$", RegexOptions.IgnoreCase, "en-US")]
    private static partial Regex MyRegex7();
    [GeneratedRegex("(alumn|alg|vertebr)(?:a|ae)$", RegexOptions.IgnoreCase, "en-US")]
    private static partial Regex MyRegex8();
    [GeneratedRegex("(seraph|cherub)(?:im)?$", RegexOptions.IgnoreCase, "en-US")]
    private static partial Regex MyRegex9();
    [GeneratedRegex("(her|at|gr)o$", RegexOptions.IgnoreCase, "en-US")]
    private static partial Regex MyRegex10();
    [GeneratedRegex("(agend|addend|millenni|dat|extrem|bacteri|desiderat|strat|candelabr|errat|ov|symposi|curricul|automat|quor)(?:a|um)$", RegexOptions.IgnoreCase, "en-US")]
    private static partial Regex MyRegex11();
    [GeneratedRegex("(apheli|hyperbat|periheli|asyndet|noumen|phenomen|criteri|organ|prolegomen|hedr|automat)(?:a|on)$", RegexOptions.IgnoreCase, "en-US")]
    private static partial Regex MyRegex12();
    [GeneratedRegex("sis$", RegexOptions.IgnoreCase, "en-US")]
    private static partial Regex MyRegex13();
    [GeneratedRegex("(?:(kni|wi|li)fe|(ar|l|ea|eo|oa|hoo)f)$", RegexOptions.IgnoreCase, "en-US")]
    private static partial Regex MyRegex14();
    [GeneratedRegex("([^aeiouy]|qu)y$", RegexOptions.IgnoreCase, "en-US")]
    private static partial Regex MyRegex15();
    [GeneratedRegex("([^ch][ieo][ln])ey$", RegexOptions.IgnoreCase, "en-US")]
    private static partial Regex MyRegex16();
    [GeneratedRegex("(x|ch|ss|sh|zz)$", RegexOptions.IgnoreCase, "en-US")]
    private static partial Regex MyRegex17();
    [GeneratedRegex("(matr|cod|mur|sil|vert|ind|append)(?:ix|ex)$", RegexOptions.IgnoreCase, "en-US")]
    private static partial Regex MyRegex18();
    [GeneratedRegex("(m|l)(?:ice|ouse)$", RegexOptions.IgnoreCase, "en-US")]
    private static partial Regex MyRegex19();
    [GeneratedRegex("(pe)(?:rson|ople)$", RegexOptions.IgnoreCase, "en-US")]
    private static partial Regex MyRegex20();
    [GeneratedRegex("(child)(?:ren)?$", RegexOptions.IgnoreCase, "en-US")]
    private static partial Regex MyRegex21();
    [GeneratedRegex("eaux$", RegexOptions.IgnoreCase, "en-US")]
    private static partial Regex MyRegex22();
    [GeneratedRegex("m[ae]n$", RegexOptions.IgnoreCase, "en-US")]
    private static partial Regex MyRegex23();
    [GeneratedRegex("^thou$", RegexOptions.IgnoreCase, "en-US")]
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
