using System;

namespace Expressions
{
    /// <summary>
    /// Abstract syntax for a function call expression
    /// </summary>
    public class FuncCall : Expression
    {
        private readonly String fName;
        private readonly Expression arg;

        public FuncCall(String fName, Expression arg)
        {
            this.fName = fName; this.arg = arg;
        }

        public override Type Check(TEnv env, FEnv fenv)
        {
            Type argType = arg.Check(env, fenv);
            FuncDef fDef = fenv.getFunction(fName);
            if (fDef.CheckArgType(argType))
                return fDef.returnType;
            else
                throw new TypeException("Type error in call of function " + fName);
        }

        public override int Eval(REnv env, FEnv fenv)
        {
            int argValue = arg.Eval(env, fenv);
            FuncDef fDef = fenv.getFunction(fName);
            return fDef.Eval(env, fenv, argValue);
        }

        public override void Compile(CEnv env, Generator gen)
        {
            arg.Compile(env, gen);
            String fLabel = env.getFunctionLabel(fName);
            gen.Emit(new CALL(1, fLabel));
        }
    }
}