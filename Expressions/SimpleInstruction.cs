using System.Collections.Generic;

namespace Expressions
{
    public class SimpleInstruction : Instruction
    {
        public SimpleInstruction(Opcode opcode) : base(opcode) { }

        public override int Size
        {
            get { return 1; }
        }

        public override void ToBytecode(Dictionary<string, int> labelMap, List<int> bytecode)
        {
            bytecode.Add((int)opcode);
        }

        public override string ToString()
        {
            return opcode.ToString();
        }
    }
}