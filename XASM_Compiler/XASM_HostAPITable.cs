using System.Collections.Generic;

namespace XASM
{
    public class XASM_HostAPITable
    {
        public List<string> g_HostAPICallTable = new List<string>();

        public int AddString(string strString)
        {
            for (int i = 0; i < g_HostAPICallTable.Count; i++)
            {
                if (g_HostAPICallTable[i] == strString)
                {
                    return i;
                }
            }

            g_HostAPICallTable.Add(strString);
            return g_HostAPICallTable.Count - 1;
        }
    }
}