using System;

namespace Expressions
{
    public class IFZERO : JumpInstruction
    {
        public IFZERO(String target) : base(Opcode.IFZERO, target) { }
    }
}