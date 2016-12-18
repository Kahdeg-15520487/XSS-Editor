using System.Collections.Generic;

namespace XASM
{
    public class XASM_StringTable
    {
        public List<string> g_StringTable = new List<string>();

        public int AddString(string strString)
        {
            foreach (string item in g_StringTable)
            {
                if (item == strString)
                {
                    return -1;
                }
            }
            g_StringTable.Add(strString);
            return g_StringTable.Count - 1;
        }
    }
}