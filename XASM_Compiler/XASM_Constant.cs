namespace XASM
{
    public static class Constants
    {
        public enum ErrorCode
        {
            None,
            OK,
            Read,
            Assemble
        };

        public static string SomeConstant { get { return "Some value"; } }
        // ---- General ---------------------------------------------------------------------------
        public static int TRUE { get { return 1; } }           // True
        public static int FALSE { get { return 0; } }           // False

        // ---- Filename --------------------------------------------------------------------------

        public static int MAX_FILENAME_SIZE { get { return 2048; } }        // Maximum filename length

        public static string SOURCE_FILE_EXT { get { return ".XASM"; } }    // Extension of a source code file
        public static string EXEC_FILE_EXT { get { return ".XSE"; } }       // Extension of an executable code file

        // ---- Source Code -----------------------------------------------------------------------

        public static int MAX_SOURCE_CODE_SIZE { get { return 65536; } }        // Maximum number of lines in source

        public static int MAX_SOURCE_LINE_SIZE { get { return 4096; } }     // Maximum source line length

        // ---- ,XSE Header -----------------------------------------------------------------------

        public static string XSE_ID_STRING { get { return "XSE0"; } }       // Written to the file to state it's

        public static int VERSION_MAJOR { get { return 0; } }           // Major version number
        public static int VERSION_MINOR { get { return 8; } }           // Minor version number

        // ---- Lexer -----------------------------------------------------------------------------

        public static int MAX_LEXEME_SIZE { get { return 256; } }       // Maximum lexeme size

        public const int LEX_STATE_NO_STRING = 0;                           // Lexemes are scanned as normal
        public const int LEX_STATE_IN_STRING = 1;                           // Lexemes are scanned as strings
        public const int LEX_STATE_END_STRING = 2;                          // Lexemes are scanned as normal, and the
                                                                            // next state is LEXEME_STATE_NOSTRING

        public const int TOKEN_TYPE_INT = 0;            // An integer literal
        public const int TOKEN_TYPE_FLOAT = 1;          // An floating-point literal
        public const int TOKEN_TYPE_STRING = 2;         // An string literal
        public const int TOKEN_TYPE_QUOTE = 3;          // A double-quote
        public const int TOKEN_TYPE_IDENT = 4;          // An identifier
        public const int TOKEN_TYPE_COLON = 5;          // A colon
        public const int TOKEN_TYPE_OPEN_BRACKET = 6;           // An openening bracket
        public const int TOKEN_TYPE_CLOSE_BRACKET = 7;          // An closing bracket
        public const int TOKEN_TYPE_COMMA = 8;          // A comma
        public const int TOKEN_TYPE_OPEN_BRACE = 9;         // An openening curly brace
        public const int TOKEN_TYPE_CLOSE_BRACE = 10;           // An closing curly brace
        public const int TOKEN_TYPE_NEWLINE = 11;           // A newline

        public const int TOKEN_TYPE_INSTR = 12;         // An instruction

        public const int TOKEN_TYPE_SETSTACKSIZE = 13;           // The SetStackSize directive
        public const int TOKEN_TYPE_SETPRIORITY = 14;           // The SetPriority directive
        public const int TOKEN_TYPE_VAR = 15;           // The Var/Var [] directives
        public const int TOKEN_TYPE_FUNC = 16;          // The Func directives
        public const int TOKEN_TYPE_PARAM = 17;         // The Param directives
        public const int TOKEN_TYPE_REG_RETVAL = 18;            // The _RetVal directives

        public const int TOKEN_TYPE_INVALID = 19;           // Error code for invalid tokens
        public const int END_OF_TOKEN_STREAM = 20;          // The end of the stream has been
                                                            // reached
        public const int TOKEN_TYPE_REG_T0 = 21;            // The _RetVal directives
        public const int TOKEN_TYPE_REG_T1 = 22;            // The _RetVal directives
        public const int TOKEN_TYPE_REG_T2 = 23;            // The _RetVal directives

        public static int MAX_IDENT_SIZE { get { return 256; } }        // Maximum identifier size

        // ---- Instruction Lookup Table ----------------------------------------------------------

        public static int MAX_INSTR_LOOKUP_COUNT { get { return 256; } }        // The maximum number of instructions
                                                                                // the lookup table will hold
        public static int MAX_INSTR_MNEMONIC_SIZE { get { return 16; } }            // Maximum size of an instruction
                                                                                    // mnemonic's string

        // ---- Instruction Opcodes -----------------------------------------------------------

        public static int INSTR_MOV { get { return 0; } }

        public static int INSTR_ADD { get { return 1; } }
        public static int INSTR_SUB { get { return 2; } }
        public static int INSTR_MUL { get { return 3; } }
        public static int INSTR_DIV { get { return 4; } }
        public static int INSTR_MOD { get { return 5; } }
        public static int INSTR_EXP { get { return 6; } }
        public static int INSTR_NEG { get { return 7; } }
        public static int INSTR_INC { get { return 8; } }
        public static int INSTR_DEC { get { return 9; } }

        public static int INSTR_AND { get { return 10; } }
        public static int INSTR_OR { get { return 11; } }
        public static int INSTR_XOR { get { return 12; } }
        public static int INSTR_NOT { get { return 13; } }
        public static int INSTR_SHL { get { return 14; } }
        public static int INSTR_SHR { get { return 15; } }

        public static int INSTR_CONCAT { get { return 16; } }
        public static int INSTR_GETCHAR { get { return 17; } }
        public static int INSTR_SETCHAR { get { return 18; } }

        public static int INSTR_JMP { get { return 19; } }
        public static int INSTR_JE { get { return 20; } }
        public static int INSTR_JNE { get { return 21; } }
        public static int INSTR_JG { get { return 22; } }
        public static int INSTR_JL { get { return 23; } }
        public static int INSTR_JGE { get { return 24; } }
        public static int INSTR_JLE { get { return 25; } }

        public static int INSTR_PUSH { get { return 26; } }
        public static int INSTR_POP { get { return 27; } }

        public static int INSTR_CALL { get { return 28; } }
        public static int INSTR_RET { get { return 29; } }
        public static int INSTR_CALLHOST { get { return 30; } }

        public static int INSTR_PAUSE { get { return 31; } }
        public static int INSTR_EXIT { get { return 32; } }

        // ---- Operand Type Bitfield Flags ---------------------------------------------------

        // The following constants are used as flags into an operand type bit field, hence
        // their values being increasing powers of 2.

        public static int OP_FLAG_TYPE_INT { get { return 1; } }                // Integer literal value
        public static int OP_FLAG_TYPE_FLOAT { get { return 2; } }              // Floating-point literal value
        public static int OP_FLAG_TYPE_STRING { get { return 4; } }             // Integer literal value
        public static int OP_FLAG_TYPE_MEM_REF { get { return 8; } }                // Memory reference (variable or array
                                                                                    // index, both absolute and relative)
        public static int OP_FLAG_TYPE_LINE_LABEL { get { return 16; } }                // Line label (used for jumps)
        public static int OP_FLAG_TYPE_FUNC_NAME { get { return 32; } }             // Function table index (used for Call)
        public static int OP_FLAG_TYPE_HOST_API_CALL { get { return 64; } }             // Host API Call table index (used for
                                                                                        // CallHost)
        public static int OP_FLAG_TYPE_REG { get { return 128; } }          // Register

        // ---- Assembled Instruction Stream ------------------------------------------------------

        public const int OP_TYPE_INT = 0;                               // Integer literal value
        public const int OP_TYPE_FLOAT = 1;                             // Floating-point literal value
        public const int OP_TYPE_STRING_INDEX = 2;                              // String literal value
        public const int OP_TYPE_ABS_STACK_INDEX = 3;                               // Absolute array index
        public const int OP_TYPE_REL_STACK_INDEX = 4;                               // Relative array index
        public const int OP_TYPE_INSTR_INDEX = 5;                               // Instruction index
        public const int OP_TYPE_FUNC_INDEX = 6;                                // Function index
        public const int OP_TYPE_HOST_API_CALL_INDEX = 7;                               // Host API call index
        public const int OP_TYPE_REG = 8;                               // Register

        // ---- Built Instruction Stream ----------------------------------------------------------

        public const string OP_TYPE_INT_BUILT = "INT";                    // Integer literal value
        public const string OP_TYPE_FLOAT_BUILT = "FLT";                    // Floating-point literal value
        public const string OP_TYPE_STRING_INDEX_BUILT = "STR";                 // String literal value
        public const string OP_TYPE_STACK_INDEX_BUILT = "STA";                  // array index
        public const string OP_TYPE_INSTR_INDEX_BUILT = "INS";                  // Instruction index
        public const string OP_TYPE_FUNC_INDEX_BUILT = "FUN";                   // Function index
        public const string OP_TYPE_HOST_API_CALL_INDEX_BUILT = "API";                  // Host API call index
        public const string OP_TYPE_REG_BUILT = "REG";                  // Register

        // ---- Priority Types --------------------------------------------------------------------

        public static int PRIORITY_USER { get { return 0; } }               // User-defined priority
        public static int PRIORITY_LOW { get { return 1; } }               // Low priority
        public static int PRIORITY_MED { get { return 2; } }               // Medium priority
        public static int PRIORITY_HIGH { get { return 3; } }               // High priority

        public static string PRIORITY_LOW_KEYWORD { get { return "LOW"; } }           // Low priority keyword
        public static string PRIORITY_MED_KEYWORD { get { return "MED"; } }           // Low priority keyword
        public static string PRIORITY_HIGH_KEYWORD { get { return "HIGH"; } }          // Low priority keyword

        // ---- Functions -------------------------------------------------------------------------

        public static string MAIN_FUNC_NAME { get { return "_MAIN"; } }     // _Main ()'s name

        // ---- Error Strings ---------------------------------------------------------------------

        // The following macros are used to represent assembly-time error strings

        public static string ERROR_MSSG_INVALID_INPUT { get { return "Invalid input"; } }

        public static string ERROR_MSSG_LOCAL_SETSTACKSIZE { get { return "SetStackSize can only appear in the global scope"; } }

        public static string ERROR_MSSG_INVALID_STACK_SIZE { get { return "Invalid stack size"; } }

        public static string ERROR_MSSG_MULTIPLE_SETSTACKSIZES { get { return "Multiple instances of SetStackSize illegal"; } }

        public static string ERROR_MSSG_LOCAL_SETPRIORITY { get { return "SetPriority can only appear in the global scope"; } }

        public static string ERROR_MSSG_INVALID_PRIORITY { get { return "Invalid priority"; } }

        public static string ERROR_MSSG_MULTIPLE_SETPRIORITIES { get { return "Multiple instances of SetPriority illegal"; } }

        public static string ERROR_MSSG_IDENT_EXPECTED { get { return "Identifier expected"; } }

        public static string ERROR_MSSG_INVALID_ARRAY_SIZE { get { return "Invalid array size"; } }

        public static string ERROR_MSSG_IDENT_REDEFINITION { get { return "Identifier redefinition"; } }

        public static string ERROR_MSSG_UNDEFINED_IDENT { get { return "Undefined identifier"; } }

        public static string ERROR_MSSG_NESTED_FUNC { get { return "Nested functions illegal"; } }

        public static string ERROR_MSSG_FUNC_REDEFINITION { get { return "Function redefinition"; } }

        public static string ERROR_MSSG_UNDEFINED_FUNC { get { return "Undefined function"; } }

        public static string ERROR_MSSG_GLOBAL_PARAM { get { return "Parameters can only appear inside functions"; } }

        public static string ERROR_MSSG_MAIN_PARAM { get { return "_Main () function cannot accept parameters"; } }

        public static string ERROR_MSSG_GLOBAL_LINE_LABEL { get { return "Line labels can only appear inside functions"; } }

        public static string ERROR_MSSG_LINE_LABEL_REDEFINITION { get { return "Line label redefinition"; } }

        public static string ERROR_MSSG_UNDEFINED_LINE_LABEL { get { return "Undefined line label"; } }

        public static string ERROR_MSSG_GLOBAL_INSTR { get { return "Instructions can only appear inside functions"; } }

        public static string ERROR_MSSG_INVALID_INSTR { get { return "Invalid instruction"; } }

        public static string ERROR_MSSG_INVALID_OP { get { return "Invalid operand"; } }

        public static string ERROR_MSSG_INVALID_STRING { get { return "Invalid string"; } }

        public static string ERROR_MSSG_INVALID_ARRAY_NOT_INDEXED { get { return "Arrays must be indexed"; } }

        public static string ERROR_MSSG_INVALID_ARRAY { get { return "Invalid array"; } }

        public static string ERROR_MSSG_INVALID_ARRAY_INDEX { get { return "Invalid array index"; } }

    }
}