namespace Expressions
{
    public class GotoGoto : JumpInstruction
    {
        public GotoGoto(string target) : base(Opcode.GOTO, target) { }
    }
}