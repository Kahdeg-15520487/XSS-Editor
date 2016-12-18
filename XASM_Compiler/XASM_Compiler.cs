using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System;
using Newtonsoft.Json;

namespace XASM
{
    public class XASM_Compiler
    {
        public Constants.ErrorCode errorcode = Constants.ErrorCode.None;
        string scriptPath;

        List<string> fileLines;
        List<Instruction> g_pInstrStream;

        XASM_InstructionTable instrtable;
        XASM_FunctionTable functable;
        XASM_LabelTable labeltable;
        XASM_SymbolTable symtable;
        XASM_StringTable strtable;
        XASM_HostAPITable hostapitable;


        Lexer g_Lexer;
        int g_iSourceCodeSize;
        ScriptHeader g_ScriptHeader;
        int g_iInstrStreamSize;
        int g_iCurrInstrIndex;
        bool g_iIsSetStackSizeFound;
        bool g_iIsSetPriorityFound;

        public XASM_Compiler() { }
        public XASM_Compiler(string script,bool isFromFile)
        {
            InitTable();
            ReadFile(script,isFromFile);
            scriptPath = new string(script.ToCharArray());
        }

        public void Compile()
        {
            AssmblSourceFile();
            BuildXSE(null);
            Shutdown();
        }

        public void Compile(string filename)
        {
            AssmblSourceFile();
            BuildXSE(filename);
            Shutdown();
        }

        void InitTable()
        {
            g_Lexer = new Lexer();
            instrtable = new XASM_InstructionTable();
            instrtable.InitInstrTable();
            functable = new XASM_FunctionTable();
            hostapitable = new XASM_HostAPITable();
            labeltable = new XASM_LabelTable();
            strtable = new XASM_StringTable();
            symtable = new XASM_SymbolTable();
            g_ScriptHeader = new ScriptHeader();
            Console.WriteLine("initialized");
        }

		//load the script from an outside file
        void ReadFile(string script,bool isFromFile)
        {
            if (isFromFile)
            {
                StreamReader sr;
                try
                {
                    sr = File.OpenText(script);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    Console.ReadLine();
                    errorcode = Constants.ErrorCode.Read;
                    return;
                    throw;
                }
                if (errorcode == Constants.ErrorCode.None)
                {
                    fileLines = sr.ReadToEnd().Split('\n').ToList();
                    sr.Close();
                    g_iSourceCodeSize = fileLines.Count;
                    Console.WriteLine("finished reading");
                    errorcode = Constants.ErrorCode.OK;
                    Prepare_Source_Code();
                }
            }
            else
            {
                fileLines = script.Split('\n').ToList();
                g_iSourceCodeSize = fileLines.Count;
                Console.WriteLine("finished reading");
                Prepare_Source_Code();
            }
        }

        void Prepare_Source_Code()
        {
			//strip all comments
            for (int i = 0; i < fileLines.Count; i++)
                fileLines[i] = new string(XASM_String_Process.StripComments(fileLines[i]).ToCharArray());
			
			//trim all unnecessary whitespace
            for (int i = 0; i < fileLines.Count; i++)
                fileLines[i] = new string(XASM_String_Process.TrimWhitespace(fileLines[i]).ToCharArray());

			//append a newline char at the end of each line
            for (int i = 0; i < fileLines.Count; i++)
            {
                var buildertemp = new StringBuilder();
                buildertemp.Append(fileLines[i]);
                buildertemp.Append('\n');
                fileLines[i] = buildertemp.ToString();
            }
        }

