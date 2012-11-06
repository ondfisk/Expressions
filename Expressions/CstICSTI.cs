namespace Expressions
{
    public class CstICSTI : IntArgInstr
    {
        public CstICSTI(int argument) : base(Opcode.CSTI, argument) { }
    }
}