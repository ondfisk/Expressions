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
}