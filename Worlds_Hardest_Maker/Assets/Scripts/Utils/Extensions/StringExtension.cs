using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class StringExtension
{
    public static string Reverse(this string s)
    {
        char[] charArray = s.ToCharArray();
        Array.Reverse(charArray);
        return new(charArray);
    }
}
