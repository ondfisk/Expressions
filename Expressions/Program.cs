using System;
using System.Collections.Generic;
using System.IO;

namespace Expressions
{
    /// <summary>
    /// Programs abstract syntax. A program consists of a term to be evaluated and a function environment in which it is evaluated
    /// </summary>
    public class Program
    {
        private FEnv fenv;
        private Expression e;

        public Program(Dictionary<string, FuncDef> functions, Expression e)
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
}