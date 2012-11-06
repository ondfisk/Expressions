using System;

namespace Expressions
{
    class MainProgram
    {
        static void Main(string[] args)
        {
            if (args.Length == 2)
            {
                var scanner = new Scanner(args[0]);
                var parser = new Parser(scanner);
                parser.Parse();
                switch (args[1])
                {
                    case "run":
                        Console.WriteLine(parser.program.Eval());
                        return;
                    case "check":
                        parser.program.Check();
                        return;
                    case "compile":
                        var gen = new Generator();
                        const string outputFile = "a.out";
                        parser.program.Compile(gen, outputFile);
                        return;
                }
            }
            Console.WriteLine("Usage: Program <expression.txt> [run|check|compile]");
        }
    }
}