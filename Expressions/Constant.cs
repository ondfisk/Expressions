namespace Expressions
{
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
}