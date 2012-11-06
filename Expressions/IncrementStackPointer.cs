namespace Expressions
{
    public class IncrementStackPointer : IntArgInstr
    {
        public IncrementStackPointer(int argument)
            : base(Opcode.INCSP, argument)
        {
        }
    }
}