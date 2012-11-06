namespace Expressions
{
    /// <summary>
    /// Expression abstract syntax
    /// </summary>
    public abstract class Expression
    {
        abstract public int Eval(REnv runtimeEnvironment, FEnv functionEnvironment);
        abstract public Type Check(TEnv typeCheckingEnvironment, FEnv functionEnvironment);
        abstract public void Compile(CEnv env, Generator gen);
    }
}