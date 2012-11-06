namespace Expressions
{
    public class IncSP : IntArgInstr
    {
        public IncSP(int argument)
            : base(Opcode.INCSP, argument)
        {
        }
    }
}