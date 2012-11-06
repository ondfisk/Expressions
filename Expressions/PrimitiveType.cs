using System;

namespace Expressions
{
    public class PrimitiveType : Type
    {
        private readonly string _name;

        public string Name
        {
            get { return _name; }
        }

        public PrimitiveType(String name)
        {
            _name = name;
        }

        public override string ToString()
        {
            return _name;
        }
    }
}