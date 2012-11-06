// Abstract syntax, interpretation and compilation 
// for arithmetic and logical expressions
// sestoft@itu.dk 2007-03-12, 2010-02-08
// Extended with functions by Rasmus Mogelberg 2011-10-20

using System;
using System.Collections.Generic;
using System.IO;

namespace Expressions
{
    class MainProgram
    {
        static void Main(string[] args)
        {
            if (args.Length == 2)
            {
                Scanner scanner = new Scanner(args[0]);
                Parser parser = new Parser(scanner);
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
                        Generator gen = new Generator();
                        String outputFile = "a.out";
                        parser.program.Compile(gen, outputFile);
                        return;
                }
            }
            Console.WriteLine("Usage: Expressions <expression.txt> [run|check|compile]");
        }
    }

    // Programs abstract syntax. A program consists of a term to be evaluated and a function environment in which it is evaluated

    public class Program
    {
        private FEnv fenv;
        private Expression e;

        public Program(Dictionary<String, FuncDef> functions, Expression e)
        {
            fenv = new FEnv(functions);
            this.e = e;
        }

        public int Eval()
        {
            REnv renv = new REnv();
            return e.Eval(renv, fenv);
        }

        public Type Check()
        {
            TEnv tenv = new TEnv();
            fenv.Check(tenv, fenv);
            return e.Check(tenv, fenv);
        }

        public void Compile(Generator gen, String outputFile)
        {
            // Generate compiletime environment
            var labelMap = new Dictionary<String, String>();
            foreach (String funName in fenv.getFunctionNames())
                labelMap.Add(funName, Label.Fresh());
            CEnv cenv = new CEnv(labelMap);

            // Compile expression
            e.Compile(cenv, gen);
            gen.Emit(Instruction.PRINTI);
            gen.Emit(Instruction.STOP);

            // Compile functions
            foreach (FuncDef f in fenv.getFunctions())
            {
                cenv = new CEnv(labelMap);
                f.Compile(gen, cenv);
            }

            //  Generate bytecode at and print to file
            gen.PrintCode();
            int[] bytecode = gen.ToBytecode();
            using (TextWriter wr = new StreamWriter(outputFile))
            {
                foreach (int b in bytecode)
                {
                    wr.Write(b);
                    wr.Write(" ");
                }
            }
        }
    }

    // Abstract syntax for function definitions. 

    public class FuncDef
    {
        private readonly String fName;
        private readonly Pair<String, Type> formArg;
        private readonly Expression body;
        public readonly Type returnType;

        public FuncDef(Type returnType, String fName, Type argType, String argName, Expression body)
        {
            this.formArg = new Pair<string, Type>(argName, argType);
            this.body = body; this.returnType = returnType;
            this.fName = fName;
        }

        public void Check(TEnv env, FEnv fEnv)
        {
            env.DeclareLocal(formArg.Fst, formArg.Snd);
            Type t = body.Check(env, fEnv);
            env.PopEnv();
            if (t != returnType)
                throw new TypeException("Body of " + fName + " returns " + t + ", " + returnType + " expected");
        }

        public int Eval(REnv env, FEnv fenv, int arg)
        {
            env.AllocateLocal(formArg.Fst);
            env.GetVariable(formArg.Fst).value = arg;
            int v = body.Eval(env, fenv);
            env.PopEnv();
            return v;
        }

        public void Compile(Generator gen, CEnv env)
        {
            env.DeclareLocal(formArg.Fst); // Formal argument name points to top of stack
            gen.Label(env.getFunctionLabel(fName));
            body.Compile(env, gen);
            gen.Emit(new RET(1));
        }

        public bool CheckArgType(Type argType)
        {
            if (argType != formArg.Snd)
                return false;
            return true;
        }
    }

    // Expression abstract syntax

    public abstract class Expression
    {
        abstract public int Eval(REnv env, FEnv fEnv);
        abstract public Type Check(TEnv env, FEnv fEnv);
        abstract public void Compile(CEnv env, Generator gen);
    }

    public class Constant : Expression
    {
        private readonly int value;
        private readonly Type type;

        public Constant(int value, Type type)
        {
            this.value = value;
            this.type = type;
        }

        public override int Eval(REnv env, FEnv fEnv)
        {
            return value;
        }

        public override Type Check(TEnv env, FEnv fEnv)
        {
            return type;
        }

        public override void Compile(CEnv env, Generator gen)
        {
            gen.Emit(new CSTI(value));
        }
    }

    public class Variable : Expression
    {
        private readonly String name;

        public Variable(String name)
        {
            this.name = name;
        }

        public override int Eval(REnv env, FEnv fEnv)
        {
            return env.GetVariable(name).value;
        }

        public override Type Check(TEnv env, FEnv fEnv)
        {
            return env.GetVariable(name);
        }

        public override void Compile(CEnv env, Generator gen)
        {
            env.CompileVariable(gen, name);
            gen.Emit(Instruction.LDI);
        }
    }

    public enum Operator
    {
        Add, Sub, Mul, Div, Neg, Eq, Ne, Lt, Le, Gt, Ge, Not, And, Or, Bad
    }

    public class BinOp : Expression
    {
        private readonly Operator op;
        private readonly Expression e1, e2;

        public BinOp(Operator op, Expression e1, Expression e2)
        {
            this.op = op;
            this.e1 = e1;
            this.e2 = e2;
        }

        public override int Eval(REnv env, FEnv fEnv)
        {
            int v1 = e1.Eval(env, fEnv);
            int v2 = e2.Eval(env, fEnv);
            switch (op)
            {
                case Operator.Add:
                    return v1 + v2;
                case Operator.Div:
                    return v1 / v2;
                case Operator.Mul:
                    return v1 * v2;
                case Operator.Sub:
                    return v1 - v2;
                case Operator.Eq:
                    return v1 == v2 ? 1 : 0;
                case Operator.Ne:
                    return v1 != v2 ? 1 : 0;
                case Operator.Lt:
                    return v1 < v2 ? 1 : 0;
                case Operator.Le:
                    return v1 <= v2 ? 1 : 0;
                case Operator.Gt:
                    return v1 > v2 ? 1 : 0;
                case Operator.Ge:
                    return v1 >= v2 ? 1 : 0;
                case Operator.And:
                    return v1 == 0 ? 0 : v2;
                case Operator.Or:
                    return v1 == 0 ? v2 : 1;
                default:
                    throw new Exception("Unknown binary operator: " + op);
            }
        }

        public override Type Check(TEnv env, FEnv fEnv)
        {
            Type t1 = e1.Check(env, fEnv);
            Type t2 = e2.Check(env, fEnv);
            switch (op)
            {
                case Operator.Add:
                case Operator.Div:
                case Operator.Mul:
                case Operator.Sub:
                    if (t1 == Type.intType && t2 == Type.intType)
                        return Type.intType;
                    else
                        throw new TypeException("Arguments to + - * / must be int");
                case Operator.Eq:
                case Operator.Ge:
                case Operator.Gt:
                case Operator.Le:
                case Operator.Lt:
                case Operator.Ne:
                    if (t1 == Type.intType && t2 == Type.intType)
                        return Type.boolType;
                    else
                        throw new TypeException("Arguments to == >= > <= < != must be int");
                case Operator.Or:
                case Operator.And:
                    if (t1 == Type.boolType && t2 == Type.boolType)
                        return Type.boolType;
                    else
                        throw new TypeException("Arguments to & must be bool");
                default:
                    throw new Exception("Unknown binary operator: " + op);
            }
        }

        public override void Compile(CEnv env, Generator gen)
        {
            e1.Compile(env, gen);
            env.PushTemporary();
            e2.Compile(env, gen);
            switch (op)
            {
                case Operator.Add:
                    gen.Emit(Instruction.ADD);
                    break;
                case Operator.Div:
                    gen.Emit(Instruction.DIV);
                    break;
                case Operator.Mul:
                    gen.Emit(Instruction.MUL);
                    break;
                case Operator.Sub:
                    gen.Emit(Instruction.SUB);
                    break;
                case Operator.Eq:
                    gen.Emit(Instruction.EQ);
                    break;
                case Operator.Ne:
                    gen.Emit(Instruction.EQ);
                    gen.Emit(Instruction.NOT);
                    break;
                case Operator.Ge:
                    gen.Emit(Instruction.LT);
                    gen.Emit(Instruction.NOT);
                    break;
                case Operator.Gt:
                    gen.Emit(Instruction.SWAP);
                    gen.Emit(Instruction.LT);
                    break;
                case Operator.Le:
                    gen.Emit(Instruction.SWAP);
                    gen.Emit(Instruction.LT);
                    gen.Emit(Instruction.NOT);
                    break;
                case Operator.Lt:
                    gen.Emit(Instruction.LT);
                    break;
                case Operator.And:
                    gen.Emit(Instruction.MUL);
                    break;
                case Operator.Or:
                    gen.Emit(Instruction.ADD);
                    gen.Emit(new CSTI(0));
                    gen.Emit(Instruction.EQ);
                    gen.Emit(Instruction.NOT);
                    break;
                default:
                    throw new Exception("Unknown binary operator: " + op);
            }
            env.PopTemporary();
        }
    }

    public class UnOp : Expression
    {
        private readonly Operator op;
        private readonly Expression e1;

        public UnOp(Operator op, Expression e1)
        {
            this.op = op;
            this.e1 = e1;
        }

        public override int Eval(REnv env, FEnv fEnv)
        {
            int v1 = e1.Eval(env, fEnv);
            switch (op)
            {
                case Operator.Not:
                    return v1 == 0 ? 1 : 0;
                case Operator.Neg:
                    return -v1;
                default:
                    throw new Exception("Unknown unary operator: " + op);
            }
        }

        public override Type Check(TEnv env, FEnv fEnv)
        {
            Type t1 = e1.Check(env, fEnv);
            switch (op)
            {
                case Operator.Neg:
                    if (t1 == Type.intType)
                        return Type.intType;
                    else
                        throw new TypeException("Argument to - must be int");
                case Operator.Not:
                    if (t1 == Type.boolType)
                        return Type.boolType;
                    else
                        throw new TypeException("Argument to ! must be bool");
                default:
                    throw new Exception("Unknown unary operator: " + op);
            }
        }

        public override void Compile(CEnv env, Generator gen)
        {
            e1.Compile(env, gen);
            switch (op)
            {
                case Operator.Not:
                    gen.Emit(Instruction.NOT);
                    break;
                case Operator.Neg:
                    gen.Emit(new CSTI(0));
                    gen.Emit(Instruction.SWAP);
                    gen.Emit(Instruction.SUB);
                    break;
                default:
                    throw new Exception("Unknown unary operator: " + op);
            }
        }
    }


    // Abstract syntax for a function call expression	

    public class FuncCall : Expression
    {
        private readonly String fName;
        private readonly Expression arg;

        public FuncCall(String fName, Expression arg)
        {
            this.fName = fName; this.arg = arg;
        }

        public override Type Check(TEnv env, FEnv fenv)
        {
            Type argType = arg.Check(env, fenv);
            FuncDef fDef = fenv.getFunction(fName);
            if (fDef.CheckArgType(argType))
                return fDef.returnType;
            else
                throw new TypeException("Type error in call of function " + fName);
        }

        public override int Eval(REnv env, FEnv fenv)
        {
            int argValue = arg.Eval(env, fenv);
            FuncDef fDef = fenv.getFunction(fName);
            return fDef.Eval(env, fenv, argValue);
        }

        public override void Compile(CEnv env, Generator gen)
        {
            arg.Compile(env, gen);
            String fLabel = env.getFunctionLabel(fName);
            gen.Emit(new CALL(1, fLabel));
        }
    }



    // Function environment. Keeps track of the functions defined in a given context. 

    public class FEnv
    {
        private Dictionary<String, FuncDef> functions;

        public FEnv(Dictionary<String, FuncDef> functions)
        {
            this.functions = functions;
        }

        public void Check(TEnv env, FEnv fEnv)
        {
            foreach (FuncDef f in functions.Values)
                f.Check(env, fEnv);
        }

        public FuncDef getFunction(String name)
        {
            if (functions.ContainsKey(name))
                return functions[name];
            else
                throw new Exception("Undefined Function " + name);
        }

        public List<FuncDef> getFunctions()
        {
            return new List<FuncDef>(functions.Values);
        }

        public List<String> getFunctionNames()
        {
            return new List<String>(functions.Keys);
        }
    }

    // Runtime environments
    // Map a variable name to a Storage object that can hold an int
    // The environment is a stack because of nested scopes

    public class REnv
    {
        private readonly Stack<Pair<String, Storage>> locals;

        public REnv()
        {
            locals = new Stack<Pair<String, Storage>>();
        }

        // Find variable in innermost local scope
        public Storage GetVariable(String name)
        {
            foreach (Pair<String, Storage> variable in locals)
                if (variable.Fst == name)
                    return variable.Snd;
            throw new Exception("Unbound variable: " + name);
        }

        // Allocate variable
        public void AllocateLocal(String name)
        {
            locals.Push(new Pair<String, Storage>(name, new Storage()));
        }

        public void PopEnv()
        {
            locals.Pop();
        }
    }

    // Runtime storage for a local (int or bool) variable

    public class Storage
    {
        public int value = 0;
    }

    public struct Pair<T, U>
    {
        public readonly T Fst;
        public readonly U Snd;

        public Pair(T fst, U snd)
        {
            this.Fst = fst;
            this.Snd = snd;
        }
    }

    // Types 

    abstract public class Type
    {
        public static readonly Type intType = new PrimitiveType("int");
        public static readonly Type boolType = new PrimitiveType("bool");
    }

    public class PrimitiveType : Type
    {
        public readonly String name;

        public PrimitiveType(String name)
        {
            this.name = name;
        }

        public override String ToString() { return name; }
    }

    // Type checking environments
    // Map a variable name to a Type
    // The environment is a stack because of a nested scopes

    public class TEnv
    {
        private readonly Stack<Pair<String, Type>> locals;

        public TEnv()
        {
            locals = new Stack<Pair<String, Type>>();
        }

        public void PushEnv()
        {
            locals.Push(new Pair<String, Type>());
        }

        public void PopEnv()
        {
            locals.Pop();
        }

        public void DeclareLocal(String name, Type type)
        {
            locals.Push(new Pair<String, Type>(name, type));
        }

        public Type GetVariable(String name)
        {
            foreach (Pair<String, Type> variable in locals)
                if (variable.Fst == name)
                    return variable.Snd;
            throw new Exception("Unbound variable: " + name);
        }
    }

    // Compilation environments
    // An implicit map from string to offset (distance from stack top)
    // The environment is a stack because of nested scopes

    public class CEnv
    {
        private readonly Stack<String> locals;
        private Dictionary<String, String> labelMap;

        public CEnv(Dictionary<String, String> labelMap)
        {
            locals = new Stack<String>();
            this.labelMap = labelMap;
        }

        public void PopEnv()
        {
            locals.Pop();
        }

        public void DeclareLocal(String name)
        {
            locals.Push(name);
        }

        public void PushTemporary()
        {
            locals.Push("_ temporary _");
        }

        public void PopTemporary()
        {
            String s = locals.Pop();
            if (s != "_ temporary _")
                throw new Exception("Internal problem: popping non-temporary");
        }

        public void CompileVariable(Generator gen, String name)
        {
            int offset = 0;
            foreach (String variableName in locals)
            {
                if (variableName == name)
                {
                    gen.Emit(Instruction.GETSP);
                    gen.Emit(new CSTI(offset));
                    gen.Emit(Instruction.SUB);
                    return;
                }
                else
                    offset++;
            }
            throw new Exception("Undeclared variable: " + name);
        }

        public String getFunctionLabel(String funName)
        {
            if (labelMap.ContainsKey(funName))
                return labelMap[funName];
            else
                throw new Exception("Internal error: Undefined function " + funName);
        }
    }

    // Exceptions

    class TypeException : Exception
    {
        public TypeException(String msg) : base(msg) { }
    }

    // Code generation

    public class Generator
    {
        private readonly List<Instruction> instructions;

        public Generator()
        {
            instructions = new List<Instruction>();
        }

        public void Emit(Instruction instr)
        {
            instructions.Add(instr);
        }

        public void Label(String label)
        {
            instructions.Add(new Label(label));
        }

        public int[] ToBytecode()
        {
            // Pass 1: Build mapping from labels to absolute addresses
            Dictionary<String, int> labelMap = new Dictionary<string, int>();
            int address = 0;
            foreach (Instruction instr in instructions)
            {
                if (instr is Label)
                    labelMap.Add(((Label)instr).name, address);
                else
                    address += instr.Size;
            }
            // Pass 2: Use mapping to convert symbolic code to bytes
            List<int> bytecode = new List<int>();
            foreach (Instruction instr in instructions)
                instr.ToBytecode(labelMap, bytecode);
            return bytecode.ToArray();
        }

        public void PrintCode()
        {
            int address = 0;
            foreach (Instruction instr in instructions)
            {
                Console.WriteLine("{0,5} {1}", address, instr);
                address += instr.Size;
            }
        }
    }

    public abstract class Instruction
    {
        public readonly Opcode opcode;
        public static readonly Instruction
          ADD = new SimpleInstruction(Opcode.ADD),
          SUB = new SimpleInstruction(Opcode.SUB),
          MUL = new SimpleInstruction(Opcode.MUL),
          DIV = new SimpleInstruction(Opcode.DIV),
          MOD = new SimpleInstruction(Opcode.MOD),
          EQ = new SimpleInstruction(Opcode.EQ),
          LT = new SimpleInstruction(Opcode.LT),
          NOT = new SimpleInstruction(Opcode.NOT),
          DUP = new SimpleInstruction(Opcode.DUP),
          SWAP = new SimpleInstruction(Opcode.SWAP),
          LDI = new SimpleInstruction(Opcode.LDI),
          STI = new SimpleInstruction(Opcode.STI),
          GETBP = new SimpleInstruction(Opcode.GETBP),
          GETSP = new SimpleInstruction(Opcode.GETSP),
          PRINTC = new SimpleInstruction(Opcode.PRINTC),
          PRINTI = new SimpleInstruction(Opcode.PRINTI),
          READ = new SimpleInstruction(Opcode.READ),
          LDARGS = new SimpleInstruction(Opcode.LDARGS),
          STOP = new SimpleInstruction(Opcode.STOP);

        public Instruction(Opcode opcode)
        {
            this.opcode = opcode;
        }

        public abstract int Size { get; }

        public abstract void ToBytecode(Dictionary<string, int> labelMap, List<int> bytecode);

        public override string ToString()
        {
            return opcode.ToString();
        }
    }

    public class Label : Instruction
    {  // Pseudo-instruction
        public readonly String name;
        private static int last = 0;  // For generating fresh labels

        public Label(String name)
            : base(Opcode.LABEL)
        {
            this.name = name;
        }

        public override int Size
        {
            get { return 0; }
        }

        public override void ToBytecode(Dictionary<string, int> labelMap, List<int> bytecode)
        {
            // No bytecode for a label
        }

        public static String Fresh()
        {
            last++;
            return "L" + last.ToString();
        }

        public override string ToString()
        {
            return name + ":";
        }
    }

    public class SimpleInstruction : Instruction
    {
        public SimpleInstruction(Opcode opcode) : base(opcode) { }

        public override int Size
        {
            get { return 1; }
        }

        public override void ToBytecode(Dictionary<string, int> labelMap, List<int> bytecode)
        {
            bytecode.Add((int)opcode);
        }

        public override string ToString()
        {
            return opcode.ToString();
        }
    }

    public class JumpInstruction : Instruction
    {
        public readonly String target;

        public JumpInstruction(Opcode opcode, String target)
            : base(opcode)
        {
            this.target = target;
        }

        public override int Size
        {
            get { return 2; }
        }

        public override void ToBytecode(Dictionary<string, int> labelMap, List<int> bytecode)
        {
            bytecode.Add((int)opcode);
            bytecode.Add(labelMap[target]);
        }

        public override string ToString()
        {
            return base.ToString() + " " + target;
        }
    }

    public class GOTO : JumpInstruction
    {
        public GOTO(String target) : base(Opcode.GOTO, target) { }
    }

    public class IFZERO : JumpInstruction
    {
        public IFZERO(String target) : base(Opcode.IFZERO, target) { }
    }

    public class IFNZRO : JumpInstruction
    {
        public IFNZRO(String target) : base(Opcode.IFNZRO, target) { }
    }

    public class CALL : Instruction
    {
        public readonly int argCount;
        public readonly String target;

        public CALL(int argCount, String target)
            : base(Opcode.CALL)
        {
            this.argCount = argCount;
            this.target = target;
        }

        public override int Size
        {
            get { return 3; }
        }

        public override void ToBytecode(Dictionary<string, int> labelMap, List<int> bytecode)
        {
            bytecode.Add((int)opcode);
            bytecode.Add(argCount);
            bytecode.Add(labelMap[target]);
        }

        public override string ToString()
        {
            return base.ToString() + " " + argCount.ToString() + " " + target;
        }
    }

    public class TCALL : Instruction
    {
        public readonly int argCount;
        public readonly int slideBy;
        public readonly String target;

        public TCALL(int argCount, int slideBy, String target)
            : base(Opcode.TCALL)
        {
            this.argCount = argCount;
            this.slideBy = slideBy;
            this.target = target;
        }

        public override int Size
        {
            get { return 4; }
        }

        public override void ToBytecode(Dictionary<string, int> labelMap, List<int> bytecode)
        {
            bytecode.Add((int)opcode);
            bytecode.Add(argCount);
            bytecode.Add(slideBy);
            bytecode.Add(labelMap[target]);
        }
    }

    public class IntArgInstr : Instruction
    {
        public readonly int argument;

        public IntArgInstr(Opcode opcode, int argument)
            : base(opcode)
        {
            this.argument = argument;
        }

        public override int Size
        {
            get { return 2; }
        }

        public override void ToBytecode(Dictionary<string, int> labelMap, List<int> bytecode)
        {
            bytecode.Add((int)opcode);
            bytecode.Add(argument);
        }

        public override string ToString()
        {
            return base.ToString() + " " + argument.ToString();
        }
    }

    public class CSTI : IntArgInstr
    {
        public CSTI(int argument) : base(Opcode.CSTI, argument) { }
    }

    public class INCSP : IntArgInstr
    {
        public INCSP(int argument) : base(Opcode.INCSP, argument) { }
    }

    public class RET : IntArgInstr
    {
        public RET(int argument) : base(Opcode.RET, argument) { }
    }

    public enum Opcode
    {
        LABEL = -1, // Unused
        CSTI, ADD, SUB, MUL, DIV, MOD, EQ, LT, NOT,
        DUP, SWAP, LDI, STI, GETBP, GETSP, INCSP,
        GOTO, IFZERO, IFNZRO, CALL, TCALL, RET,
        PRINTI, PRINTC, READ, LDARGS,
        STOP
    }
}
