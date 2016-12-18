using System.Collections.Generic;
using System.Text;

namespace XASM
{
    public struct _OP
    {
        public int iType;                              // Type
        public int iOffsetIndex;                       // Index of the offset
        public int iIntLiteral;                        // Integer literal    
        public float fFloatLiteral;                    // Float literal    
        public int iStringTableIndex;                  // String table index    
        public int iStackIndex;                        // Stack index    
        public int iInstrIndex;                        // Instruction index    
        public int iFuncIndex;                         // Function index    
        public int iHostAPICallIndex;                  // Host API Call index    
        public int iReg;                               // Register code
    }

    public class ScriptHeader                    // Script header data
    {
        public int iStackSize;                             // Requested stack size
        public int iGlobalDataSize;                        // The size of the script's global data

        public bool iIsMainFuncPresent;                     // Is _Main () present?
        public int iMainFuncIndex;                         // _Main ()'s function index

        public int iPriorityType;                          // The thread priority type
        public int iUserPriority;                          // The user-defined priority (if any)
    }

    public class Lexer
    {
        public int iCurrSourceLine { get; set; }                       // Current line in the source
        public int iIndex0 { get; set; }                               // Indices into the string
        public int iIndex1 { get; set; }
        public int CurrToken { get; set; }                             // Current token
        public string strCurrLexeme { get; set; }                      // Current lexeme
        public int iCurrLexState { get; set; }                         // The current lex state

        public void ResetLexer()
        {
            // Set the current line to the start of the file
            iCurrSourceLine = 0;

            // Set both indices to point to the start of the string
            iIndex0 = 0;
            iIndex1 = 0;

            // Set the token type to invalid, since a token hasn't been read yet
            CurrToken = Constants.TOKEN_TYPE_INVALID;

            // Set the lexing state to no strings
            iCurrLexState = Constants.LEX_STATE_NO_STRING;
        }

    }

    public class InstrLookup
    {
        public string strMnemonic { get; set; }                         // Mnemonic string
        public int iOpcode { get; set; }                                // Opcode
        public int iOpCount { get; set; }                               // Number of operands
        public List<int> OpList;
    }

    public class Operand
    {
        public _OP operand;
    }

    public class Instruction
    {
        public int iOpcode { get; set; }            // Opcode
        public int iOpCount { get; set; }           // Number of operands
        public List<Operand> OpList;                // Pointer to operand list

        public void CloneOplist(List<Operand> source)
        {
            OpList = new List<Operand>(source);
        }
    }

    public class FuncNode
    {
        public int iIndex { get; set; }             // Index
        public string strName { get; set; }         // Name
        public int iEntryPoint { get; set; }        // Entry point
        public int iParamCount { get; set; }        // Param count
        public int iLocalDataSize { get; set; }     // Local data size

        public FuncNode() { }

        public FuncNode(int index, string name, int entrypoint, int paramcount, int localdatasize)
        {
            iIndex = index;
            strName = new string(name.ToCharArray());
            iEntryPoint = entrypoint;
            iParamCount = paramcount;
            iLocalDataSize = localdatasize;
        }

        public FuncNode(FuncNode source)
        {
            iIndex = source.iIndex;
            strName = new string(source.strName.ToCharArray());
            iEntryPoint = source.iEntryPoint;
            iParamCount = source.iParamCount;
            iLocalDataSize = source.iLocalDataSize;
        }
    }

    public class LabelNode
    {
        public int iIndex { get; set; }                                 // Index
        public string strIdent { get; set; }                            // Identifier
        public int iTargetIndex { get; set; }                           // Index of the target instruction
        public int iFuncIndex { get; set; }                             // Function in which the label resides

        public LabelNode() { }

        public LabelNode(int index, string ident, int targetindex, int funcindex)
        {
            iIndex = index;
            strIdent = new string(ident.ToCharArray());
            iTargetIndex = targetindex;
            iFuncIndex = funcindex;
        }

        public LabelNode(LabelNode source)
        {
            iIndex = source.iIndex;
            strIdent = new string(source.strIdent.ToCharArray());
            iTargetIndex = source.iTargetIndex;
            iFuncIndex = source.iFuncIndex;
        }
    }

    public class SymbolNode
    {
        public int iIndex { get; set; }                                 // Index
        public string strIdent { get; set; }                            // Identifier
        public int iSize { get; set; }                                  // Size (1 for variables, N for arrays)
        public int iStackIndex { get; set; }                            // The stack index to which the symbol
                                                                        // points
        public int iFuncIndex { get; set; }                             // Function in which the symbol resides

        public SymbolNode() { }

        public SymbolNode(int index, string ident, int size, int stackindex, int funcindex)
        {
            iIndex = index;
            strIdent = new string(ident.ToCharArray());
            iSize = size;
            iStackIndex = stackindex;
            iFuncIndex = funcindex;
        }

        public SymbolNode(SymbolNode source)
        {
            iIndex = source.iIndex;
            strIdent = new string(source.strIdent.ToCharArray());
            iSize = source.iSize;
            iStackIndex = source.iStackIndex;
            iFuncIndex = source.iFuncIndex;
        }

        public override string ToString()
        {
            StringBuilder result = new StringBuilder();
            result.Append("identifier :" + strIdent + '\n');
            result.Append("size       :" + iSize + '\n');
            result.Append("stack index:" + iStackIndex + '\n');
            result.Append("func index :" + iFuncIndex + '\n');
            return result.ToString();
        }
    }
}