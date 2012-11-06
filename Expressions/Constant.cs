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

        public override int Eval(RuntimeEnvironment runtimeEnvironment, FunctionEnvironment functionEnvironment)
        {
            return value;
        }

        public override Type Check(TypeCheckingEnvironment typeCheckingEnvironment, FunctionEnvironment functionEnvironment)
        {
            return type;
        }

        public override void Compile(CompilationEnvironment env, Generator gen)
        {
            gen.Emit(new CSTI(value));
        }
    }
}