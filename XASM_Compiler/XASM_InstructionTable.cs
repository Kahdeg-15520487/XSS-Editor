using System.Collections.Generic;

namespace XASM
{
    public class XASM_InstructionTable
    {

        public List<InstrLookup> g_InstrTable = new List<InstrLookup>();

        public void InitInstrTable()
        {
            // Create a temporary index to use with each instruction
            int iInstrIndex;

            // The following code makes repeated calls to AddInstrLookup () to add a hardcoded
            // instruction set to the assembler's vocabulary. Each AddInstrLookup () call is
            // followed by zero or more calls to SetOpType (), which set the supported types of
            // a specific operand. The instructions are grouped by family.

            // ---- Main

            // Mov          Destination, Source

            iInstrIndex = AddInstrLookup("Mov", Constants.INSTR_MOV, 2);
            SetOpType(Constants.OP_FLAG_TYPE_MEM_REF |
                                        Constants.OP_FLAG_TYPE_REG);
            SetOpType(Constants.OP_FLAG_TYPE_INT |
                                        Constants.OP_FLAG_TYPE_FLOAT |
                                        Constants.OP_FLAG_TYPE_STRING |
                                        Constants.OP_FLAG_TYPE_MEM_REF |
                                        Constants.OP_FLAG_TYPE_REG);

            // ---- Arithmetic

            // Add         Destination, Source

            iInstrIndex = AddInstrLookup("Add", Constants.INSTR_ADD, 2);
            SetOpType(Constants.OP_FLAG_TYPE_MEM_REF |
                                        Constants.OP_FLAG_TYPE_REG);
            SetOpType(Constants.OP_FLAG_TYPE_INT |
                                        Constants.OP_FLAG_TYPE_FLOAT |
                                        Constants.OP_FLAG_TYPE_STRING |
                                        Constants.OP_FLAG_TYPE_MEM_REF |
                                        Constants.OP_FLAG_TYPE_REG);

            // Sub          Destination, Source

            iInstrIndex = AddInstrLookup("Sub", Constants.INSTR_SUB, 2);
            SetOpType(Constants.OP_FLAG_TYPE_MEM_REF |
                                        Constants.OP_FLAG_TYPE_REG);
            SetOpType(Constants.OP_FLAG_TYPE_INT |
                                        Constants.OP_FLAG_TYPE_FLOAT |
                                        Constants.OP_FLAG_TYPE_STRING |
                                        Constants.OP_FLAG_TYPE_MEM_REF |
                                        Constants.OP_FLAG_TYPE_REG);

            // Mul          Destination, Source

            iInstrIndex = AddInstrLookup("Mul", Constants.INSTR_MUL, 2);
            SetOpType(Constants.OP_FLAG_TYPE_MEM_REF |
                                        Constants.OP_FLAG_TYPE_REG);
            SetOpType(Constants.OP_FLAG_TYPE_INT |
                                        Constants.OP_FLAG_TYPE_FLOAT |
                                        Constants.OP_FLAG_TYPE_STRING |
                                        Constants.OP_FLAG_TYPE_MEM_REF |
                                        Constants.OP_FLAG_TYPE_REG);

            // Div          Destination, Source

            iInstrIndex = AddInstrLookup("Div", Constants.INSTR_DIV, 2);
            SetOpType(Constants.OP_FLAG_TYPE_MEM_REF |
                                        Constants.OP_FLAG_TYPE_REG);
            SetOpType(Constants.OP_FLAG_TYPE_INT |
                                        Constants.OP_FLAG_TYPE_FLOAT |
                                        Constants.OP_FLAG_TYPE_STRING |
                                        Constants.OP_FLAG_TYPE_MEM_REF |
                                        Constants.OP_FLAG_TYPE_REG);

            // Mod          Destination, Source

            iInstrIndex = AddInstrLookup("Mod", Constants.INSTR_MOD, 2);
            SetOpType(Constants.OP_FLAG_TYPE_MEM_REF |
                                        Constants.OP_FLAG_TYPE_REG);
            SetOpType(Constants.OP_FLAG_TYPE_INT |
                                        Constants.OP_FLAG_TYPE_FLOAT |
                                        Constants.OP_FLAG_TYPE_STRING |
                                        Constants.OP_FLAG_TYPE_MEM_REF |
                                        Constants.OP_FLAG_TYPE_REG);

            // Exp          Destination, Source

            iInstrIndex = AddInstrLookup("Exp", Constants.INSTR_EXP, 2);
            SetOpType(Constants.OP_FLAG_TYPE_MEM_REF |
                                        Constants.OP_FLAG_TYPE_REG);
            SetOpType(Constants.OP_FLAG_TYPE_INT |
                                        Constants.OP_FLAG_TYPE_FLOAT |
                                        Constants.OP_FLAG_TYPE_STRING |
                                        Constants.OP_FLAG_TYPE_MEM_REF |
                                        Constants.OP_FLAG_TYPE_REG);

            // Neg          Destination

            iInstrIndex = AddInstrLookup("Neg", Constants.INSTR_NEG, 1);
            SetOpType(Constants.OP_FLAG_TYPE_MEM_REF |
                                        Constants.OP_FLAG_TYPE_REG);

            // Inc          Destination

            iInstrIndex = AddInstrLookup("Inc", Constants.INSTR_INC, 1);
            SetOpType(Constants.OP_FLAG_TYPE_MEM_REF |
                                        Constants.OP_FLAG_TYPE_REG);

            // Dec          Destination

            iInstrIndex = AddInstrLookup("Dec", Constants.INSTR_DEC, 1);
            SetOpType(Constants.OP_FLAG_TYPE_MEM_REF |
                                        Constants.OP_FLAG_TYPE_REG);

            // ---- Bitwise

            // And          Destination, Source

            iInstrIndex = AddInstrLookup("And", Constants.INSTR_AND, 2);
            SetOpType(Constants.OP_FLAG_TYPE_MEM_REF |
                                        Constants.OP_FLAG_TYPE_REG);
            SetOpType(Constants.OP_FLAG_TYPE_INT |
                                        Constants.OP_FLAG_TYPE_FLOAT |
                                        Constants.OP_FLAG_TYPE_STRING |
                                        Constants.OP_FLAG_TYPE_MEM_REF |
                                        Constants.OP_FLAG_TYPE_REG);

            // Or           Destination, Source

            iInstrIndex = AddInstrLookup("Or", Constants.INSTR_OR, 2);
            SetOpType(Constants.OP_FLAG_TYPE_MEM_REF |
                                        Constants.OP_FLAG_TYPE_REG);
            SetOpType(Constants.OP_FLAG_TYPE_INT |
                                        Constants.OP_FLAG_TYPE_FLOAT |
                                        Constants.OP_FLAG_TYPE_STRING |
                                        Constants.OP_FLAG_TYPE_MEM_REF |
                                        Constants.OP_FLAG_TYPE_REG);

            // XOr          Destination, Source

            iInstrIndex = AddInstrLookup("XOr", Constants.INSTR_XOR, 2);
            SetOpType(Constants.OP_FLAG_TYPE_MEM_REF |
                                        Constants.OP_FLAG_TYPE_REG);
            SetOpType(Constants.OP_FLAG_TYPE_INT |
                                        Constants.OP_FLAG_TYPE_FLOAT |
                                        Constants.OP_FLAG_TYPE_STRING |
                                        Constants.OP_FLAG_TYPE_MEM_REF |
                                        Constants.OP_FLAG_TYPE_REG);

            // Not          Destination

            iInstrIndex = AddInstrLookup("Not", Constants.INSTR_NOT, 1);
            SetOpType(Constants.OP_FLAG_TYPE_MEM_REF |
                                        Constants.OP_FLAG_TYPE_REG);

            // ShL          Destination, Source

            iInstrIndex = AddInstrLookup("ShL", Constants.INSTR_SHL, 2);
            SetOpType(Constants.OP_FLAG_TYPE_MEM_REF |
                                        Constants.OP_FLAG_TYPE_REG);
            SetOpType(Constants.OP_FLAG_TYPE_INT |
                                        Constants.OP_FLAG_TYPE_FLOAT |
                                        Constants.OP_FLAG_TYPE_STRING |
                                        Constants.OP_FLAG_TYPE_MEM_REF |
                                        Constants.OP_FLAG_TYPE_REG);

            // ShR          Destination, Source

            iInstrIndex = AddInstrLookup("ShR", Constants.INSTR_SHR, 2);
            SetOpType(Constants.OP_FLAG_TYPE_MEM_REF |
                                        Constants.OP_FLAG_TYPE_REG);
            SetOpType(Constants.OP_FLAG_TYPE_INT |
                                        Constants.OP_FLAG_TYPE_FLOAT |
                                        Constants.OP_FLAG_TYPE_STRING |
                                        Constants.OP_FLAG_TYPE_MEM_REF |
                                        Constants.OP_FLAG_TYPE_REG);

            // ---- String Manipulation

            // Concat       String0, String1

            iInstrIndex = AddInstrLookup("Concat", Constants.INSTR_CONCAT, 2);
            SetOpType(Constants.OP_FLAG_TYPE_MEM_REF |
                                        Constants.OP_FLAG_TYPE_REG);
            SetOpType(Constants.OP_FLAG_TYPE_MEM_REF |
                                        Constants.OP_FLAG_TYPE_REG |
                                        Constants.OP_FLAG_TYPE_STRING);

            // GetChar      Destination, Source, Index

            iInstrIndex = AddInstrLookup("GetChar", Constants.INSTR_GETCHAR, 3);
            SetOpType(Constants.OP_FLAG_TYPE_MEM_REF |
                                        Constants.OP_FLAG_TYPE_REG);
            SetOpType(Constants.OP_FLAG_TYPE_MEM_REF |
                                        Constants.OP_FLAG_TYPE_REG |
                                        Constants.OP_FLAG_TYPE_STRING);
            SetOpType(Constants.OP_FLAG_TYPE_MEM_REF |
                                        Constants.OP_FLAG_TYPE_REG |
                                        Constants.OP_FLAG_TYPE_INT);

            // SetChar      Destination, Index, Source

            iInstrIndex = AddInstrLookup("SetChar", Constants.INSTR_SETCHAR, 3);
            SetOpType(Constants.OP_FLAG_TYPE_MEM_REF |
                                        Constants.OP_FLAG_TYPE_REG);
            SetOpType(Constants.OP_FLAG_TYPE_MEM_REF |
                                        Constants.OP_FLAG_TYPE_REG |
                                        Constants.OP_FLAG_TYPE_INT);
            SetOpType(Constants.OP_FLAG_TYPE_MEM_REF |
                                        Constants.OP_FLAG_TYPE_REG |
                                        Constants.OP_FLAG_TYPE_STRING);

            // ---- Conditional Branching

            // Jmp          Label

            iInstrIndex = AddInstrLookup("Jmp", Constants.INSTR_JMP, 1);
            SetOpType(Constants.OP_FLAG_TYPE_LINE_LABEL);

            // JE           Op0, Op1, Label

            iInstrIndex = AddInstrLookup("JE", Constants.INSTR_JE, 3);
            SetOpType(Constants.OP_FLAG_TYPE_INT |
                                        Constants.OP_FLAG_TYPE_FLOAT |
                                        Constants.OP_FLAG_TYPE_STRING |
                                        Constants.OP_FLAG_TYPE_MEM_REF |
                                        Constants.OP_FLAG_TYPE_REG);
            SetOpType(Constants.OP_FLAG_TYPE_INT |
                                        Constants.OP_FLAG_TYPE_FLOAT |
                                        Constants.OP_FLAG_TYPE_STRING |
                                        Constants.OP_FLAG_TYPE_MEM_REF |
                                        Constants.OP_FLAG_TYPE_REG);
            SetOpType(Constants.OP_FLAG_TYPE_LINE_LABEL);

            // JNE          Op0, Op1, Label

            iInstrIndex = AddInstrLookup("JNE", Constants.INSTR_JNE, 3);
            SetOpType(Constants.OP_FLAG_TYPE_INT |
                                        Constants.OP_FLAG_TYPE_FLOAT |
                                        Constants.OP_FLAG_TYPE_STRING |
                                        Constants.OP_FLAG_TYPE_MEM_REF |
                                        Constants.OP_FLAG_TYPE_REG);
            SetOpType(Constants.OP_FLAG_TYPE_INT |
                                        Constants.OP_FLAG_TYPE_FLOAT |
                                        Constants.OP_FLAG_TYPE_STRING |
                                        Constants.OP_FLAG_TYPE_MEM_REF |
                                        Constants.OP_FLAG_TYPE_REG);
            SetOpType(Constants.OP_FLAG_TYPE_LINE_LABEL);

            // JG           Op0, Op1, Label

            iInstrIndex = AddInstrLookup("JG", Constants.INSTR_JG, 3);
            SetOpType(Constants.OP_FLAG_TYPE_INT |
                                        Constants.OP_FLAG_TYPE_FLOAT |
                                        Constants.OP_FLAG_TYPE_STRING |
                                        Constants.OP_FLAG_TYPE_MEM_REF |
                                        Constants.OP_FLAG_TYPE_REG);
            SetOpType(Constants.OP_FLAG_TYPE_INT |
                                        Constants.OP_FLAG_TYPE_FLOAT |
                                        Constants.OP_FLAG_TYPE_STRING |
                                        Constants.OP_FLAG_TYPE_MEM_REF |
                                        Constants.OP_FLAG_TYPE_REG);
            SetOpType(Constants.OP_FLAG_TYPE_LINE_LABEL);

            // JL           Op0, Op1, Label

            iInstrIndex = AddInstrLookup("JL", Constants.INSTR_JL, 3);
            SetOpType(Constants.OP_FLAG_TYPE_INT |
                                        Constants.OP_FLAG_TYPE_FLOAT |
                                        Constants.OP_FLAG_TYPE_STRING |
                                        Constants.OP_FLAG_TYPE_MEM_REF |
                                        Constants.OP_FLAG_TYPE_REG);
            SetOpType(Constants.OP_FLAG_TYPE_INT |
                                        Constants.OP_FLAG_TYPE_FLOAT |
                                        Constants.OP_FLAG_TYPE_STRING |
                                        Constants.OP_FLAG_TYPE_MEM_REF |
                                        Constants.OP_FLAG_TYPE_REG);
            SetOpType(Constants.OP_FLAG_TYPE_LINE_LABEL);

            // JGE          Op0, Op1, Label

            iInstrIndex = AddInstrLookup("JGE", Constants.INSTR_JGE, 3);
            SetOpType(Constants.OP_FLAG_TYPE_INT |
                                        Constants.OP_FLAG_TYPE_FLOAT |
                                        Constants.OP_FLAG_TYPE_STRING |
                                        Constants.OP_FLAG_TYPE_MEM_REF |
                                        Constants.OP_FLAG_TYPE_REG);
            SetOpType(Constants.OP_FLAG_TYPE_INT |
                                        Constants.OP_FLAG_TYPE_FLOAT |
                                        Constants.OP_FLAG_TYPE_STRING |
                                        Constants.OP_FLAG_TYPE_MEM_REF |
                                        Constants.OP_FLAG_TYPE_REG);
            SetOpType(Constants.OP_FLAG_TYPE_LINE_LABEL);

            // JLE           Op0, Op1, Label

            iInstrIndex = AddInstrLookup("JLE", Constants.INSTR_JLE, 3);
            SetOpType(Constants.OP_FLAG_TYPE_INT |
                                        Constants.OP_FLAG_TYPE_FLOAT |
                                        Constants.OP_FLAG_TYPE_STRING |
                                        Constants.OP_FLAG_TYPE_MEM_REF |
                                        Constants.OP_FLAG_TYPE_REG);
            SetOpType(Constants.OP_FLAG_TYPE_INT |
                                        Constants.OP_FLAG_TYPE_FLOAT |
                                        Constants.OP_FLAG_TYPE_STRING |
                                        Constants.OP_FLAG_TYPE_MEM_REF |
                                        Constants.OP_FLAG_TYPE_REG);
            SetOpType(Constants.OP_FLAG_TYPE_LINE_LABEL);

            // ---- The Stack Interface

            // Push          Source

            iInstrIndex = AddInstrLookup("Push", Constants.INSTR_PUSH, 1);
            SetOpType(Constants.OP_FLAG_TYPE_INT |
                                        Constants.OP_FLAG_TYPE_FLOAT |
                                        Constants.OP_FLAG_TYPE_STRING |
                                        Constants.OP_FLAG_TYPE_MEM_REF |
                                        Constants.OP_FLAG_TYPE_REG);

            // Pop           Destination

            iInstrIndex = AddInstrLookup("Pop", Constants.INSTR_POP, 1);
            SetOpType(Constants.OP_FLAG_TYPE_MEM_REF |
                                        Constants.OP_FLAG_TYPE_REG);

            // ---- The Function Interface

            // Call          FunctionName

            iInstrIndex = AddInstrLookup("Call", Constants.INSTR_CALL, 1);
            SetOpType(Constants.OP_FLAG_TYPE_FUNC_NAME);

            // Ret

            iInstrIndex = AddInstrLookup("Ret", Constants.INSTR_RET, 0);

            // CallHost      FunctionName

            iInstrIndex = AddInstrLookup("CallHost", Constants.INSTR_CALLHOST, 1);
            SetOpType(Constants.OP_FLAG_TYPE_HOST_API_CALL);

            // ---- Miscellaneous

            // Pause        Duration

            iInstrIndex = AddInstrLookup("Pause", Constants.INSTR_PAUSE, 1);
            SetOpType(Constants.OP_FLAG_TYPE_INT |
                                        Constants.OP_FLAG_TYPE_FLOAT |
                                        Constants.OP_FLAG_TYPE_STRING |
                                        Constants.OP_FLAG_TYPE_MEM_REF |
                                        Constants.OP_FLAG_TYPE_REG);

            // Exit         Code

            iInstrIndex = AddInstrLookup("Exit", Constants.INSTR_EXIT, 1);
            SetOpType(Constants.OP_FLAG_TYPE_INT |
                                        Constants.OP_FLAG_TYPE_FLOAT |
                                        Constants.OP_FLAG_TYPE_STRING |
                                        Constants.OP_FLAG_TYPE_MEM_REF |
                                        Constants.OP_FLAG_TYPE_REG);
        }