        int GetNextToken()
        {
            // ---- Lexeme Extraction
            // Move the first index (Index0) past the end of the last token, which is marked
            // by the second index (Index1).
            g_Lexer.iIndex0 = g_Lexer.iIndex1;

            // Make sure we aren't past the end of the current line. If a string is 8 characters long,
            // it's indexed from 0 to 7; therefore, indices 8 and beyond lie outside of the string and
            // require us to move to the next line. This is why I use >= for the comparison rather
            // than >. The value returned by Length is always one greater than the last valid
            // character index.
            //The while loop will hop over all those emptylines and stuff
            while (g_Lexer.iIndex0 >= fileLines[g_Lexer.iCurrSourceLine].Length)
            {
                // If so, skip to the next line but make sure we don't go past the end of the file.
                // SkipToNextLine () will return false if we hit the end of the file, which is
                // the end of the token stream.
                if (!SkipToNextLine())
                    return Constants.END_OF_TOKEN_STREAM;
            }

            // If we just ended a string, tell the lexer to stop lexing
            // strings and return to the normal state
            if (g_Lexer.iCurrLexState == Constants.LEX_STATE_END_STRING)
                g_Lexer.iCurrLexState = Constants.LEX_STATE_NO_STRING;

            // Scan through the potential whitespace preceding the next lexeme, but ONLY if we're
            // not currently parsing a string lexeme (since strings can contain arbitrary whitespace
            // which must be preserved).
            if (g_Lexer.iCurrLexState != Constants.LEX_STATE_IN_STRING)
            {
                // Scan through the whitespace and check for the end of the line
                while (true)
                {
                    // If the current character is not whitespace, exit the loop because the lexeme
                    // is starting.
                    if (!XASM_String_Process.IsCharWhitespace(fileLines[g_Lexer.iCurrSourceLine][g_Lexer.iIndex0]))
                        break;
                    // It is whitespace, however, so move to the next character and continue scanning
                    ++g_Lexer.iIndex0;
                }
            }

            // Bring the second index (Index1) to the lexeme's starting character, which is marked by
            // the first index (Index0)
            g_Lexer.iIndex1 = g_Lexer.iIndex0;

            // Scan through the lexeme until a delimiter is hit, incrementing Index1 each time
            while (true)
            {
                // Are we currently scanning through a string?
                if (g_Lexer.iCurrLexState == Constants.LEX_STATE_IN_STRING)
                {
                    // If we're at the end of the line, return an invalid token since the string has no
                    // ending double-quote on the line
                    if (g_Lexer.iIndex1 >= fileLines[g_Lexer.iCurrSourceLine].Length)
                    {
                        g_Lexer.CurrToken = Constants.TOKEN_TYPE_INVALID;
                        return g_Lexer.CurrToken;
                    }

                    // If the current character is a backslash, move ahead two characters to skip the
                    // escape sequence and jump to the next iteration of the loop
                    if (fileLines[g_Lexer.iCurrSourceLine][g_Lexer.iIndex1] == '\\')
                    {
                        g_Lexer.iIndex1 += 2;
                        continue;
                    }

                    // If the current character isn't a double-quote, move to the next, otherwise exit
                    // the loop, because the string has ended.
                    if (fileLines[g_Lexer.iCurrSourceLine][g_Lexer.iIndex1] == '"')
                        break;

                    ++g_Lexer.iIndex1;
                }

                // We are not currently scanning through a string
                else
                {
                    // If we're at the end of the line, the lexeme has ended so exit the loop
                    if (g_Lexer.iIndex1 >= fileLines[g_Lexer.iCurrSourceLine].Length)
                        break;

                    // If the current character isn't a delimiter, move to the next, otherwise exit the loop
                    if (XASM_String_Process.IsCharDelimiter(fileLines[g_Lexer.iCurrSourceLine][g_Lexer.iIndex1]))
                        break;

                    ++g_Lexer.iIndex1;
                }
            }

            // Single-character lexemes will appear to be zero characters at this point (since Index1
            // will equal Index0), so move Index1 over by one to give it some noticable width
            if (g_Lexer.iIndex1 - g_Lexer.iIndex0 == 0)
                ++g_Lexer.iIndex1;

            // The lexeme has been isolated and lies between Index0 and Index1 (inclusive), so make a local
            // copy for the lexer
            string temp = fileLines[g_Lexer.iCurrSourceLine].Substring(g_Lexer.iIndex0, g_Lexer.iIndex1 - g_Lexer.iIndex0);
            var builder = new StringBuilder();
            foreach (var c in temp)
            {
                // If we're parsing a string, check for escape sequences and just copy the character after
                // the backslash
                if (g_Lexer.iCurrLexState == Constants.LEX_STATE_IN_STRING)
                    if (c == '\\')
                        continue;
                // Copy the character from the source line to the lexeme
                builder.Append(c);
            }
            // Set the null terminator
            g_Lexer.strCurrLexeme = builder.ToString();

            // Convert it to uppercase if it's not a string
            if (g_Lexer.iCurrLexState != Constants.LEX_STATE_IN_STRING)
                g_Lexer.strCurrLexeme = g_Lexer.strCurrLexeme.ToUpper();

            // ---- Token Identification
            // Let's find out what sort of token our new lexeme is
            // We'll set the type to invalid now just in case the lexer doesn't match any
            // token types
            g_Lexer.CurrToken = Constants.TOKEN_TYPE_INVALID;

            // The first case is the easiest-- if the string lexeme state is active, we know we're
            // dealing with a string token. However, if the string is the double-quote sign, it
            // means we've read an empty string and should return a double-quote instead
            if (g_Lexer.strCurrLexeme.Length > 1 || g_Lexer.strCurrLexeme[0] != '"')
            {
                if (g_Lexer.iCurrLexState == Constants.LEX_STATE_IN_STRING)
                {
                    g_Lexer.CurrToken = Constants.TOKEN_TYPE_STRING;
                    return Constants.TOKEN_TYPE_STRING;
                }
            }

            // Now let's check for the single-character tokens
            if (g_Lexer.strCurrLexeme.Length == 1)
            {
                switch (g_Lexer.strCurrLexeme[0])
                {
                    // Double-Quote
                    case '"':
                        // If a quote is read, advance the lexing state so that strings are lexed
                        // properly
                        switch (g_Lexer.iCurrLexState)
                        {
                            // If we're not lexing strings, tell the lexer we're now
                            // in a string
                            case Constants.LEX_STATE_NO_STRING:
                                g_Lexer.iCurrLexState = Constants.LEX_STATE_IN_STRING;
                                break;

                            // If we're in a string, tell the lexer we just ended a string
                            case Constants.LEX_STATE_IN_STRING:
                                g_Lexer.iCurrLexState = Constants.LEX_STATE_END_STRING;
                                break;
                        }

                        g_Lexer.CurrToken = Constants.TOKEN_TYPE_QUOTE;
                        break;

                    // Comma
                    case ',':
                        g_Lexer.CurrToken = Constants.TOKEN_TYPE_COMMA;
                        break;

                    // Colon
                    case ':':
                        g_Lexer.CurrToken = Constants.TOKEN_TYPE_COLON;
                        break;

                    // Opening Bracket
                    case '[':
                        g_Lexer.CurrToken = Constants.TOKEN_TYPE_OPEN_BRACKET;
                        break;

                    // Closing Bracket
                    case ']':
                        g_Lexer.CurrToken = Constants.TOKEN_TYPE_CLOSE_BRACKET;
                        break;

                    // Opening Brace
                    case '{':
                        g_Lexer.CurrToken = Constants.TOKEN_TYPE_OPEN_BRACE;
                        break;

                    // Closing Brace
                    case '}':
                        g_Lexer.CurrToken = Constants.TOKEN_TYPE_CLOSE_BRACE;
                        break;

                    // Newline
                    case '\n':
                        g_Lexer.CurrToken = Constants.TOKEN_TYPE_NEWLINE;
                        break;
                }
            }

            // Now let's check for the multi-character tokens
            // Is it an integer?
            if (XASM_String_Process.IsStringInteger(g_Lexer.strCurrLexeme))
                g_Lexer.CurrToken = Constants.TOKEN_TYPE_INT;

            // Is it a float?
            if (XASM_String_Process.IsStringFloat(g_Lexer.strCurrLexeme))
                g_Lexer.CurrToken = Constants.TOKEN_TYPE_FLOAT;

            // Is it an identifier (which may also be a line label or instruction)?
            if (XASM_String_Process.IsStringIdent(g_Lexer.strCurrLexeme))
                g_Lexer.CurrToken = Constants.TOKEN_TYPE_IDENT;

            // Check for directives or _RetVal
            // Is it SetStackSize?
            if (g_Lexer.strCurrLexeme == "SETSTACKSIZE")
                g_Lexer.CurrToken = Constants.TOKEN_TYPE_SETSTACKSIZE;

            // Is it SetPriority?
            if (g_Lexer.strCurrLexeme == "SETPRIORITY")
                g_Lexer.CurrToken = Constants.TOKEN_TYPE_SETPRIORITY;

            // Is it Var/Var []?
            if (g_Lexer.strCurrLexeme == "VAR")
                g_Lexer.CurrToken = Constants.TOKEN_TYPE_VAR;

            // Is it Func?
            if (g_Lexer.strCurrLexeme == "FUNC")
                g_Lexer.CurrToken = Constants.TOKEN_TYPE_FUNC;

            // Is it Param?
            if (g_Lexer.strCurrLexeme == "PARAM")
                g_Lexer.CurrToken = Constants.TOKEN_TYPE_PARAM;

            // Is it _RetVal?
            if (g_Lexer.strCurrLexeme == "_RETVAL")
                g_Lexer.CurrToken = Constants.TOKEN_TYPE_REG_RETVAL;

            if (g_Lexer.strCurrLexeme == "_T0")
                g_Lexer.CurrToken = Constants.TOKEN_TYPE_REG_T0;

            if (g_Lexer.strCurrLexeme == "_T1")
                g_Lexer.CurrToken = Constants.TOKEN_TYPE_REG_T1;

            if (g_Lexer.strCurrLexeme == "_T2")
                g_Lexer.CurrToken = Constants.TOKEN_TYPE_REG_T2;

            // Is it an instruction?
            if (instrtable.IsMnemonicaInstr(g_Lexer.strCurrLexeme))
                g_Lexer.CurrToken = Constants.TOKEN_TYPE_INSTR;

            return g_Lexer.CurrToken;
        }

        char GetLookAheadChar()
        {
            // We don't actually want to move the lexer's indices, so we'll make a copy of them
            int iCurrSourceLine = g_Lexer.iCurrSourceLine;
            int iIndex = g_Lexer.iIndex1;

            // If the next lexeme is not a string, scan past any potential leading whitespace
            if (g_Lexer.iCurrLexState != Constants.LEX_STATE_IN_STRING)
            {
                // Scan through the whitespace and check for the end of the line
                while (true)
                {
                    // If we've passed the end of the line, skip to the next line and reset the
                    // index to zero
                    while (iIndex >= fileLines[iCurrSourceLine].Length)
                    {
                        // Increment the source code index
                        iCurrSourceLine += 1;

                        // If we've passed the end of the source file, just return a null character
                        if (iCurrSourceLine >= g_iSourceCodeSize)
                            return '\0';

                        // Otherwise, reset the index to the first character on the new line
                        iIndex = 0;
                    }

                    // If the current character is not whitespace, return it, since it's the first
                    // character of the next lexeme and is thus the look-ahead
                    if (!XASM_String_Process.IsCharWhitespace(fileLines[iCurrSourceLine][iIndex]))
                        break;

                    // It is whitespace, however, so move to the next character and continue scanning
                    ++iIndex;
                }
            }

            // Return whatever character the loop left iIndex at
            return fileLines[iCurrSourceLine][iIndex];
        }

