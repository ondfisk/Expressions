using System;
using System.Collections.Generic;

namespace Expressions
{
    /// <summary>
    /// Function environment. Keeps track of the functions defined in a given context. 
    /// </summary>
    public class FunctionEnvironment
    {
        private Dictionary<string, FunctionDefinition> functions;

        public FunctionEnvironment(Dictionary<string, FunctionDefinition> functions)
        {
            this.functions = functions;
        }

        public void Check(TypeCheckingEnvironment typeCheckingEnvironment, FunctionEnvironment functionEnvironment)
        {
            foreach (FunctionDefinition f in functions.Values)
                f.Check(typeCheckingEnvironment, functionEnvironment);
        }

        public FunctionDefinition getFunction(String name)
        {
            if (functions.ContainsKey(name))
                return functions[name];
            else
                throw new Exception("Undefined Function " + name);
        }

        public List<FunctionDefinition> getFunctions()
        {
            return new List<FunctionDefinition>(functions.Values);
        }

        public List<String> getFunctionNames()
        {
            return new List<String>(functions.Keys);
        }
    }
}