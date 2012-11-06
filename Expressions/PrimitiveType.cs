using System;

namespace Expressions
{
    public class PrimitiveType : Type
    {
        public readonly String name;

        public PrimitiveType(String name)
        {
            this.name = name;
        }

        public override String ToString() { return name; }
    }
}