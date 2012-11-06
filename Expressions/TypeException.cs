using System;

namespace Expressions
{
    /// <summary>
    /// Exceptions
    /// </summary>
    class TypeException : Exception
    {
        public TypeException(String msg) : base(msg) { }
    }
}