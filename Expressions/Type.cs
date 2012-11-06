namespace Expressions
{
    /// <summary>
    /// Types
    /// </summary>
    abstract public class Type
    {
        public static readonly Type intType = new PrimitiveType("int");
        public static readonly Type boolType = new PrimitiveType("bool");
    }
}