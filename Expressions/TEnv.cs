using System;
using System.Collections.Generic;

namespace Expressions
{
    /// <summary>
    /// Type checking environments
    /// Map a variable name to a Type
    /// The environment is a stack because of a nested scopes
    /// </summary>
    public class TEnv
    {
        private readonly Stack<Pair<string, Type>> locals;

        public TEnv()
        {
            locals = new Stack<Pair<string, Type>>();
        }

        public void PushEnv()
        {
            locals.Push(new Pair<string, Type>());
        }

        public void PopEnv()
        {
            locals.Pop();
        }

        public void DeclareLocal(String name, Type type)
        {
            locals.Push(new Pair<String, Type>(name, type));
        }

        public Type GetVariable(String name)
        {
            foreach (Pair<String, Type> variable in locals)
                if (variable.Fst == name)
                    return variable.Snd;
            throw new Exception("Unbound variable: " + name);
        }
    }
}