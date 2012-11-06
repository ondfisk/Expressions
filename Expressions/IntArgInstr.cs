using System.Collections.Generic;

namespace Expressions
{
    public class IntArgInstr : Instruction
    {
        private readonly int _argument;

        public override int Size
        {
            get { return 2; }
        }

        public int Argument
        {
            get { return _argument; }
        }

        public IntArgInstr(Opcode opcode, int argument)
            : base(opcode)
        {
            _argument = argument;
        }

        public override void ToBytecode(IDictionary<string, int> labelMap, List<int> bytecode)
        {
            bytecode.Add((int)Opcode);
            bytecode.Add(_argument);
        }

        public override string ToString()
        {
            return string.Join(" ", base.ToString(), _argument);
        }
    }
}