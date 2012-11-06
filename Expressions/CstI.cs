namespace Expressions
{
    public class CstI : IntegerArgumentInstruction
    {
        public CstI(int argument)
            : base(Opcode.CSTI, argument)
        {
        }
    }
}