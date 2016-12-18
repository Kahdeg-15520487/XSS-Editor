using System.Collections.Generic;
using System.Text;

namespace XASM
{
    public class XASM_SymbolTable
    {
        public List<SymbolNode> g_SymbolTable = new List<SymbolNode>();

        public int AddSymbol(string strIdent, int iSize, int iStackIndex, int iFuncIndex)
        {
            // If a label already exists
            if (GetSymbolByIdent(strIdent, iFuncIndex) != 9999)
                return 9999;

            // Create a new symbol node and Initialize the new label
            SymbolNode NewSymbol = new SymbolNode(-1, strIdent, iSize, iStackIndex, iFuncIndex);

            // Add the symbol to the list and get its index
            g_SymbolTable.Add(NewSymbol);
            int iIndex = g_SymbolTable.Count - 1;
            //Debug.Log(g_SymbolTable.Count);

            // Set the symbol node's index
            g_SymbolTable[iIndex].iIndex = iIndex;
            // Return the new symbol's index
            return iIndex;
        }
        public int GetSymbolByIdent(string strIdent, int iFuncIndex)
        {
            // If the table is empty, return a NULL pointer
            if (g_SymbolTable.Count == 0)
                return 9999;

            // Traverse the list until the matching structure is found
            foreach (var item in g_SymbolTable)
            {
                // See if the names match
                if (item.strIdent == strIdent)
                    // If the functions match, or if the existing symbol is global, they match.
                    // Return the symbol.
                    if (item.iFuncIndex == iFuncIndex || item.iStackIndex >= 0)
                        return item.iIndex;
            }

            // The structure was not found, so return a NULL pointer
            return 9999;
        }
        public int GetStackIndexByIdent(string strIdent, int iFuncIndex)
        {
            int itempIndex = GetSymbolByIdent(strIdent, iFuncIndex);
            return g_SymbolTable[itempIndex].iStackIndex;
        }

        public int GetSizeByIdent(string strIdent, int iFuncIndex)
        {
            int itempIndex = GetSymbolByIdent(strIdent, iFuncIndex);
            return g_SymbolTable[itempIndex].iSize;
        }

        public override string ToString()
        {
            StringBuilder result = new StringBuilder();
            foreach (var symbol in g_SymbolTable)
            {
                result.Append(symbol.ToString() + '\n');
            }
            return result.ToString();
        }
    }
}