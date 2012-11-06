using System;
using System.Collections.Generic;

namespace Expressions
{
    /// <summary>
    /// Function environment. Keeps track of the functions defined in a given context. 
    /// </summary>
    public class FEnv
    {
        private Dictionary<string, FuncDef> functions;

        public FEnv(Dictionary<string, FuncDef> functions)
        {
            this.functions = functions;
        }

        public void Check(TEnv env, FEnv fEnv)
        {
            foreach (FuncDef f in functions.Values)
                f.Check(env, fEnv);
        }

        public FuncDef getFunction(String name)
        {
            if (functions.ContainsKey(name))
                return functions[name];
            else
                throw new Exception("Undefined Function " + name);
        }

        public List<FuncDef> getFunctions()
        {
            return new List<FuncDef>(functions.Values);
        }

        public List<String> getFunctionNames()
        {
            return new List<String>(functions.Keys);
        }
    }
}