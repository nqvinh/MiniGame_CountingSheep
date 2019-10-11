using System.Collections.Generic;
using UnityEngine;

namespace BigNumbers
{
    public partial class BigNumber
    {
        private static List<string> numberCodes = new List<string> { "", "K", "M", "B", "T" };
        private static readonly string alphabe = "abcdefghijklmnopqrstuvwxyz";
        private static bool init = false;
        private static int MaxCharDigit = 2;


        public static BigNumber Parse(string str)
        {
            BigNumber A = new BigNumber();
            BigNumber.SetFromString(A, str,true);
            return A;
        }
        // disclaimed:

        // + WORK WITH long string format. eg: "234234234242343545646948583.585938583"
        // - NOT WORK WITH science format yet!. eg: "23.33E+34"
        // - NOT WORK WITH digit <= 2
        public static string ToShortString(string str, int digit = 6, int precision = 2)
        {
            if (digit <= 2) return str;
            if (!init) InitNumberCodes(MaxCharDigit);

            bool isFloat = str.Contains(".");
            int exponentIdx = str.Contains("E") ? str.IndexOf("E") : -1;

            // get exponent value
            int exponent = exponentIdx == -1 ? 0 :
                str[exponentIdx + 1] == '+' ? int.Parse(str.Substring(exponentIdx + 2, str.Length - exponentIdx - 2)) :
                -int.Parse(str.Substring(exponentIdx + 2, str.Length - exponentIdx - 2));


            int digitLen = isFloat ? str.IndexOf(".") : exponentIdx == -1 ? str.Length : exponentIdx;
            int a = Mathf.FloorToInt(digitLen / 3);

            while (a > 0 && digitLen - (a - 1) * 3 <= digit) a--;

            int b = a * 3;
            int endIdx = digitLen - b;
            string newStr = str.Substring(0, endIdx);

            // insert ","
            string _newStr = newStr;
            for (int i = _newStr.Length - 3; i >= 1; i -= 3)
                newStr = newStr.Insert(i, ",");

            // insert number after float point
            string floatNumber = "";
            int indexOfDot = str.IndexOf(".");
            
            if (indexOfDot >= 0)
            {
                string decimalNumber = str.Substring(0, indexOfDot);
                if (decimalNumber.Length > 4)
                {
                    precision = 0;
                }
                else
                {
                    floatNumber = str.Substring(indexOfDot + 1);
                    if (floatNumber.Length > precision)
                        floatNumber = floatNumber.Substring(0, precision);
                    int numberFloat = -1;
                    int.TryParse(floatNumber, out numberFloat);
                    if (numberFloat == 0)
                    {
                        precision = 0;
                    }
                }
            }
            else
            {
                precision = 0;
            }
                
            //
            if (precision > 0)
            {
                int from = isFloat ? endIdx + 1 : endIdx;
                int len = Mathf.Min(precision, exponentIdx != -1 ? exponentIdx - from : str.Length - from);

                if (len > 0)
                {
                    newStr += "." + str.Substring(from, len);
                    int diff = precision - len;
                    if (diff > 0) newStr += "".PadRight(diff, '0');
                }
                else
                    newStr += "." + "".PadRight(precision, '0');

            }

            newStr += numberCodes[Mathf.Min(a + exponent)];
            return newStr;
        }

        public static void InitNumberCodes(int charLen)
        {
            Iter("", charLen, 0);
            init = true;
        }

        private static int IterChar(string code, int step, int i)
        {
            code += alphabe[i];
            if (step > 0) return Iter(code, step, i);

            numberCodes.Add(code);
            return i + 1;
        }

        private static int Iter(string code, int step, int i)
        {
            int k = 0;
            while (k < alphabe.Length) k = IterChar(code, step - 1, k);
            return i + 1;
        }
    }
}