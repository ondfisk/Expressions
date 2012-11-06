namespace Expressions
{
    public class IncSp : IntArgInstr
    {
        public IncSp(int argument)
            : base(Opcode.INCSP, argument)
        {
        }
    }
}