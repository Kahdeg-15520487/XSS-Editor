using System.Collections.Generic;

namespace XASM
{

    public class XASM_LabelTable
    {
        public List<LabelNode> g_LabelTable = new List<LabelNode>();

        public int GetLabelByIdent(string strIdent, int iFuncIndex)
        {
            if (g_LabelTable.Count == 0)
                return -1;

            // Traverse the list until the matching structure is found
            foreach (var item in g_LabelTable)
            {
                // If the names and scopes match, return the current pointer
                if ((item.strIdent == strIdent) && (item.iFuncIndex == iFuncIndex))
                    return item.iIndex;
            }

            return -1;
        }

        public int AddLabel(string strIdent, int iTargetIndex, int iFuncIndex)
        {
            // If a label already exists, return -1
            if (GetLabelByIdent(strIdent, iFuncIndex) != -1)
                return -1;

            // Create a new label node and Initialize the new label
            LabelNode NewLabel = new LabelNode(-1, strIdent, iTargetIndex, iFuncIndex);

            // Add the label to the list and get its index
            g_LabelTable.Add(NewLabel);
            int iIndex = g_LabelTable.Count - 1;

            // Set the index of the label node
            g_LabelTable[iIndex].iIndex = iIndex;

            // Return the new label's index
            return iIndex;
        }
    }
}