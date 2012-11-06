using System;
using System.Collections.Generic;

namespace Expressions
{
    /// <summary>
    /// Type checking environments
    /// Map a variable name to a Type
    /// The environment is a stack because of a nested scopes
    /// </summary>
    public class TypeCheckingEnvironment
    {
        private readonly Stack<Tuple<string, Type>> locals;

        public TypeCheckingEnvironment()
        {
            locals = new Stack<Tuple<string, Type>>();
        }

        public void PushEnv()
        {
            locals.Push(new Tuple<string, Type>(null, null));
        }

        public void PopEnv()
        {
            locals.Pop();
        }

        public void DeclareLocal(String name, Type type)
        {
            locals.Push(new Tuple<String, Type>(name, type));
        }

        public Type GetVariable(String name)
        {
            foreach (Tuple<String, Type> variable in locals)
                if (variable.Item1 == name)
                    return variable.Item2;
            throw new Exception("Unbound variable: " + name);
        }
    }
}