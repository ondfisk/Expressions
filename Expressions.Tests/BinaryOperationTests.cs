using NUnit.Framework;

namespace Expressions.Tests
{
    [TestFixture]
    public class BinaryOperationTests
    {
        [Test]
        public void Multiply_Given_6_and_7_should_be_type_of_integer()
        {
            const string expression = "6 * 7";

            var result = Check(expression);

            Assert.AreEqual(Type.IntegerType, result);
        }

        [Test]
        public void Multiply_Given_6_and_7_should_return_42()
        {
            const string expression = "6 * 7";

            var result = Eval(expression);

            Assert.AreEqual(42, result);
        }

        private Type Check(string expression)
        {
            var scanner = new Scanner(expression.ToStream());
            var parser = new Parser(scanner);
            parser.Parse();

            return parser.program.Check();
        }

        private int Eval(string expression)
        {
            var scanner = new Scanner(expression.ToStream());
            var parser = new Parser(scanner);
            parser.Parse();

            return parser.program.Eval();
        }
    }
}
