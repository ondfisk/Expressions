namespace Expressions
{
    public class CstI : IntArgInstr
    {
        public CstI(int argument) : base(Opcode.CSTI, argument) { }
    }
}