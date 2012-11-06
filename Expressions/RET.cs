// Abstract syntax, interpretation and compilation 
// for arithmetic and logical expressions
// sestoft@itu.dk 2007-03-12, 2010-02-08
// Extended with functions by Rasmus Mogelberg 2011-10-20

namespace Expressions
{
    public class RET : IntegerArgumentInstruction
    {
        public RET(int argument) : base(Opcode.Ret, argument) { }
    }
}
