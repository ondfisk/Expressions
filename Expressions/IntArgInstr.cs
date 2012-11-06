using System.Collections.Generic;

namespace Expressions
{
    public class IntArgInstr : Instruction
    {
        public readonly int argument;

        public IntArgInstr(Opcode opcode, int argument)
            : base(opcode)
        {
            this.argument = argument;
        }

        public override int Size
        {
            get { return 2; }
        }

        public override void ToBytecode(Dictionary<string, int> labelMap, List<int> bytecode)
        {
            bytecode.Add((int)opcode);
            bytecode.Add(argument);
        }

        public override string ToString()
        {
            return base.ToString() + " " + argument.ToString();
        }
    }
}