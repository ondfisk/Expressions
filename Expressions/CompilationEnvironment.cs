using System;
using System.Collections.Generic;

namespace Expressions
{
    /// <summary>
    /// Compilation environments
    /// An implicit map from string to offset (distance from stack top)
    /// The environment is a stack because of nested scopes
    /// </summary>
    public class CompilationEnvironment
    {
        private readonly Stack<string> locals;
        private Dictionary<string, string> labelMap;

        public CompilationEnvironment(Dictionary<string, string> labelMap)
        {
            locals = new Stack<string>();
            this.labelMap = labelMap;
        }

        public void PopEnv()
        {
            locals.Pop();
        }

        public void DeclareLocal(String name)
        {
            locals.Push(name);
        }

        public void PushTemporary()
        {
            locals.Push("_ temporary _");
        }

        public void PopTemporary()
        {
            String s = locals.Pop();
            if (s != "_ temporary _")
                throw new Exception("Internal problem: popping non-temporary");
        }

        public void CompileVariable(Generator gen, String name)
        {
            int offset = 0;
            foreach (String variableName in locals)
            {
                if (variableName == name)
                {
                    gen.Emit(Instruction.GETSP);
                    gen.Emit(new CstI(offset));
                    gen.Emit(Instruction.SUB);
                    return;
                }
                else
                    offset++;
            }
            throw new Exception("Undeclared variable: " + name);
        }

        public String getFunctionLabel(String funName)
        {
            if (labelMap.ContainsKey(funName))
                return labelMap[funName];
            else
                throw new Exception("Internal error: Undefined function " + funName);
        }
    }
}