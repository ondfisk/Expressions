using System;
using System.Collections.Generic;

namespace Expressions
{
    public class TCALL : Instruction
    {
        public readonly int argCount;
        public readonly int slideBy;
        public readonly String target;

        public TCALL(int argCount, int slideBy, String target)
            : base(Opcode.TCALL)
        {
            this.argCount = argCount;
            this.slideBy = slideBy;
            this.target = target;
        }

        public override int Size
        {
            get { return 4; }
        }

        public override void ToBytecode(Dictionary<string, int> labelMap, List<int> bytecode)
        {
            bytecode.Add((int)opcode);
            bytecode.Add(argCount);
            bytecode.Add(slideBy);
            bytecode.Add(labelMap[target]);
        }
    }
}