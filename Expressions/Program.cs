using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Expressions
{
    /// <summary>
    /// Programs abstract syntax. A Program consists of a term to be evaluated and a function environment in which it is evaluated
    /// </summary>
    public class Program
    {
        private readonly FunctionEnvironment _functionEnvironment;
        private readonly Expression _expression;

        public Program(IDictionary<string, FunctionDefinition> functions, Expression expression)
        {
            _functionEnvironment = new FunctionEnvironment(functions);
            _expression = expression;
        }

        public int Eval()
        {
            var renv = new RuntimeEnvironment();
            return _expression.Eval(renv, _functionEnvironment);
        }

        public Type Check()
        {
            var tenv = new TypeCheckingEnvironment();
            _functionEnvironment.Check(tenv, _functionEnvironment);
            return _expression.Check(tenv, _functionEnvironment);
        }

        public void Compile(Generator gen, String outputFile)
        {
            // Generate compiletime environment
            var labels = _functionEnvironment.GetFunctionNames().ToDictionary(funName => funName, funName => Label.Fresh());
            var compilationEnvironment = new CompilationEnvironment(labels);

            // Compile expression
            _expression.Compile(compilationEnvironment, gen);
            gen.Emit(Instruction.PrintI);
            gen.Emit(Instruction.Stop);

            // Compile functions
            foreach (var functionDefinition in _functionEnvironment.GetFunctions())
            {
                compilationEnvironment = new CompilationEnvironment(labels);
                functionDefinition.Compile(gen, compilationEnvironment);
            }

            //  Generate bytecode at and print to file
            gen.PrintCode();
            var bytecode = gen.ToBytecode();
            using (TextWriter writer = new StreamWriter(outputFile))
            {
                foreach (var b in bytecode)
                {
                    writer.Write(b);
                    writer.Write(" ");
                }
            }
        }
    }
}