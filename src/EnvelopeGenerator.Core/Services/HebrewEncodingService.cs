using System.Text;

namespace EnvelopeGenerator.Core.Services;

public class HebrewEncodingService : IEncodingService
{
    private static readonly Dictionary<char, char> WindowsToDosHebrew = new()
    {
        // מיפוי תווים מ-Windows-1255 ל-DOS Hebrew
        // יש להשלים את הטבלה המלאה
        {'א', '\x80'},
        {'ב', '\x81'},
        {'ג', '\x82'},
        {'ד', '\x83'},
        {'ה', '\x84'},
        {'ו', '\x85'},
        {'ז', '\x86'},
        {'ח', '\x87'},
        {'ט', '\x88'},
        {'י', '\x89'},
        {'כ', '\x8A'},
        {'ל', '\x8B'},
        {'מ', '\x8C'},
        {'נ', '\x8D'},
        {'ס', '\x8E'},
        {'ע', '\x8F'},
        {'פ', '\x90'},
        {'צ', '\x91'},
        {'ק', '\x92'},
        {'ר', '\x93'},
        {'ש', '\x94'},
        {'ת', '\x95'},
        {'ך', '\x9A'},
        {'ם', '\x9B'},
        {'ן', '\x9C'},
        {'ף', '\x9D'},
        {'ץ', '\x9E'}
    };

    public string ConvertToDosHebrew(string text)
    {
        if (string.IsNullOrEmpty(text))
            return text;

        var result = new StringBuilder(text.Length);
        foreach (char c in text)
        {
            if (WindowsToDosHebrew.TryGetValue(c, out char dosChar))
            {
                result.Append(dosChar);
            }
            else
            {
                result.Append(c);
            }
        }

        return result.ToString();
    }

    public string ReverseHebrew(string text)
    {
        if (string.IsNullOrEmpty(text))
            return text;

        char[] chars = text.ToCharArray();
        Array.Reverse(chars);
        return new string(chars);
    }

    public string ConvertToWindowsHebrew(string text)
    {
        if (string.IsNullOrEmpty(text))
            return text;

        var reverseDictionary = WindowsToDosHebrew.ToDictionary(x => x.Value, x => x.Key);
        var result = new StringBuilder(text.Length);

        foreach (char c in text)
        {
            if (reverseDictionary.TryGetValue(c, out char winChar))
            {
                result.Append(winChar);
            }
            else
            {
                result.Append(c);
            }
        }

        return result.ToString();
    }
}