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

        public override int Eval(REnv runtimeEnvironment, FEnv functionEnvironment)
        {
            return value;
        }

        public override Type Check(TEnv typeCheckingEnvironment, FEnv functionEnvironment)
        {
            return type;
        }

        public override void Compile(CEnv env, Generator gen)
        {
            gen.Emit(new CSTI(value));
        }
    }
}