        bool SkipToNextLine()
        {
            // Increment the current line
            g_Lexer.iCurrSourceLine += 1;

            // Return false if we've gone past the end of the source code
            if (g_Lexer.iCurrSourceLine >= g_iSourceCodeSize)
                return false;

            // Set both indices to point to the start of the string
            g_Lexer.iIndex0 = 0;
            g_Lexer.iIndex1 = 0;

            // Turn off string lexeme mode, since strings can't span multiple lines
            g_Lexer.iCurrLexState = Constants.LEX_STATE_NO_STRING;

            // Return true to indicate success
            return true;
        }

        void AssmblSourceFile()
        {
            // ---- Initialize the script header
            g_ScriptHeader.iStackSize = 0;
            g_ScriptHeader.iIsMainFuncPresent = false;

            // ---- Set some initial variables
            g_iInstrStreamSize = 0;
            g_iIsSetStackSizeFound = false;
            g_iIsSetPriorityFound = false;
            g_ScriptHeader.iGlobalDataSize = 0;

            // Set the current function's flags and variables
            bool iIsFuncActive = false;
            int CurrFunc = 0;
            int iCurrFuncIndex = 0;
            string strCurrFuncName = new string(' ', 1);
            int iCurrFuncParamCount = 0;
            int iCurrFuncLocalDataSize = 0;

            // Create an instruction definition structure to hold instruction information when
            // dealing with instructions.
            InstrLookup CurrInstr;

            // ---- Perform first pass over the source
            // Reset the lexer
            g_Lexer.ResetLexer();

            // Loop through each line of code
            while (true)
            {
                // Get the next token and make sure we aren't at the end of the stream
                if (GetNextToken() == Constants.END_OF_TOKEN_STREAM)
                    break;

                // Check the initial token
                switch (g_Lexer.CurrToken)
                {
                    // ---- Start by checking for directives
                    // SetStackSize
                    case Constants.TOKEN_TYPE_SETSTACKSIZE:

                        // SetStackSize can only be found in the global scope, so make sure we
                        // aren't in a function.
                        if (iIsFuncActive)
                            ExitOnCodeError(Constants.ERROR_MSSG_LOCAL_SETSTACKSIZE);

                        // It can only be found once, so make sure we haven't already found it
                        if (g_iIsSetStackSizeFound)
                            ExitOnCodeError(Constants.ERROR_MSSG_MULTIPLE_SETSTACKSIZES);

                        // Read the next lexeme, which should contain the stack size
                        if (GetNextToken() != Constants.TOKEN_TYPE_INT)
                            ExitOnCodeError(Constants.ERROR_MSSG_INVALID_STACK_SIZE);

                        // Convert the lexeme to an integer value from its string
                        // representation and store it in the script header
                        g_ScriptHeader.iStackSize = Convert.ToInt32(g_Lexer.strCurrLexeme);

                        // Mark the presence of SetStackSize for future encounters
                        g_iIsSetStackSizeFound = true;

                        break;

                    // SetPriority
                    case Constants.TOKEN_TYPE_SETPRIORITY:

                        // SetPriority can only be found in the global scope, so make sure we
                        // aren't in a function.
                        if (iIsFuncActive)
                            ExitOnCodeError(Constants.ERROR_MSSG_LOCAL_SETPRIORITY);

                        // It can only be found once, so make sure we haven't already found it
                        if (g_iIsSetPriorityFound)
                            ExitOnCodeError(Constants.ERROR_MSSG_MULTIPLE_SETPRIORITIES);

                        GetNextToken();

                        // Determine
                        switch (g_Lexer.CurrToken)
                        {
                            // An integer lexeme means the user is defining a specific priority
                            case Constants.TOKEN_TYPE_INT:

                                // Convert the lexeme to an integer value from its string
                                // representation and store it in the script header
                                g_ScriptHeader.iUserPriority = Convert.ToInt32(g_Lexer.strCurrLexeme);

                                // Set the user priority flag
                                g_ScriptHeader.iPriorityType = Constants.PRIORITY_USER;

                                break;

                            // An identifier means it must be one of the predefined priority
                            // ranks
                            case Constants.TOKEN_TYPE_IDENT:

                                // Determine which rank was specified
                                if (g_Lexer.strCurrLexeme == Constants.PRIORITY_LOW_KEYWORD)
                                    g_ScriptHeader.iPriorityType = Constants.PRIORITY_LOW;
                                else if (g_Lexer.strCurrLexeme == Constants.PRIORITY_MED_KEYWORD)
                                    g_ScriptHeader.iPriorityType = Constants.PRIORITY_MED;
                                else if (g_Lexer.strCurrLexeme == Constants.PRIORITY_HIGH_KEYWORD)
                                    g_ScriptHeader.iPriorityType = Constants.PRIORITY_HIGH;
                                else
                                {
                                    Console.WriteLine("-" + g_Lexer.strCurrLexeme + "-");
                                    ExitOnCodeError(Constants.ERROR_MSSG_INVALID_PRIORITY);
                                }
                                break;

                            // Anything else should cause an error
                            default:
                                ExitOnCodeError(Constants.ERROR_MSSG_INVALID_PRIORITY);
                                break;
                        }

                        // Mark the presence of SetStackSize for future encounters
                        g_iIsSetPriorityFound = true;

                        break;

                    // Var/Var []
                    case Constants.TOKEN_TYPE_VAR:
                        {
                            // Get the variable's identifier
                            if (GetNextToken() != Constants.TOKEN_TYPE_IDENT)
                                ExitOnCodeError(Constants.ERROR_MSSG_IDENT_EXPECTED);

                            string strIdent = new string(g_Lexer.strCurrLexeme.ToCharArray());

                            // Now determine its size by finding out if it's an array or not, otherwise
                            // default to 1.
                            int iSize = 1;

                            // Find out if an opening bracket lies ahead
                            if (GetLookAheadChar() == '[')
                            {
                                // Validate and consume the opening bracket
                                if (GetNextToken() != Constants.TOKEN_TYPE_OPEN_BRACKET)
                                    ExitOnCharExpectedError('[');

                                // We're parsing an array, so the next lexeme should be an integer
                                // describing the array's size
                                if (GetNextToken() != Constants.TOKEN_TYPE_INT)
                                    ExitOnCodeError(Constants.ERROR_MSSG_INVALID_ARRAY_SIZE);

                                // Convert the size lexeme to an integer value
                                iSize = Convert.ToInt32(g_Lexer.strCurrLexeme);

                                // Make sure the size is valid, in that it's greater than zero
                                if (iSize <= 0)
                                    ExitOnCodeError(Constants.ERROR_MSSG_INVALID_ARRAY_SIZE);

                                // Make sure the closing bracket is present as well
                                if (GetNextToken() != Constants.TOKEN_TYPE_CLOSE_BRACKET)
                                    ExitOnCharExpectedError(']');
                            }

                            // Determine the variable's index into the stack
                            // If the variable is local, then its stack index is always the local data
                            // size + 2 subtracted from zero
                            int iStackIndex;

                            if (iIsFuncActive)
                                iStackIndex = -(iCurrFuncLocalDataSize + 2);

                            // Otherwise it's global, so it's equal to the current global data size
                            else
                                iStackIndex = g_ScriptHeader.iGlobalDataSize;

                            // Attempt to add the symbol to the table
                            if (symtable.AddSymbol(strIdent, iSize, iStackIndex, iCurrFuncIndex) == 9999)
                            {
                                ExitOnCodeError(Constants.ERROR_MSSG_IDENT_REDEFINITION);
                            }
                            // Depending on the scope, increment either the local or global data size
                            // by the size of the variable
                            if (iIsFuncActive)
                                iCurrFuncLocalDataSize += iSize;
                            else
                                g_ScriptHeader.iGlobalDataSize += iSize;

                            break;
                        }

                    // Func
                    case Constants.TOKEN_TYPE_FUNC:
                        {
                            // First make sure we aren't in a function already, since nested functions
                            // are illegal
                            if (iIsFuncActive)
                                ExitOnCodeError(Constants.ERROR_MSSG_NESTED_FUNC);

                            // Read the next lexeme, which is the function name
                            if (GetNextToken() != Constants.TOKEN_TYPE_IDENT)
                                ExitOnCodeError(Constants.ERROR_MSSG_IDENT_EXPECTED);

                            string strFuncName = new string(g_Lexer.strCurrLexeme.ToCharArray());


                            // Calculate the function's entry point, which is the instruction immediately
                            // following the current one, which is in turn equal to the instruction stream
                            // size
                            int iEntryPoint = g_iInstrStreamSize;

                            // Try adding it to the function table, and print an error if it's already
                            // been declared
                            int iFuncIndex = functable.AddFunc(strFuncName, iEntryPoint);
                            if (iFuncIndex == -1)
                                ExitOnCodeError(Constants.ERROR_MSSG_FUNC_REDEFINITION);

                            // Is this the _Main () function?
                            if (strFuncName == Constants.MAIN_FUNC_NAME)
                            {
                                g_ScriptHeader.iIsMainFuncPresent = true;
                                g_ScriptHeader.iMainFuncIndex = iFuncIndex;
                            }

                            // Set the function flag to true for any future encounters and re-initialize
                            // function tracking variables
                            iIsFuncActive = true;
                            strCurrFuncName = new string(strFuncName.ToCharArray());
                            iCurrFuncIndex = iFuncIndex;
                            iCurrFuncParamCount = 0;
                            iCurrFuncLocalDataSize = 0;

                            // Read any number of line breaks until the opening brace is found
                            while (GetNextToken() == Constants.TOKEN_TYPE_NEWLINE) ;

                            // Make sure the lexeme was an opening brace
                            if (g_Lexer.CurrToken != Constants.TOKEN_TYPE_OPEN_BRACE)
                                ExitOnCharExpectedError('{');

                            // All functions are automatically appended with Ret, so increment the
                            // required size of the instruction stream
                            ++g_iInstrStreamSize;

                            break;
                        }

                    // Closing bracket
                    case Constants.TOKEN_TYPE_CLOSE_BRACE:

                        // This should be closing a function, so make sure we're in one
                        if (!iIsFuncActive)
                            ExitOnCharExpectedError('}');

                        // Set the fields we've collected
                        functable.SetFuncInfo(strCurrFuncName, iCurrFuncParamCount, iCurrFuncLocalDataSize);

                        // Close the function
                        iIsFuncActive = false;

                        break;

                    // Param
                    case Constants.TOKEN_TYPE_PARAM:
                        {
                            // If we aren't currently in a function, print an error
                            if (!iIsFuncActive)
                                ExitOnCodeError(Constants.ERROR_MSSG_GLOBAL_PARAM);

                            // _Main () can't accept parameters, so make sure we aren't in it
                            if (strCurrFuncName == Constants.MAIN_FUNC_NAME)
                                ExitOnCodeError(Constants.ERROR_MSSG_MAIN_PARAM);

                            // The parameter's identifier should follow
                            if (GetNextToken() != Constants.TOKEN_TYPE_IDENT)
                                ExitOnCodeError(Constants.ERROR_MSSG_IDENT_EXPECTED);

                            // Increment the current function's local data size
                            ++iCurrFuncParamCount;

                            break;
                        }

                    // ---- Instructions
                    case Constants.TOKEN_TYPE_INSTR:
                        {
                            // Make sure we aren't in the global scope, since instructions
                            // can only appear in functions
                            if (!iIsFuncActive)
                                ExitOnCodeError(Constants.ERROR_MSSG_GLOBAL_INSTR);

                            // Increment the instruction stream size
                            ++g_iInstrStreamSize;

                            break;
                        }

                    // ---- Identifiers (line labels)
                    case Constants.TOKEN_TYPE_IDENT:
                        {
                            // Make sure it's a line label
                            if (GetLookAheadChar() != ':')
                                ExitOnCodeError(Constants.ERROR_MSSG_INVALID_INSTR);

                            // Make sure we're in a function, since labels can only appear there
                            if (!iIsFuncActive)
                                ExitOnCodeError(Constants.ERROR_MSSG_GLOBAL_LINE_LABEL);

                            // The current lexeme is the label's identifier
                            string strIdent = new string(g_Lexer.strCurrLexeme.ToCharArray());

                            // The target instruction is always the value of the current
                            // instruction count, which is the current size - 1
                            int iTargetIndex = g_iInstrStreamSize - 1;

                            // Save the label's function index as well
                            int iFuncIndex = iCurrFuncIndex;

                            // Try adding the label to the label table, and print an error if it
                            // already exists
                            if (labeltable.AddLabel(strIdent, iTargetIndex, iFuncIndex) == -1)
                                ExitOnCodeError(Constants.ERROR_MSSG_LINE_LABEL_REDEFINITION);

                            break;
                        }

                    default:

                        // Anything else should cause an error, minus line breaks
                        if (g_Lexer.CurrToken != Constants.TOKEN_TYPE_NEWLINE)
                            ExitOnCodeError(Constants.ERROR_MSSG_INVALID_INPUT);
                        break;
                }

                // Skip to the next line, since the initial tokens are all we're really worrid
                // about in this phase

                if (!SkipToNextLine())
                    break;
            }

            // We counted the instructions, so allocate the assembled instruction stream array
            // so the next phase can begin
            g_pInstrStream = new List<Instruction>();

            // Initialize every operand list pointer to NULL
            for (int iCurrInstrIndex = 0; iCurrInstrIndex < g_iInstrStreamSize; ++iCurrInstrIndex)
            {
                g_pInstrStream.Add(new Instruction());
                g_pInstrStream[iCurrInstrIndex].OpList = new List<Operand>();
            }
            // Set the current instruction index to zero
            g_iCurrInstrIndex = 0;

            // ---- Perform the second pass over the source
            // Reset the lexer so we begin at the top of the source again

            g_Lexer.ResetLexer();

            // Loop through each line of code
            while (true)
            {
                // Get the next token and make sure we aren't at the end of the stream

                if (GetNextToken() == Constants.END_OF_TOKEN_STREAM)
                    break;

                // Check the initial token

                switch (g_Lexer.CurrToken)
                {
                    // Func

                    case Constants.TOKEN_TYPE_FUNC:
                        {
                            // We've encountered a Func directive, but since we validated the syntax
                            // of all functions in the previous phase, we don't need to perform any
                            // error handling here and can assume the syntax is perfect.

                            // Read the identifier
                            GetNextToken();

                            // Use the identifier (the current lexeme) to get it's corresponding function
                            // from the table
                            CurrFunc = functable.GetFuncByName(g_Lexer.strCurrLexeme);

                            // Set the active function flag
                            iIsFuncActive = true;

                            // Set the parameter count to zero, since we'll need to count parameters as
                            // we parse Param directives
                            iCurrFuncParamCount = 0;

                            // Save the function's index
                            iCurrFuncIndex = CurrFunc;

                            // Read any number of line breaks until the opening brace is found
                            while (GetNextToken() == Constants.TOKEN_TYPE_NEWLINE) ;

                            break;
                        }

                    // Closing brace
                    case Constants.TOKEN_TYPE_CLOSE_BRACE:
                        {
                            // Clear the active function flag
                            iIsFuncActive = false;

                            // If the ending function is _Main (), append an Exit instruction
                            if (functable.g_FuncTable[CurrFunc].strName == Constants.MAIN_FUNC_NAME)
                            {
                                // First set the opcode
                                g_pInstrStream[g_iCurrInstrIndex].iOpcode = Constants.INSTR_EXIT;

                                // Now set the operand count
                                g_pInstrStream[g_iCurrInstrIndex].iOpCount = 1;

                                // Now set the return code by allocating space for a single operand and
                                // setting it to zero
                                g_pInstrStream[g_iCurrInstrIndex].OpList = new List<Operand>();
                                g_pInstrStream[g_iCurrInstrIndex].OpList.Add(new Operand());
                                g_pInstrStream[g_iCurrInstrIndex].OpList[0].operand.iType = Constants.OP_TYPE_INT;
                                g_pInstrStream[g_iCurrInstrIndex].OpList[0].operand.iIntLiteral = 0;
                            }

                            // Otherwise append a Ret instruction and make sure to NULLify the operand
                            // list pointer
                            else
                            {
                                g_pInstrStream[g_iCurrInstrIndex].iOpcode = Constants.INSTR_RET;
                                g_pInstrStream[g_iCurrInstrIndex].iOpCount = 0;
                                g_pInstrStream[g_iCurrInstrIndex].OpList = new List<Operand>();
                            }

                            ++g_iCurrInstrIndex;

                            break;
                        }

                    // Param
                    case Constants.TOKEN_TYPE_PARAM:
                        {
                            // Read the next token to get the identifier
                            if (GetNextToken() != Constants.TOKEN_TYPE_IDENT)
                                ExitOnCodeError(Constants.ERROR_MSSG_IDENT_EXPECTED);

                            // Read the identifier, which is the current lexeme
                            string strIdent = new string(g_Lexer.strCurrLexeme.ToCharArray());

                            // Calculate the parameter's stack index
                            int iStackIndex = -(functable.g_FuncTable[CurrFunc].iLocalDataSize + 2 + (iCurrFuncParamCount + 1));

                            // Add the parameter to the symbol table
                            if (symtable.AddSymbol(strIdent, 1, iStackIndex, iCurrFuncIndex) == 9999)
                                ExitOnCodeError(Constants.ERROR_MSSG_IDENT_REDEFINITION);

                            // Increment the current parameter count
                            ++iCurrFuncParamCount;

                            break;
                        }

                    // Instructions
                    case Constants.TOKEN_TYPE_INSTR:
                        {
                            // Get the instruction's info using the current lexeme (the mnemonic )
                            instrtable.GetInstrByMnemonic(g_Lexer.strCurrLexeme, out CurrInstr);

                            // Write the opcode to the stream
                            g_pInstrStream[g_iCurrInstrIndex].iOpcode = CurrInstr.iOpcode;

                            // Write the operand count to the stream
                            g_pInstrStream[g_iCurrInstrIndex].iOpCount = CurrInstr.iOpCount;

                            // Allocate space to hold the operand list
                            List<Operand> CurrOpList = new List<Operand>();

                            // Loop through each operand, read it from the source and assemble it
                            for (int iCurrOpIndex = 0; iCurrOpIndex < CurrInstr.iOpCount; ++iCurrOpIndex)
                            {
                                // Read the operands' type bitfield
                                int CurrOpTypes = CurrInstr.OpList[iCurrOpIndex];

                                // Read in the next token, which is the initial token of the operand
                                int InitOpToken = GetNextToken();
                                switch (InitOpToken)
                                {
                                    // An integer literal
                                    case Constants.TOKEN_TYPE_INT:

                                        // Make sure the operand type is valid
                                        if ((CurrOpTypes & Constants.OP_FLAG_TYPE_INT) != 0)
                                        {
                                            Operand temp = new Operand();

                                            // Set an integer operand type
                                            temp.operand.iType = Constants.OP_TYPE_INT;

                                            // Copy the value into the operand list from the current
                                            // lexeme
                                            temp.operand.iIntLiteral = Convert.ToInt32(g_Lexer.strCurrLexeme);
                                            CurrOpList.Add(temp);
                                        }
                                        else
                                            ExitOnCodeError(Constants.ERROR_MSSG_INVALID_OP);

                                        break;

                                    // A floating-point literal
                                    case Constants.TOKEN_TYPE_FLOAT:

                                        // Make sure the operand type is valid
                                        if ((CurrOpTypes & Constants.OP_FLAG_TYPE_FLOAT) != 0)
                                        {
                                            Operand temp = new Operand();
                                            // Set a floating-point operand type
                                            temp.operand.iType = Constants.OP_TYPE_FLOAT;

                                            // Copy the value into the operand list from the current                                        
                                            // lexeme
                                            temp.operand.fFloatLiteral = Convert.ToSingle(g_Lexer.strCurrLexeme);
                                            CurrOpList.Add(temp);
                                        }
                                        else
                                            ExitOnCodeError(Constants.ERROR_MSSG_INVALID_OP);

                                        break;

                                    // A string literal (since strings always start with quotes)
                                    case Constants.TOKEN_TYPE_QUOTE:
                                        {
                                            // Make sure the operand type is valid
                                            if ((CurrOpTypes & Constants.OP_FLAG_TYPE_STRING) != 0)
                                            {
                                                Operand temp = new Operand();
                                                GetNextToken();

                                                // Handle the string based on its type
                                                switch (g_Lexer.CurrToken)
                                                {
                                                    // If we read another quote, the string is empty
                                                    case Constants.TOKEN_TYPE_QUOTE:
                                                        {
                                                            // Convert empty strings to the integer value zero
                                                            temp.operand.iType = Constants.OP_TYPE_INT;
                                                            temp.operand.iIntLiteral = 0;
                                                            CurrOpList.Add(temp);
                                                            break;
                                                        }

                                                    // It's a normal string
                                                    case Constants.TOKEN_TYPE_STRING:
                                                        {
                                                            // Get the string literal
                                                            string strString = new string(g_Lexer.strCurrLexeme.ToCharArray());

                                                            // Add the string to the table, or get the index of
                                                            // the existing copy
                                                            int iStringIndex = strtable.AddString(strString);

                                                            // Make sure the closing double-quote is present
                                                            if (GetNextToken() != Constants.TOKEN_TYPE_QUOTE)
                                                                ExitOnCharExpectedError('\\');

                                                            // Set the operand type to string index and set its
                                                            // data field
                                                            temp.operand.iType = Constants.OP_TYPE_STRING_INDEX;
                                                            temp.operand.iStringTableIndex = iStringIndex;
                                                            CurrOpList.Add(temp);
                                                            break;
                                                        }

                                                    // The string is invalid
                                                    default:
                                                        ExitOnCodeError(Constants.ERROR_MSSG_INVALID_STRING);
                                                        break;
                                                }
                                            }
                                            else
                                                ExitOnCodeError(Constants.ERROR_MSSG_INVALID_OP);

                                            break;
                                        }

                                    // _RetVal
                                    case Constants.TOKEN_TYPE_REG_RETVAL:

                                        // Make sure the operand type is valid
                                        if ((CurrOpTypes & Constants.OP_FLAG_TYPE_REG) != 0)
                                        {
                                            Operand temp = new Operand();
                                            // Set a register type
                                            temp.operand.iType = Constants.OP_TYPE_REG;
                                            temp.operand.iReg = 0;
                                            CurrOpList.Add(temp);
                                        }
                                        else
                                            ExitOnCodeError(Constants.ERROR_MSSG_INVALID_OP);

                                        break;

                                    // _T0
                                    case Constants.TOKEN_TYPE_REG_T0:

                                        // Make sure the operand type is valid
                                        if ((CurrOpTypes & Constants.OP_FLAG_TYPE_REG) != 0)
                                        {
                                            Operand temp = new Operand();
                                            // Set a register type
                                            temp.operand.iType = Constants.OP_TYPE_REG;
                                            temp.operand.iReg = 1;
                                            CurrOpList.Add(temp);
                                        }
                                        else
                                            ExitOnCodeError(Constants.ERROR_MSSG_INVALID_OP);

                                        break;

                                    // _T1
                                    case Constants.TOKEN_TYPE_REG_T1:

                                        // Make sure the operand type is valid
                                        if ((CurrOpTypes & Constants.OP_FLAG_TYPE_REG) != 0)
                                        {
                                            Operand temp = new Operand();
                                            // Set a register type
                                            temp.operand.iType = Constants.OP_TYPE_REG;
                                            temp.operand.iReg = 2;
                                            CurrOpList.Add(temp);
                                        }
                                        else
                                            ExitOnCodeError(Constants.ERROR_MSSG_INVALID_OP);

                                        break;

                                    // _T2
                                    case Constants.TOKEN_TYPE_REG_T2:

                                        // Make sure the operand type is valid
                                        if ((CurrOpTypes & Constants.OP_FLAG_TYPE_REG) != 0)
                                        {
                                            Operand temp = new Operand();
                                            // Set a register type
                                            temp.operand.iType = Constants.OP_TYPE_REG;
                                            temp.operand.iReg = 3;
                                            CurrOpList.Add(temp);
                                        }
                                        else
                                            ExitOnCodeError(Constants.ERROR_MSSG_INVALID_OP);

                                        break;

                                    // Identifiers
                                    // These operands can be any of the following
                                    //      - Variables/Array Indices
                                    //      - Line Labels
                                    //      - Function Names
                                    //      - Host API Calls
                                    case Constants.TOKEN_TYPE_IDENT:
                                        {
                                            // Find out which type of identifier is expected. Since no
                                            // instruction in XVM assebly accepts more than one type
                                            // of identifier per operand, we can use the operand types
                                            // alone to determine which type of identifier we're
                                            // parsing.
                                            // Parse a memory reference-- a variable or array index
                                            if ((CurrOpTypes & Constants.OP_FLAG_TYPE_MEM_REF) != 0)
                                            {
                                                Operand temp = new Operand();
                                                // Whether the memory reference is a variable or array
                                                // index, the current lexeme is the identifier so save a
                                                // copy of it for later
                                                string strIdent = new string(g_Lexer.strCurrLexeme.ToCharArray());

                                                // Make sure the variable/array has been defined
                                                if (symtable.GetSymbolByIdent(strIdent, iCurrFuncIndex) == -1)
                                                    ExitOnCodeError(Constants.ERROR_MSSG_UNDEFINED_IDENT);

                                                // Get the identifier's index as well; it may either be
                                                // an absolute index or a base index
                                                int iBaseIndex = symtable.GetStackIndexByIdent(strIdent, iCurrFuncIndex);

                                                // Use the lookahead character to find out whether or not
                                                // we're parsing an array
                                                if (GetLookAheadChar() != '[')
                                                {
                                                    // It's just a single identifier so the base index we
                                                    // already saved is the variable's stack index
                                                    // Make sure the variable isn't an array
                                                    if (symtable.GetSizeByIdent(strIdent, iCurrFuncIndex) > 1)
                                                        ExitOnCodeError(Constants.ERROR_MSSG_INVALID_ARRAY_NOT_INDEXED);

                                                    // Set the operand type to stack index and set the data
                                                    // field
                                                    temp.operand.iType = Constants.OP_TYPE_ABS_STACK_INDEX;
                                                    temp.operand.iStackIndex = iBaseIndex;
                                                    CurrOpList.Add(temp);
                                                }
                                                else
                                                {
                                                    // It's an array, so lets verify that the identifier is
                                                    // an actual array
                                                    if (symtable.GetSizeByIdent(strIdent, iCurrFuncIndex) == 1)
                                                        ExitOnCodeError(Constants.ERROR_MSSG_INVALID_ARRAY);

                                                    // First make sure the open brace is valid
                                                    if (GetNextToken() != Constants.TOKEN_TYPE_OPEN_BRACKET)
                                                        ExitOnCharExpectedError('[');

                                                    // The next token is the index, be it an integer literal
                                                    // or variable identifier
                                                    // or the register _AI
                                                    int IndexToken = GetNextToken();

                                                    if (IndexToken == Constants.TOKEN_TYPE_INT)
                                                    {
                                                        // It's an integer, so determine its value by
                                                        // converting the current lexeme to an integer
                                                        int iOffsetIndex = Convert.ToInt32(g_Lexer.strCurrLexeme);

                                                        // Add the index to the base index to find the offset
                                                        // index and set the operand type to absolute stack
                                                        // index
                                                        temp.operand.iType = Constants.OP_TYPE_ABS_STACK_INDEX;
                                                        temp.operand.iStackIndex = iBaseIndex + iOffsetIndex;
                                                        CurrOpList.Add(temp);
                                                    }
                                                    else if (IndexToken == Constants.TOKEN_TYPE_IDENT)
                                                    {
                                                        // It's an identifier, so save the current lexeme
                                                        string strIndexIdent = new string(g_Lexer.strCurrLexeme.ToCharArray());

                                                        // Make sure the index is a valid array index, in
                                                        // that the identifier represents a single variable
                                                        // as opposed to another array
                                                        if (symtable.GetSymbolByIdent(strIndexIdent, iCurrFuncIndex) == 9999)
                                                        {
                                                            foreach (var item in symtable.g_SymbolTable)
                                                            {
                                                                Console.WriteLine(item.ToString());
                                                            }
                                                            ExitOnCodeError(Constants.ERROR_MSSG_UNDEFINED_IDENT);
                                                        }
                                                        if (symtable.GetSizeByIdent(strIndexIdent, iCurrFuncIndex) > 1)
                                                            ExitOnCodeError(Constants.ERROR_MSSG_INVALID_ARRAY_INDEX);

                                                        // Get the variable's stack index and set the operand
                                                        // type to relative stack index
                                                        int iOffsetIndex = symtable.GetStackIndexByIdent(strIndexIdent, iCurrFuncIndex);

                                                        temp.operand.iType = Constants.OP_TYPE_REL_STACK_INDEX;
                                                        temp.operand.iStackIndex = iBaseIndex;
                                                        temp.operand.iOffsetIndex = iOffsetIndex;
                                                        CurrOpList.Add(temp);
                                                    }
                                                    else
                                                    {
                                                        // Whatever it is, it's invalid
                                                        ExitOnCodeError(Constants.ERROR_MSSG_INVALID_ARRAY_INDEX);
                                                    }

                                                    // Lastly, make sure the closing brace is present as well
                                                    if (GetNextToken() != Constants.TOKEN_TYPE_CLOSE_BRACKET)
                                                        ExitOnCharExpectedError('[');
                                                }
                                            }

                                            // Parse a line label
                                            if ((CurrOpTypes & Constants.OP_FLAG_TYPE_LINE_LABEL) != 0)
                                            {
                                                Operand temp = new Operand();
                                                // Get the current lexeme, which is the line label
                                                string strLabelIdent = new string(g_Lexer.strCurrLexeme.ToCharArray());

                                                // Use the label identifier to get the label's information
                                                int Label = labeltable.GetLabelByIdent(strLabelIdent, iCurrFuncIndex);

                                                // Make sure the label exists
                                                if (Label == -1)
                                                    ExitOnCodeError(Constants.ERROR_MSSG_UNDEFINED_LINE_LABEL);

                                                // Set the operand type to instruction index and set the
                                                // data field
                                                temp.operand.iType = Constants.OP_TYPE_INSTR_INDEX;
                                                temp.operand.iInstrIndex = labeltable.g_LabelTable[Label].iTargetIndex;
                                                CurrOpList.Add(temp);
                                            }

                                            // Parse a function name
                                            if ((CurrOpTypes & Constants.OP_FLAG_TYPE_FUNC_NAME) != 0)
                                            {
                                                Operand temp = new Operand();
                                                // Get the current lexeme, which is the function name
                                                string strFuncName = new string(g_Lexer.strCurrLexeme.ToCharArray());

                                                // Use the function name to get the function's information
                                                int Func = functable.GetFuncByName(strFuncName);

                                                // Make sure the function exists
                                                if (Func == -1)
                                                    ExitOnCodeError(Constants.ERROR_MSSG_UNDEFINED_FUNC);

                                                // Set the operand type to function index and set its data
                                                // field
                                                temp.operand.iType = Constants.OP_TYPE_FUNC_INDEX;
                                                temp.operand.iFuncIndex = functable.g_FuncTable[Func].iIndex;
                                                CurrOpList.Add(temp);
                                            }

                                            // Parse a host API call
                                            if ((CurrOpTypes & Constants.OP_FLAG_TYPE_HOST_API_CALL) != 0)
                                            {
                                                Operand temp = new Operand();
                                                // Get the current lexeme, which is the host API call
                                                string strHostAPICall = new string(g_Lexer.strCurrLexeme.ToCharArray());

                                                // Add the call to the table, or get the index of the
                                                // existing copy
                                                int iIndex = hostapitable.AddString(strHostAPICall);

                                                // Set the operand type to host API call index and set its
                                                // data field
                                                temp.operand.iType = Constants.OP_TYPE_HOST_API_CALL_INDEX;
                                                temp.operand.iHostAPICallIndex = iIndex;
                                                CurrOpList.Add(temp);
                                            }

                                            break;
                                        }

                                    // Anything else
                                    default:

                                        ExitOnCodeError(Constants.ERROR_MSSG_INVALID_OP);
                                        break;
                                }

                                // Make sure a comma follows the operand, unless it's the last one
                                if (iCurrOpIndex < CurrInstr.iOpCount - 1)
                                    if (GetNextToken() != Constants.TOKEN_TYPE_COMMA)
                                        ExitOnCharExpectedError(',');
                            }

                            // Make sure there's no extranous stuff ahead
                            if (GetNextToken() != Constants.TOKEN_TYPE_NEWLINE)
                                ExitOnCodeError(Constants.ERROR_MSSG_INVALID_INPUT);

                            // Copy the operand list pointer into the assembled stream
                            g_pInstrStream[g_iCurrInstrIndex].CloneOplist(CurrOpList);

                            // Move along to the next instruction in the stream
                            ++g_iCurrInstrIndex;

                            break;
                        }
                }

                // Skip to the next line
                if (!SkipToNextLine())
                    break;
            }
            Console.WriteLine("assembled");
        }

