using System;

namespace PelmeniLib
{
    public class StdLib
    {
        public static void Print(string str)
        {
            Console.Write(str);
        }

        public static string IntToString(long num)
        {
            return num.ToString();
        }

        public static string ReadLine()
        {
            return Console.ReadLine()!;
        }

        public static string StringConcat(string left, string right)
        {
            return left + right;
        }

        public static bool StringEquals(string a, string b)
        {
            return a.Equals(b);
        }
    }
}