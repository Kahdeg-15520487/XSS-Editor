using System;
using System.Text;
using XASM;

namespace XASM_Agent
{
    class Program
    {
        static int Main(string[] args)
        {
            int returncode = 1;
            //check space and tab in the arg
            foreach (var arg in args)
            {
                if (arg.Contains(" "))
                {
                    Console.WriteLine("Error:There is space in source code file path.");
                    Console.WriteLine("...");
                    Console.ReadLine();
                    returncode = 0;
                }
            }
            if (returncode == 1)
            {
                XASM_Compiler compiler;
                if (args.Length == 1)
                {
                    Console.Write("lala ");
                    Console.WriteLine(GetFilePathWithoutName(args[0]));
                    if (GetExtension(args[0]).Equals("xasm", StringComparison.OrdinalIgnoreCase))
                    {
                        compiler = new XASM_Compiler(args[0], true);
                        switch (compiler.errorcode)
                        {
                            case Constants.ErrorCode.None:
                                break;
                            case Constants.ErrorCode.OK:
                                Console.Write("Compiling:");
                                Console.WriteLine(GetFileName(args[0]));
                                compiler.Compile();
                                break;
                            case Constants.ErrorCode.Read:
                                Console.WriteLine("Can't read xasm source file");
                                returncode = 0;
                                break;
                            case Constants.ErrorCode.Assemble:
                                break;
                            default:
                                break;
                        }
                    }
                    else
                    {
                        Console.WriteLine("This is not a xasm sourcecode file");
                        Console.ReadLine();
                        returncode = 0;
                    }
                }

                if (args.Length == 2)
                {
                    if (GetExtension(args[0]).Equals("xasm"))
                    {
                        compiler = new XASM_Compiler(args[0], true);
                        switch (compiler.errorcode)
                        {
                            case Constants.ErrorCode.None:
                                break;
                            case Constants.ErrorCode.OK:
                                Console.Write("Compiling:");
                                Console.WriteLine(GetFileName(args[0]));
                                compiler.Compile(args[1]);  //save the assembled file as the name stated in args[1]
                                break;
                            case Constants.ErrorCode.Read:
                                Console.WriteLine("Can't read xasm source file");
                                returncode = 0;
                                break;
                            case Constants.ErrorCode.Assemble:
                                break;
                            default:
                                break;
                        }
                    }
                    else
                    {
                        Console.WriteLine("This is not a xasm sourcecode file");
                        Console.ReadLine();
                        returncode = 0;
                    }
                }
            }
            Console.ReadLine();
            return returncode;
        }

        static string GetFilePathWithoutName(string path)
        {
            string[] split = path.Split('\\');
            StringBuilder resultpath = new StringBuilder();
            for (int i = 0; i < split.Length - 2; i++)
                resultpath.Append(split[i]);
            //return resultpath.ToString();
            return split[0];
        }

        static string GetFileName(string path)
        {
            string[] split = path.Split('\\');
            return split[split.GetLength(0) - 1];
        }

        static string GetExtension(string path)
        {
            string[] split = path.Split('.');
            return split[split.GetLength(0) - 1];
        }
    }
}
