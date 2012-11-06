using System;
using System.Collections.Generic;

namespace Expressions
{
    /// <summary>
    /// Runtime environments
    /// Map a variable name to a Storage object that can hold an int
    /// The environment is a stack because of nested scopes
    /// </summary>
    public class REnv
    {
        private readonly Stack<Pair<string, Storage>> locals;

        public REnv()
        {
            locals = new Stack<Pair<string, Storage>>();
        }

        // Find variable in innermost local scope
        public Storage GetVariable(String name)
        {
            foreach (Pair<String, Storage> variable in locals)
                if (variable.Fst == name)
                    return variable.Snd;
            throw new Exception("Unbound variable: " + name);
        }

        // Allocate variable
        public void AllocateLocal(String name)
        {
            locals.Push(new Pair<String, Storage>(name, new Storage()));
        }

        public void PopEnv()
        {
            locals.Pop();
        }
    }
}