using System;
using System.Collections.Generic;

namespace Expressions
{
    public class JumpInstruction : Instruction
    {
        public readonly String target;

        public JumpInstruction(Opcode opcode, String target)
            : base(opcode)
        {
            this.target = target;
        }

        public override int Size
        {
            get { return 2; }
        }

        public override void ToBytecode(IDictionary<string, int> labelMap, List<int> bytecode)
        {
            bytecode.Add((int)Opcode);
            bytecode.Add(labelMap[target]);
        }

        public override string ToString()
        {
            return base.ToString() + " " + target;
        }
    }
}