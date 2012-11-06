
/* A unified-stack abstract machine for imperative programs
   sestoft@dina.kvl.dk * 2001-03-21, 2007-03-15

   Compile this file with 

      csc Program.cs

   To execute a program file using this abstract machine, do:

      Program <programfile> <arg1> <arg2> ...

   or

      Program -trace <programfile> <arg1> <arg2> ...
 
   The Read instruction will attempt to read from file a.in if it exists.
*/

using System;
using System.IO;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Machine
{
    class Program
    {
        static void Main(string[] args)
        {
            bool trace = args.Length > 0 && (args[0] == "-trace" || args[0] == "/trace");
            if (args.Length == 0 || trace && args.Length == 1)
                Console.WriteLine("Usage: Program [-trace] <programfile> <arg1> ...\n");
            else
                Execute(args, trace);
        }

        // These numeric instruction codes must agree with code Program.sml:

        public enum Opcode
        {
            Label = -1, // Unused
            CstI, Add, Sub, Mul, Div, Mod, Eq, LT, Not,
            Dup, Swap, LdI, StI, GetBp, GetSp, IncSp,
            Goto, IfZero, IfNZero, Call, TCall, Ret,
            PrintI, PrintC, Read, LdArgs,
            Stop
        }

        // Read code from file and execute it

        const int STACKSIZE = 1000;

        static void Execute(String[] args, bool trace)
        {
            int firstArg = trace ? 1 : 0;
            int[] p = Readfile(args[firstArg]);         // Read the program from file
            int[] s = new int[STACKSIZE];               // The evaluation stack
            IEnumerator<int> inputs = MakeInputReader("a.in").GetEnumerator();
            int[] iargs = new int[args.Length - firstArg - 1];
            for (int i = firstArg + 1; i < args.Length; i++)  // Commandline arguments
                iargs[i - 1] = int.Parse(args[i]);
            DateTime starttime = DateTime.Now;
            ExecCode(p, s, iargs, inputs, trace);       // Execute program
            double dur = (DateTime.Now - starttime).TotalSeconds;
            Console.Error.WriteLine("\nRan " + dur + " seconds");
        }

        // Read a stream of integers from a text file

        public static IEnumerable<int> MakeInputReader(String filename)
        {
            FileInfo fi = new FileInfo(filename);
            if (fi.Exists)
            {
                Regex regex = new Regex("[ \\t]+");
                using (TextReader rd = fi.OpenText())
                {
                    for (String line = rd.ReadLine(); line != null; line = rd.ReadLine())
                        foreach (String s in regex.Split(line))
                            if (s != "")
                                yield return int.Parse(s);
                }
            }
            else
                yield break;
        }

        // The machine: execute the code starting at p[pc] 

        static int ExecCode(int[] p, int[] s, int[] iargs,
                            IEnumerator<int> inputs, bool trace)
        {
            int bp = -999;  // Base pointer, for accessing local
            int sp = -1;    // Stack top pointer: current top of stack
            int pc = 0;     // Program counter: next instruction
            for (; ; )
            {
                if (trace)
                    PrintSpPc(s, bp, sp, p, pc);
                switch ((Opcode)p[pc++])
                {
                    case Opcode.CstI:
                        s[sp + 1] = p[pc++]; sp++; break;
                    case Opcode.Add:
                        s[sp - 1] = s[sp - 1] + s[sp]; sp--; break;
                    case Opcode.Sub:
                        s[sp - 1] = s[sp - 1] - s[sp]; sp--; break;
                    case Opcode.Mul:
                        s[sp - 1] = s[sp - 1] * s[sp]; sp--; break;
                    case Opcode.Div:
                        s[sp - 1] = s[sp - 1] / s[sp]; sp--; break;
                    case Opcode.Mod:
                        s[sp - 1] = s[sp - 1] % s[sp]; sp--; break;
                    case Opcode.Eq:
                        s[sp - 1] = (s[sp - 1] == s[sp] ? 1 : 0); sp--; break;
                    case Opcode.LT:
                        s[sp - 1] = (s[sp - 1] < s[sp] ? 1 : 0); sp--; break;
                    case Opcode.Not:
                        s[sp] = (s[sp] == 0 ? 1 : 0); break;
                    case Opcode.Dup:
                        s[sp + 1] = s[sp]; sp++; break;
                    case Opcode.Swap:
                        { int tmp = s[sp]; s[sp] = s[sp - 1]; s[sp - 1] = tmp; } break;
                    case Opcode.LdI:                 // load indirect
                        s[sp] = s[s[sp]]; break;
                    case Opcode.StI:                 // store indirect, keep value on top
                        s[s[sp - 1]] = s[sp]; s[sp - 1] = s[sp]; sp--; break;
                    case Opcode.GetBp:
                        s[sp + 1] = bp; sp++; break;
                    case Opcode.GetSp:
                        s[sp + 1] = sp; sp++; break;
                    case Opcode.IncSp:
                        sp = sp + p[pc++]; break;
                    case Opcode.Goto:
                        pc = p[pc]; break;
                    case Opcode.IfZero:
                        pc = (s[sp--] == 0 ? p[pc] : pc + 1); break;
                    case Opcode.IfNZero:
                        pc = (s[sp--] != 0 ? p[pc] : pc + 1); break;
                    case Opcode.Call:
                        {
                            int argc = p[pc++];
                            for (int i = 0; i < argc; i++)	   // Make room for return address
                                s[sp - i + 2] = s[sp - i];		   // and old base pointer
                            s[sp - argc + 1] = pc + 1; sp++;
                            s[sp - argc + 1] = bp; sp++;
                            bp = sp + 1 - argc;
                            pc = p[pc];
                        } break;
                    case Opcode.TCall:
                        {
                            int argc = p[pc++];                // Number of new arguments
                            int pop = p[pc++];		   // Number of variables to discard
                            for (int i = argc - 1; i >= 0; i--)	   // Discard variables
                                s[sp - i - pop] = s[sp - i];
                            sp = sp - pop; pc = p[pc];
                        } break;
                    case Opcode.Ret:
                        {
                            int res = s[sp];
                            sp = sp - p[pc]; bp = s[--sp]; pc = s[--sp];
                            s[sp] = res;
                        } break;
                    case Opcode.PrintI:
                        Console.Write(s[sp] + " "); break;
                    case Opcode.PrintC:
                        Console.Write((char)(s[sp])); break;
                    case Opcode.LdArgs:
                        foreach (int x in iargs)
                            s[++sp] = x;
                        break;
                    case Opcode.Read:
                        if (inputs.MoveNext())
                            s[++sp] = inputs.Current;
                        else
                            throw new Exception("No more input");
                        break;
                    case Opcode.Stop:
                        return sp;
                    default:
                        throw new Exception("Illegal instruction " + p[pc - 1]
                                            + " at address " + (pc - 1));
                }
            }
        }

        // Print the stack machine instruction at p[pc]

        static String InsName(int[] p, int pc)
        {
            switch ((Opcode)p[pc])
            {
                case Opcode.CstI: return "CST " + p[pc + 1];
                case Opcode.Add: return "ADD";
                case Opcode.Sub: return "SUB";
                case Opcode.Mul: return "MUL";
                case Opcode.Div: return "DIV";
                case Opcode.Mod: return "MOD";
                case Opcode.Eq: return "EQ";
                case Opcode.LT: return "LT";
                case Opcode.Not: return "NOT";
                case Opcode.Dup: return "DUP";
                case Opcode.Swap: return "SWAP";
                case Opcode.LdI: return "LDI";
                case Opcode.StI: return "StI";
                case Opcode.GetBp: return "GetBp";
                case Opcode.GetSp: return "GetSp";
                case Opcode.IncSp: return "IncSp " + p[pc + 1];
                case Opcode.Goto: return "GOTO " + p[pc + 1];
                case Opcode.IfZero: return "IfZero " + p[pc + 1];
                case Opcode.IfNZero: return "IfNZero " + p[pc + 1];
                case Opcode.Call: return "CALL " + p[pc + 1] + " " + p[pc + 2];
                case Opcode.TCall: return "TCall " + p[pc + 1] + " " + p[pc + 2] + " " + p[pc + 3];
                case Opcode.Ret: return "Ret " + p[pc + 1];
                case Opcode.PrintI: return "PrintI";
                case Opcode.PrintC: return "PrintC";
                case Opcode.LdArgs: return "LdArgs";
                case Opcode.Read: return "Read";
                case Opcode.Stop: return "Stop";
                default: return "<unknown>";
            }
        }

        // Print current stack and current instruction

        static void PrintSpPc(int[] s, int bp, int sp, int[] p, int pc)
        {
            Console.Write("[ ");
            for (int i = 0; i <= sp; i++)
                Console.Write(s[i] + " ");
            Console.Write("]");
            Console.WriteLine("{" + pc + ": " + InsName(p, pc) + "}");
        }

        // Read instructions from a file

        public static int[] Readfile(String filename)
        {
            List<int> rawprogram = new List<int>();
            rawprogram.AddRange(MakeInputReader(filename));
            return rawprogram.ToArray();
        }
    }
}
