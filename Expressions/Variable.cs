using System;

namespace Expressions
{
    public class Variable : Expression
    {
        private readonly String name;

        public Variable(String name)
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

        public override void Compile(CompilationEnvironment env, Generator gen)
        {
            env.CompileVariable(gen, name);
            gen.Emit(Instruction.LdI);
        }
    }
}