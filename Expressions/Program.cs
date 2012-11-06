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
        private FunctionEnvironment fenv;
        private Expression e;

        public Program(Dictionary<string, FunctionDefinition> functions, Expression e)
        {
            fenv = new FunctionEnvironment(functions);
            this.e = e;
        }

        public int Eval()
        {
            RuntimeEnvironment renv = new RuntimeEnvironment();
            return e.Eval(renv, fenv);
        }

        public Type Check()
        {
            TypeCheckingEnvironment tenv = new TypeCheckingEnvironment();
            fenv.Check(tenv, fenv);
            return e.Check(tenv, fenv);
        }

        public void Compile(Generator gen, String outputFile)
        {
            // Generate compiletime environment
            var labelMap = new Dictionary<String, String>();
            foreach (String funName in fenv.getFunctionNames())
                labelMap.Add(funName, Label.Fresh());
            CompilationEnvironment cenv = new CompilationEnvironment(labelMap);

            // Compile expression
            e.Compile(cenv, gen);
            gen.Emit(Instruction.PRINTI);
            gen.Emit(Instruction.STOP);

            // Compile functions
            foreach (FunctionDefinition f in fenv.getFunctions())
            {
                cenv = new CompilationEnvironment(labelMap);
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