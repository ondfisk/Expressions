using System.Collections.Generic;

namespace Expressions
{
    public abstract class Instruction
    {
        public readonly Opcode opcode;
        public static readonly Instruction
            ADD = new SimpleInstruction(Opcode.ADD),
            SUB = new SimpleInstruction(Opcode.SUB),
            MUL = new SimpleInstruction(Opcode.MUL),
            DIV = new SimpleInstruction(Opcode.DIV),
            MOD = new SimpleInstruction(Opcode.MOD),
            EQ = new SimpleInstruction(Opcode.EQ),
            LT = new SimpleInstruction(Opcode.LT),
            NOT = new SimpleInstruction(Opcode.NOT),
            DUP = new SimpleInstruction(Opcode.DUP),
            SWAP = new SimpleInstruction(Opcode.SWAP),
            LDI = new SimpleInstruction(Opcode.LDI),
            STI = new SimpleInstruction(Opcode.STI),
            GETBP = new SimpleInstruction(Opcode.GETBP),
            GETSP = new SimpleInstruction(Opcode.GETSP),
            PRINTC = new SimpleInstruction(Opcode.PRINTC),
            PRINTI = new SimpleInstruction(Opcode.PRINTI),
            READ = new SimpleInstruction(Opcode.READ),
            LDARGS = new SimpleInstruction(Opcode.LDARGS),
            STOP = new SimpleInstruction(Opcode.STOP);

        public Instruction(Opcode opcode)
        {
            this.opcode = opcode;
        }

        public abstract int Size { get; }

        public abstract void ToBytecode(Dictionary<string, int> labelMap, List<int> bytecode);

        public override string ToString()
        {
            return opcode.ToString();
        }
    }
}