        void BuildXSE(string filename)
        {
            StringBuilder strBuilder = new StringBuilder();
            StringWriter sw = new StringWriter(strBuilder);
            JsonWriter jw = new JsonTextWriter(sw);
            jw.Formatting = Formatting.Indented;


            //strBuilder.Append("Header");
            //strBuilder.Append('\n');
            // ---- Write the header
            // Write the ID string (4 bytes)
            jw.WriteStartObject();
            jw.WritePropertyName("ID");
            jw.WriteValue(Constants.XSE_ID_STRING);

            // Write the version (1 byte for each component, 2 total)
            jw.WritePropertyName("VMa");                    //version major
            jw.WriteValue((byte)Constants.VERSION_MAJOR);
            jw.WritePropertyName("VMi");                    //version minor
            jw.WriteValue((byte)Constants.VERSION_MINOR);

            // Write the stack size (4 bytes)
            jw.WritePropertyName("SS");                     //stack size
            jw.WriteValue(g_ScriptHeader.iStackSize);

            // Write the global data size (4 bytes )
            jw.WritePropertyName("GDS");                    //global data size
            jw.WriteValue(g_ScriptHeader.iGlobalDataSize);

            // Write the _Main () flag (1 byte)
            char cIsMainPresent = (char)0;
            if (g_ScriptHeader.iIsMainFuncPresent)
                cIsMainPresent = (char)1;
            jw.WritePropertyName("IMP");                    //is main present
            jw.WriteValue((byte)cIsMainPresent);

            // Write the _Main () function index (4 bytes)
            jw.WritePropertyName("MFI");                    //main function index
            jw.WriteValue(g_ScriptHeader.iMainFuncIndex);

            // Write the priority type (1 byte)
            char cPriorityType = (char)g_ScriptHeader.iPriorityType;
            jw.WritePropertyName("PT");                     //priority type
            jw.WriteValue((byte)cPriorityType);

            // Write the user-defined priority (4 bytes)
            jw.WritePropertyName("UP");                     //user priority
            jw.WriteValue(g_ScriptHeader.iUserPriority);
            jw.WriteEndObject();
            strBuilder.Append('|');

            // ---- Write the instruction stream
            jw.WriteStartObject();
            // Output the instruction count (4 bytes)
            jw.WritePropertyName("ISS");                    //instruction stream size
            jw.WriteValue(g_iInstrStreamSize);
            jw.WriteEndObject();
            strBuilder.Append('|');

            // Loop through each instruction and write its data out

            for (int iCurrInstrIndex = 0; iCurrInstrIndex < g_iInstrStreamSize; ++iCurrInstrIndex)
            {
                jw.WriteStartObject();
                // Write the opcode (2 bytes)
                short sOpcode = (short)g_pInstrStream[iCurrInstrIndex].iOpcode;
                jw.WritePropertyName("Opc");                 //Opcode
                jw.WriteValue(sOpcode);

                // Write the operand count (1 byte)
                char iOpCount = (char)g_pInstrStream[iCurrInstrIndex].iOpCount;
                jw.WritePropertyName("OC");                 //OpCount
                jw.WriteValue((byte)iOpCount);
                jw.WriteEndObject();

                strBuilder.Append('|');

                if (iOpCount != 0)
                {
                    // Loop through the operand list and print each one out

                    for (int iCurrOpIndex = 0; iCurrOpIndex < iOpCount; ++iCurrOpIndex)
                    {
                        jw.WriteStartObject();
                        // Make a copy of the operand pointer for convinience
                        Operand CurrOp = g_pInstrStream[iCurrInstrIndex].OpList[iCurrOpIndex];

                        // Create a character for holding operand types (1 byte)
                        char cOpType = (char)CurrOp.operand.iType;
                        jw.WritePropertyName("OT");         //OpType
                        jw.WriteValue((byte)cOpType);

                        StringBuilder temp = new StringBuilder();
                        StringBuilder Optype = new StringBuilder();
                        // Write the operand depending on its type
                        switch (CurrOp.operand.iType)
                        {
                            // Integer literal
                            case Constants.OP_TYPE_INT:
                                temp.Append(CurrOp.operand.iIntLiteral);
                                Optype.Append(Constants.OP_TYPE_INT_BUILT);
                                break;

                            // Floating-point literal
                            case Constants.OP_TYPE_FLOAT:
                                temp.Append(CurrOp.operand.fFloatLiteral);
                                Optype.Append(Constants.OP_TYPE_FLOAT_BUILT);
                                break;

                            // String index
                            case Constants.OP_TYPE_STRING_INDEX:
                                temp.Append(CurrOp.operand.iStringTableIndex);
                                Optype.Append(Constants.OP_TYPE_STRING_INDEX_BUILT);
                                break;

                            // Instruction index
                            case Constants.OP_TYPE_INSTR_INDEX:
                                temp.Append(CurrOp.operand.iInstrIndex);
                                Optype.Append(Constants.OP_TYPE_INSTR_INDEX_BUILT);
                                break;

                            // Absolute stack index
                            case Constants.OP_TYPE_ABS_STACK_INDEX:
                                temp.Append(CurrOp.operand.iStackIndex);
                                Console.WriteLine(CurrOp.operand.iStackIndex);
                                Optype.Append(Constants.OP_TYPE_STACK_INDEX_BUILT);
                                break;

                            // Relative stack index
                            case Constants.OP_TYPE_REL_STACK_INDEX:
                                temp.Append(CurrOp.operand.iStackIndex);
                                jw.WritePropertyName("OI");//offset index
                                jw.WriteValue(CurrOp.operand.iOffsetIndex);
                                Optype.Append(Constants.OP_TYPE_STACK_INDEX_BUILT);
                                break;

                            // Function index
                            case Constants.OP_TYPE_FUNC_INDEX:
                                temp.Append(CurrOp.operand.iFuncIndex);
                                Optype.Append(Constants.OP_TYPE_FUNC_INDEX_BUILT);
                                break;

                            // Host API call index
                            case Constants.OP_TYPE_HOST_API_CALL_INDEX:
                                temp.Append(CurrOp.operand.iHostAPICallIndex);
                                Optype.Append(Constants.OP_TYPE_HOST_API_CALL_INDEX_BUILT);
                                break;

                            // Register
                            case Constants.OP_TYPE_REG:
                                temp.Append(CurrOp.operand.iReg);
                                Optype.Append(Constants.OP_TYPE_REG_BUILT);
                                break;
                            default:
                                temp.Append('n');
                                break;
                        }
                        jw.WritePropertyName(Optype.ToString());     //Operand
                        if (CurrOp.operand.iType == Constants.OP_TYPE_FLOAT)
                        {
                            jw.WriteValue(Convert.ToSingle(temp.ToString()));
                        }
                        else
                        {
                            jw.WriteValue(Convert.ToInt32(temp.ToString()));
                        }
                        jw.WriteEndObject();
                        strBuilder.Append('|');
                    }

                }
            }
            
            //if (strtable.g_StringTable.Count > 0)
            //{
                strBuilder.Append('|');
                // ---- Write the string table
                jw.WriteStartObject();
                // Write out the string count (4 bytes)
                jw.WritePropertyName("SC");             //string count
                jw.WriteValue(strtable.g_StringTable.Count);
                jw.WriteEndObject();
                strBuilder.Append('|');

                strBuilder.Append(JsonConvert.SerializeObject(strtable.g_StringTable, Formatting.Indented));
                strBuilder.Append('|');
            //}

            //---- Write the function table
            //if (functable.g_FuncTable.Count > 0)
            //{
                // Create a character for writing parameter counts
                char cParamCount;

                jw.WriteStartObject();
                // Write out the function count (4 bytes)
                jw.WritePropertyName("FC");         //Function count
                jw.WriteValue(functable.g_FuncTable.Count);
                jw.WriteEndObject();
                strBuilder.Append('|');

                // Loop through each node in the list and write out its function info
                jw.WriteStartArray();
                foreach (var Func in functable.g_FuncTable)
                {
                    jw.WriteStartObject();
                    // Create a local copy of the function

                    // Write the entry point (4 bytes)
                    jw.WritePropertyName("EP");         //entry point
                    jw.WriteValue(Func.iEntryPoint);

                    // Write the parameter count (1 byte)
                    cParamCount = (char)Func.iParamCount;
                    jw.WritePropertyName("PC");         //param count
                    jw.WriteValue((byte)cParamCount);

                    // Write the local data size (4 bytes)
                    jw.WritePropertyName("LDS");        //local data size
                    jw.WriteValue(Func.iLocalDataSize);

                    // Write the function name length (1 byte)
                    char cFuncNameLength = (char)Func.strName.Length;
                    jw.WritePropertyName("FNL");        //function name length
                    jw.WriteValue((byte)cFuncNameLength);

                    // Write the function name (N bytes)
                    jw.WritePropertyName("FN");         //function name
                    jw.WriteValue(Func.strName);
                    jw.WriteEndObject();
                }
                jw.WriteEndArray();
                strBuilder.Append('|');
            //}

            // ---- Write the host API call table
            //if (hostapitable.g_HostAPICallTable.Count>0)
            //{
                jw.WriteStartObject();
                // Write out the call count (4 bytes)
                jw.WritePropertyName("HAC");            //host API call count
                jw.WriteValue(hostapitable.g_HostAPICallTable.Count);
                jw.WriteEndObject();
                strBuilder.Append('|');

                // Loop through each node in the list and write out its string
                strBuilder.Append(JsonConvert.SerializeObject(hostapitable.g_HostAPICallTable, Formatting.Indented));
            //}

            // Compress compiled source code
            //string compressed = new string(CompressionHelper.CompressString(strBuilder.ToString()).ToCharArray());
            string compressed = strBuilder.ToString();

            //Dump the whole string into compiled file
            string path;
            if (filename == null)
            {
                path = ConstructFilePath(scriptPath, GetFileName(scriptPath), ".xse");

            }
            else
            {
                path = ConstructFilePath(scriptPath, filename, ".xse");
            }

            Console.WriteLine(path);
            StreamWriter strw = new StreamWriter(path, false);
            strw.Write(compressed);
            strw.Close();

            path = ConstructFilePath(scriptPath, "symtable", ".txt");
            Console.WriteLine(path);
            strw = new StreamWriter(path, false);
            strw.Write(symtable.ToString());
            strw.Close();

            Console.WriteLine("file saved");
        }

