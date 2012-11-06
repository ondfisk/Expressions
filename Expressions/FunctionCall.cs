using System;

namespace Expressions
{
    /// <summary>
    /// Abstract syntax for a function call expression
    /// </summary>
    public class FunctionCall : Expression
    {
        private readonly string _name;
        private readonly Expression _arg;

        public FunctionCall(string name, Expression arg)
        {
            _name = name;
            _arg = arg;
        }

        public override Type Check(TypeCheckingEnvironment typeCheckingEnvironment, FunctionEnvironment functionEnvironment)
        {
            var argumentType = _arg.Check(typeCheckingEnvironment, functionEnvironment);
            var function = functionEnvironment.getFunction(_name);
            if (function.CheckArgType(argumentType))
            {
                return function.returnType;
            }
            throw new InvalidOperationException(string.Format("Type error in call of function {0}", _name));
        }

        public override int Eval(RuntimeEnvironment runtimeEnvironment, FunctionEnvironment functionEnvironment)
        {
            var argumentValue = _arg.Eval(runtimeEnvironment, functionEnvironment);
            var function = functionEnvironment.getFunction(_name);
            return function.Eval(runtimeEnvironment, functionEnvironment, argumentValue);
        }

        public override void Compile(CompilationEnvironment env, Generator gen)
        {
            _arg.Compile(env, gen);
            var label = env.GetFunctionLabel(_name);
            gen.Emit(new Call(1, label));
        }
    }
}