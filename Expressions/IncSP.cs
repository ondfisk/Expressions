namespace Expressions
{
    public class IncSp : IntegerArgumentInstruction
    {
        public IncSp(int argument)
            : base(Opcode.INCSP, argument)
        {
        }
    }
}