        public int AddInstrLookup(string strMnemonic, int iOpcode, int iOpCount)
        {
            InstrLookup temp = new InstrLookup();

            // Set the mnemonic, opcode and operand count fields
            temp.strMnemonic = new string(strMnemonic.ToUpper().ToCharArray());
            temp.iOpcode = iOpcode;
            temp.iOpCount = iOpCount;
            temp.OpList = new List<int>(iOpCount);

            g_InstrTable.Add(temp);

            // Return the used index to the caller
            int iReturnInstrIndex = g_InstrTable.Count;
            return iReturnInstrIndex;
        }

        public void SetOpType(int iOpType)
        {
            g_InstrTable[g_InstrTable.Count - 1].OpList.Add(iOpType);
        }

        public bool GetInstrByMnemonic(string strMnemonic, out InstrLookup Instr)
        {
            Instr = new InstrLookup();
            // Loop through each instruction in the lookup table
            for (int iCurrInstrIndex = 0; iCurrInstrIndex < Constants.MAX_INSTR_LOOKUP_COUNT; ++iCurrInstrIndex)
            {

                // Compare the instruction's mnemonic to the specified one
                if (g_InstrTable[iCurrInstrIndex].strMnemonic == strMnemonic)
                {
                    // Set the instruction definition to the user-specified pointer
                    Instr = g_InstrTable[iCurrInstrIndex];

                    // Return TRUE to signify success                
                    return true;
                }
            }
            // A match was not found, so return FALSE        
            return false;
        }

        public bool IsMnemonicaInstr(string strMnemonic)
        {
            foreach (var item in g_InstrTable)
            {
                if (item.strMnemonic == strMnemonic)
                    return true;
            }
            return false;
        }
    }
}