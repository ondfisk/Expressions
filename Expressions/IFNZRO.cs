using System;

namespace Expressions
{
    public class IFNZRO : JumpInstruction
    {
        public IFNZRO(String target) : base(Opcode.IFNZRO, target) { }
    }
}