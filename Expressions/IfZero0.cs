using System;

namespace Expressions
{
    public class IfZero0 : JumpInstruction
    {
        public IfZero0(String target) : base(Opcode.IFZERO, target) { }
    }
}