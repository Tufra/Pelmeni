using System;
using System.Globalization;

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

        public static string StringConcat(string left, string right)
        {
            return left + right;
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

        #endregion
    }
}