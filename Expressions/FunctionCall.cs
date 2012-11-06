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
            var function = functionEnvironment.GetFunction(_name);
            if (function.CheckArgType(argumentType))
            {
                return function.ReturnType;
            }
            throw new InvalidOperationException(string.Format("Type error in call of function {0}", _name));
        }

        public override int Eval(RuntimeEnvironment runtimeEnvironment, FunctionEnvironment functionEnvironment)
        {
            var argumentValue = _arg.Eval(runtimeEnvironment, functionEnvironment);
            var function = functionEnvironment.GetFunction(_name);
            return function.Eval(runtimeEnvironment, functionEnvironment, argumentValue);
        }

        public override void Compile(CompilationEnvironment compilationEnvironment, Generator generator)
        {
            _arg.Compile(compilationEnvironment, generator);
            var label = compilationEnvironment.GetFunctionLabel(_name);
            generator.Emit(new Call(1, label));
        }
    }
}