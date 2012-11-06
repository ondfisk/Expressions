using System.Collections.Generic;

namespace Expressions
{
    public abstract class Instruction
    {
        private readonly Opcode _opcode;

        public static readonly Instruction
            Add = new SimpleInstruction(Opcode.ADD),
            Sub = new SimpleInstruction(Opcode.SUB),
            Mul = new SimpleInstruction(Opcode.MUL),
            Div = new SimpleInstruction(Opcode.DIV),
            Mod = new SimpleInstruction(Opcode.MOD),
            Eq = new SimpleInstruction(Opcode.EQ),
            LT = new SimpleInstruction(Opcode.LT),
            Not = new SimpleInstruction(Opcode.NOT),
            Dup = new SimpleInstruction(Opcode.DUP),
            Swap = new SimpleInstruction(Opcode.SWAP),
            LdI = new SimpleInstruction(Opcode.LDI),
            StI = new SimpleInstruction(Opcode.STI),
            GetBp = new SimpleInstruction(Opcode.GETBP),
            GetSp = new SimpleInstruction(Opcode.GETSP),
            PrintC = new SimpleInstruction(Opcode.PRINTC),
            PrintI = new SimpleInstruction(Opcode.PRINTI),
            Read = new SimpleInstruction(Opcode.READ),
            LdArgs = new SimpleInstruction(Opcode.LDARGS),
            Stop = new SimpleInstruction(Opcode.STOP);

        public Opcode Opcode
        {
            get { return _opcode; }
        }

        public abstract int Size { get; }

        protected Instruction(Opcode opcode)
        {
            _opcode = opcode;
        }

        public abstract void ToBytecode(IDictionary<string, int> labels, List<int> bytecode);

        public override string ToString()
        {
            return _opcode.ToString();
        }
    }
}