using System;

public static class StringExtension
{
    public static string Reverse(this string s)
    {
        char[] charArray = s.ToCharArray();
        Array.Reverse(charArray);
        return new(charArray);
    }


    public static string GetCopyName(this string fileName)
    {
        int copyNumber = 0;
        const string pattern = " (";

        int indexOfPattern = fileName.LastIndexOf(pattern, StringComparison.Ordinal);

        if (indexOfPattern != -1)
        {
            int endIndex = fileName.LastIndexOf(')');
            if (endIndex != -1 && endIndex > indexOfPattern)
            {
                string numberString = fileName.Substring(
                    indexOfPattern + pattern.Length,
                    endIndex - indexOfPattern - pattern.Length
                );

                if (int.TryParse(numberString, out copyNumber)) fileName = fileName.Remove(indexOfPattern);
            }
        }

        fileName += $" ({copyNumber + 1})";

        return fileName;
    }
}