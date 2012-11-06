namespace Expressions
{
    public class CSTI : IntArgInstr
    {
        public CSTI(int argument) : base(Opcode.CSTI, argument) { }
    }
}