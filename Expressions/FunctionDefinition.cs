using System;

namespace Expressions
{
    /// <summary>
    /// Abstract syntax for function definitions.
    /// </summary>
    public class FunctionDefinition
    {
        private readonly string _name;
        private readonly Tuple<string, Type> _argument;
        private readonly Expression _body;
        private readonly Type _returnType;

        public Type ReturnType
        {
            get { return _returnType; }
        }

        public FunctionDefinition(Type returnType, string name, Type argumentType, string argumentName, Expression body)
        {
            _argument = Tuple.Create(argumentName, argumentType);
            _body = body; 
            _returnType = returnType;
            _name = name;
        }

        public void Check(TypeCheckingEnvironment typeCheckingEnvironment, FunctionEnvironment functionEnvironment)
        {
            typeCheckingEnvironment.DeclareLocal(_argument.Item1, _argument.Item2);
            var t = _body.Check(typeCheckingEnvironment, functionEnvironment);
            typeCheckingEnvironment.PopEnv();
            if (t != _returnType)
            {
                throw new InvalidOperationException(string.Format("Body of {0} returns {1}, {2} expected", _name, t, _returnType));
            }
        }

        public int Eval(RuntimeEnvironment runtimeEnvironment, FunctionEnvironment functionEnvironment, int argument)
        {
            runtimeEnvironment.AllocateLocal(_argument.Item1);
            runtimeEnvironment.GetVariable(_argument.Item1).value = argument;
            var v = _body.Eval(runtimeEnvironment, functionEnvironment);
            runtimeEnvironment.PopEnv();
            return v;
        }

        public void Compile(Generator gen, CompilationEnvironment env)
        {
            env.DeclareLocal(_argument.Item1); // Formal argument name points to top of stack
            gen.Label(env.GetFunctionLabel(_name));
            _body.Compile(env, gen);
            gen.Emit(new Return(1));
        }

        public bool CheckArgType(Type argType)
        {
            return argType == _argument.Item2;
        }
    }
}