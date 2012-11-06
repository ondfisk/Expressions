using System;

namespace Expressions
{
    /// <summary>
    /// Abstract syntax for function definitions.
    /// </summary>
    public class FuncDef
    {
        private readonly String fName;
        private readonly Tuple<String, Type> formArg;
        private readonly Expression body;
        public readonly Type returnType;

        public FuncDef(Type returnType, String fName, Type argType, String argName, Expression body)
        {
            this.formArg = new Tuple<string, Type>(argName, argType);
            this.body = body; this.returnType = returnType;
            this.fName = fName;
        }

        public void Check(TypeCheckingEnvironment typeCheckingEnvironment, FunctionEnvironment functionEnvironment)
        {
            typeCheckingEnvironment.DeclareLocal(formArg.Item1, formArg.Item2);
            Type t = body.Check(typeCheckingEnvironment, functionEnvironment);
            typeCheckingEnvironment.PopEnv();
            if (t != returnType)
                throw new InvalidOperationException("Body of " + fName + " returns " + t + ", " + returnType + " expected");
        }

        public int Eval(RuntimeEnvironment env, FunctionEnvironment fenv, int arg)
        {
            env.AllocateLocal(formArg.Item1);
            env.GetVariable(formArg.Item1).value = arg;
            int v = body.Eval(env, fenv);
            env.PopEnv();
            return v;
        }

        public void Compile(Generator gen, CompilationEnvironment env)
        {
            env.DeclareLocal(formArg.Item1); // Formal argument name points to top of stack
            gen.Label(env.GetFunctionLabel(fName));
            body.Compile(env, gen);
            gen.Emit(new RET(1));
        }

        public bool CheckArgType(Type argType)
        {
            if (argType != formArg.Item2)
                return false;
            return true;
        }
    }
}