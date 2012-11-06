using System;

namespace Expressions
{
    public class Variable : Expression
    {
        private readonly string name;

        public Variable(string name)
        {
            this.name = name;
        }

        public override int Eval(RuntimeEnvironment runtimeEnvironment, FunctionEnvironment functionEnvironment)
        {
            return runtimeEnvironment.GetVariable(name).Value;
        }

        public override Type Check(TypeCheckingEnvironment typeCheckingEnvironment, FunctionEnvironment functionEnvironment)
        {
            return typeCheckingEnvironment.GetVariable(name);
        }

        public override void Compile(CompilationEnvironment compilationEnvironment, Generator generator)
        {
            compilationEnvironment.CompileVariable(generator, name);
            generator.Emit(Instruction.LdI);
        }
    }
}