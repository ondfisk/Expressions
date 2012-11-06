using System;
using System.Collections.Generic;

namespace Expressions
{
    public class CALL : Instruction
    {
        public readonly int argCount;
        public readonly String target;

        public CALL(int argCount, String target)
            : base(Opcode.CALL)
        {
            this.argCount = argCount;
            this.target = target;
        }

        public override int Size
        {
            get { return 3; }
        }

        public override void ToBytecode(Dictionary<string, int> labelMap, List<int> bytecode)
        {
            bytecode.Add((int)opcode);
            bytecode.Add(argCount);
            bytecode.Add(labelMap[target]);
        }

        public override string ToString()
        {
            return base.ToString() + " " + argCount.ToString() + " " + target;
        }
    }
}