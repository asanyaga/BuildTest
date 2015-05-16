using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Distributr.Core.Utility
{
   public static class ExceptionHelper
    {
        public static string AllMessages(this Exception exp)
        {
            string message = string.Empty;
            Exception innerException = exp;

            do
            {
                message = message + (string.IsNullOrEmpty(innerException.Message) ? string.Empty : innerException.Message);
                innerException = innerException.InnerException;
            }
            while (innerException != null);

            return message;
        }
    }; 
    public class StringUtils
    {
        public static string GenerateRandomString(string allowedChars, int minLength, int maxLength, Random random)
        {
            char[] chars = new char[maxLength];
            int setLength = allowedChars.Length;
            int length = random.Next(minLength, maxLength);
            for (int i = 0; i < length; ++i)
            {
                chars[i] = allowedChars[random.Next(setLength)];
            }
            return new string(chars, 0, length);
        }

        public static string GenerateRandomString(int minLength, int maxLength, Random random)
        {
            const string allowedChars = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ";//abcdefghijklmnopqrstuvwxyz
            char[] chars = new char[maxLength];
            int setLength = allowedChars.Length;
            int length = random.Next(minLength, maxLength);
            for (int i = 0; i < length; ++i)
            {
                chars[i] = allowedChars[random.Next(setLength)];
            }
            return new string(chars, 0, length);
        }

        public static string BreakUpperCB(string sInput)
        {
            if (sInput == null) return "";
            //Regex.Replace(sInput, "([a-z](?=[A-Z0-9])|[A-Z](?=[A-Z][a-z]))", "$1 ")
            StringBuilder[] sReturn = new StringBuilder[1];
            sReturn[0] = new StringBuilder(sInput.Length);
            const string CUPPER = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            int iArrayCount = 0;
            for (int iIndex = 0; iIndex < sInput.Length; iIndex++)
            {
                string sChar = sInput.Substring(iIndex, 1); // get a char
                if ((CUPPER.Contains(sChar)) && (iIndex > 0))
                {
                    iArrayCount++;
                    StringBuilder[] sTemp = new StringBuilder[iArrayCount + 1];
                    Array.Copy(sReturn, 0, sTemp, 0, iArrayCount);
                    sTemp[iArrayCount] = new StringBuilder(sInput.Length);
                    sReturn = sTemp;
                }
                sReturn[iArrayCount].Append(sChar);
            }
            string[] sReturnString = new string[iArrayCount + 1];
            for (int iIndex = 0; iIndex < sReturn.Length; iIndex++)
            {
                sReturnString[iIndex] = sReturn[iIndex].ToString();
            }
            string returnString = "";
            for (int i = 0; i < sReturnString.Length; i++)
            {
                returnString += sReturnString[i];
                if (i < sReturnString.Length)
                    returnString += " ";
            }
            return returnString.Trim();
        }
    }
}