        string ConstructFilePath(string path,string filename,string extension)
        {
            StringBuilder strbuilder = new StringBuilder();
            string[] split = path.Split('\\');

            for(int i=0;i<split.GetLength(0)-1;i++)
            {
                strbuilder.Append(split[i]);
                strbuilder.Append('\\');
            }
            strbuilder.Append(filename);
            strbuilder.Append(extension);

            return strbuilder.ToString();
        }

        string GetFileName(string path)
        {
            string[] split = path.Split('\\');
            return split[split.GetLength(0) - 1].Split('.')[0];
        }

        void Shutdown()
        {
            //g_Lexer = new Lexer();
            //instrtable = new XASM_InstructionTable();
            //instrtable.InitInstrTable();
            //functable = new XASM_FunctionTable();
            //hostapitable = new XASM_HostAPITable();
            //labeltable = new XASM_LabelTable();
            //strtable = new XASM_StringTable();
            //symtable = new XASM_SymbolTable();
            //g_ScriptHeader = new ScriptHeader();
        }

        void Exit()
        {
            Shutdown();
        }

        void ExitOnError(string strErrorMssg)
        {
            // Print the message
            Console.WriteLine("Fatal Error: " + strErrorMssg);

            // Exit the program
            Exit();
        }

        void ExitOnCodeError(string strErrorMssg)
        {
            // Print the message
            Console.WriteLine("Error: " + strErrorMssg);
            Console.WriteLine("Line " + g_Lexer.iCurrSourceLine);

            // Reduce all of the source line's spaces to tabs so it takes less space and so the
            // karet lines up with the current token properly
            string strSourceLine = new string(fileLines[g_Lexer.iCurrSourceLine].ToCharArray());

            // Loop through each character and replace tabs with spaces
            strSourceLine = strSourceLine.Replace('\t', ' ');

            // Print the offending source line
            Console.WriteLine(strSourceLine);

            // Print a karet at the start of the (presumably) offending lexeme
            var builder = new StringBuilder();
            for (int iCurrSpace = 0; iCurrSpace < g_Lexer.iIndex0; ++iCurrSpace)
                builder.Append("-");
            builder.Append("^\n");
            Console.WriteLine(builder.ToString());

            // Print message indicating that the script could not be assembled
            Console.WriteLine("Could not assemble the script");

            // Exit the program
            Exit();
        }

        void ExitOnCharExpectedError(char cChar)
        {
            // Create an error message based on the character
            var builder = new StringBuilder();
            builder.Append("'");
            builder.Append(cChar);
            builder.Append("' expected");

            // Exit on the code error
            ExitOnCodeError(builder.ToString());
        }
    }
}