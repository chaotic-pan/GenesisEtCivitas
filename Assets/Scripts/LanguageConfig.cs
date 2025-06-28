
public class LanguageConfig
{
    public static string[] C1 = new []
    {
        "m", "n", "k", "p", "b", "t", "d", "k", "g", "s", "ch",
        "z", "f", "v", "sh", "h", "l", "r", "f", "w",
        "th", "q",
    };

    public static string[] C1Special = new[]
    {
        "đ", "č", "ł", "ž", "ǧ", "ç"
    };
    
    public static string[] C2Onset = new []
    {
        "ts", "pf", "kr", "tr", "pr", "pl", "pw",
        "kw", "ky", "ty", "py", "my", "ml", "ny", "fl",
        "fw", "fy", "dy", "dw", "dl", "ds", "gw", "gy", "gs", "gl", "sh", "tr", "br", "cr", "cl",
        "sf", "gn", "sk", "sp", "sm", "sn", "st", "sw", "sl",
        "vr", "zl", "zl", "bl", "dr"
    };

    public static string[] C2Coda = new[]
    {
        "ks", "kt", "pt", "nt", "lk", "lt", "mp", "nd",
        "nk", "ng", "nz", "ps",
    };

    public static string[] C3Onset = new[]
    {
        "str", "shtr", "spl", "scr", "shr", "pst", "skl", "spr", "skw", "sth"
    };

    public static string[] V1Base = new[]
    {
        "a", "i", "u", "e", "o", "y"
    };

    public static string[] V1Special = new[]
    {
        "á", "à", "ä", "ã"
        , "é", "è", "ê", " ë"
        , "í", "ì", "ï", "î"
        , "ú", "ù", "û", "ü"
        , "ó", "ò", "ø", "ö"
    };
}
