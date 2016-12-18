//TODO
//change everything to static
namespace XASM
{
    public static class XASM_String_Process
    {
        public static bool IsCharWhitespace(char cChar)
        {
            // Return true if the character is a space or tab.

            if (cChar == ' ' || cChar == '\t')
                return true;
            else
                return false;
        }

        public static bool IsCharNumeric(char cChar)
        {
            // Return true if the character is between 0 and 9 inclusive.

            if (cChar >= '0' && cChar <= '9')
                return true;
            else
                return false;
        }

        public static bool IsCharIdent(char cChar)
        {
            // Return true if the character is between 0 or 9 inclusive or is an uppercase or
            // lowercase letter or underscore

            if ((cChar >= '0' && cChar <= '9') ||
                 (cChar >= 'A' && cChar <= 'Z') ||
                 (cChar >= 'a' && cChar <= 'z') ||
                 cChar == '_')
                return true;
            else
                return false;
        }

        public static bool IsCharDelimiter(char cChar)
        {
            // Return true if the character is a delimiter

            if (cChar == ':' || cChar == ',' || cChar == '"' ||
                 cChar == '[' || cChar == ']' ||
                 cChar == '{' || cChar == '}' ||
                 IsCharWhitespace(cChar) || cChar == '\n')
                return true;
            else
                return false;
        }

        public static bool IsStringWhitespace(string strString)
        {
            // If the length is zero, it's technically whitespace

            if (strString.Length == 0)
                return true;

            // Loop through each character and return false if a non-whitespace is found

            for (int iCurrCharIndex = 0; iCurrCharIndex < strString.Length; ++iCurrCharIndex)
                if (!IsCharWhitespace(strString[iCurrCharIndex]) && strString[iCurrCharIndex] != '\n')
                    return false;

            // Otherwise return true

            return true;
        }

        public static bool IsStringIdent(string strString)
        {
            // If the length of the string is zero, it's not a valid identifier

            if (strString.Length == 0)
                return false;

            // If the first character is a number, it's not a valid identifier

            if (strString[0] >= '0' && strString[0] <= '9')
                return false;

            // Loop through each character and return zero upon encountering the first invalid identifier
            // character

            for (int iCurrCharIndex = 0; iCurrCharIndex < strString.Length; ++iCurrCharIndex)
                if (!IsCharIdent(strString[iCurrCharIndex]))
                    return false;

            // Otherwise return true

            return true;
        }

        public static bool IsStringInteger(string strString)
        {
            // If the string's length is zero, it's not an integer

            if (strString.Length == 0)
                return false;

            int iCurrCharIndex;

            // Loop through the string and make sure each character is a valid number or minus sign

            for (iCurrCharIndex = 0; iCurrCharIndex < strString.Length; ++iCurrCharIndex)
                if (!IsCharNumeric(strString[iCurrCharIndex]) && !(strString[iCurrCharIndex] == '-'))
                    return false;

            // Make sure the minus sign only occured at the first character

            for (iCurrCharIndex = 1; iCurrCharIndex < strString.Length; ++iCurrCharIndex)
                if (strString[iCurrCharIndex] == '-')
                    return false;

            return true;
        }

        public static bool IsStringFloat(string strString)
        {
            if (strString.Length == 0)
                return false;

            // First make sure we've got only numbers and radix points

            int iCurrCharIndex;

            for (iCurrCharIndex = 0; iCurrCharIndex < strString.Length; ++iCurrCharIndex)
                if (!IsCharNumeric(strString[iCurrCharIndex]) && !(strString[iCurrCharIndex] == '.') && !(strString[iCurrCharIndex] == '-'))
                    return false;

            // Make sure only one radix point is present

            bool iRadixPointFound = false;

            for (iCurrCharIndex = 0; iCurrCharIndex < strString.Length; ++iCurrCharIndex)
                if (strString[iCurrCharIndex] == '.')
                    if (iRadixPointFound)
                        return false;
                    else
                        iRadixPointFound = true;

            // Make sure the minus sign only appears in the first character

            for (iCurrCharIndex = 1; iCurrCharIndex < strString.Length; ++iCurrCharIndex)
                if (strString[iCurrCharIndex] == '-')
                    return false;

            // If a radix point was found, return true; otherwise, it must be an integer so return false

            if (iRadixPointFound)
                return true;
            else
                return false;
        }

        public static string TrimWhitespace(string istrString)
        {
            string strString = new string(istrString.Trim().ToCharArray());
            return strString;
        }

        public static string StripComments(string strString)
        {
            string temp = new string(strString.ToCharArray());
            int iCurrCharIndex;
            int iInString;
            // Scan through the source line and terminate the string at the first semicolon

            iInString = 0;
            for (iCurrCharIndex = 0; iCurrCharIndex < temp.Length - 1; ++iCurrCharIndex)
            {
                // Look out for strings; they can contain semicolons

                if (temp[iCurrCharIndex] == '"')
                    if (iInString != 0)
                        iInString = 0;
                    else
                        iInString = 1;

                // If a non-string semicolon is found, terminate the string at it's position

                if (temp[iCurrCharIndex] == ';')
                {
                    if (iInString == 0)
                    {
                        temp = Replace(temp, iCurrCharIndex, ' ');
                        temp = temp.Remove(iCurrCharIndex + 1);
                        break;
                    }
                }
            }

            return temp;
        }

        public static string Replace(string source, int index, char replacement)
        {
            var temp = source.ToCharArray();
            temp[index] = replacement;
            return new string(temp);
        }
    }
}