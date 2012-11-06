using System;

namespace Expressions
{
    public class GOTO : JumpInstruction
    {
        public GOTO(String target) : base(Opcode.GOTO, target) { }
    }
}