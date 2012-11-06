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

        public override int Eval(REnv runtimeEnvironment, FEnv functionEnvironment)
        {
            return runtimeEnvironment.GetVariable(name).value;
        }

        public override Type Check(TEnv typeCheckingEnvironment, FEnv functionEnvironment)
        {
            return typeCheckingEnvironment.GetVariable(name);
        }

        public override void Compile(CEnv env, Generator gen)
        {
            env.CompileVariable(gen, name);
            gen.Emit(Instruction.LDI);
        }
    }
}