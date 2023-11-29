using System;
using System.Globalization;
using System.Linq;

namespace PelmeniLib
{
    public class StdLib
    {
        #region IO

            public static void Print(string str)
            {
                Console.Write(str);
            }
            
            public static void PrintLine(string str)
            {
                Console.WriteLine(str);
            }
            
            public static string ReadLine()
            {
                return Console.ReadLine()!;
            }

        #endregion

        #region String

            public static string StringConcat(params string[] strings)
            {
                return string.Join("", strings);
            }

            public static bool StringEquals(string a, string b)
            {
                return a.Equals(b);
            }

            public static string StringReplace(string str, string oldStr, string newStr)
            {
                return str.Replace(oldStr, newStr);
            }

        #endregion

        #region Integer

            public static string IntToString(long num)
            {
                return num.ToString();
            }

            public static double IntToReal(long num)
            {
                return num;
            }

            public static long ParseInteger(string str)
            {
                return long.Parse(str);
            }

        #endregion

        #region Real

            public static string RealToString(double num)
            {
                return num.ToString(CultureInfo.InvariantCulture);
            }

            public static double ParseReal(string str)
            {
                return double.Parse(str.Replace(',', '.'), 
                    CultureInfo.InvariantCulture);
            }

            public static long RoundReal(double val)
            {
                return (long)Math.Round(val);
            }

        #endregion

        #region Boolean

            public static string BooleanToString(bool val)
            {
                return val.ToString();
            }

        #endregion

        #region Char

            public static string CharToString(char val)
            {
                return val.ToString();
            }

            public static long CharToCode(char val)
            {
                return val;
            }

        #endregion
    }
}