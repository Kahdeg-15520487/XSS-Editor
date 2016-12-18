using System.Collections.Generic;

namespace XASM
{
    public class XASM_FunctionTable
    {
        public List<FuncNode> g_FuncTable = new List<FuncNode>();

        public int GetFuncByName(string strName)
        {
            foreach (var item in g_FuncTable)
            {
                if (item.strName == strName)
                {
                    return item.iIndex;
                }
            }
            return -1;
        }

        public int AddFunc(string strName, int iEntrypoint)
        {
            //check if function existed
            if (GetFuncByName(strName) != -1)
                return -1;

            // Create a new function node
            FuncNode NewFunc = new FuncNode();

            // Initialize the new function
            NewFunc.strName = new string(strName.ToCharArray());
            NewFunc.iEntryPoint = iEntrypoint;

            // Add the function to the list and get its index
            g_FuncTable.Add(NewFunc);
            int iIndex = g_FuncTable.Count - 1;

            // Set the function node's index
            g_FuncTable[iIndex].iIndex = iIndex;

            //return func index
            return iIndex;
        }

        public void SetFuncInfo(string strName, int iParamCount, int iLocalDataSize)
        {
            // Based on the function's name, find its node in the list
            int itempIndex = GetFuncByName(strName);
            if (itempIndex == -1)
                return;
            else
            {
                // Set the remaining fields
                g_FuncTable[itempIndex].iParamCount = iParamCount;
                g_FuncTable[itempIndex].iLocalDataSize = iLocalDataSize;
            }
        }
    }
}