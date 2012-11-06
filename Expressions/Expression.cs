namespace Expressions
{
    /// <summary>
    /// Expression abstract syntax
    /// </summary>
    public abstract class Expression
    {
        abstract public int Eval(REnv env, FEnv fEnv);
        abstract public Type Check(TEnv env, FEnv fEnv);
        abstract public void Compile(CEnv env, Generator gen);
    }
}