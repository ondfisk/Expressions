using System.IO;

namespace Expressions.Tests
{
    public static class ExpressionsExtensions
    {
        public static Stream ToStream(this string expression)
        {
            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);
            writer.Write(expression);
            writer.Flush();
            return stream;
        }
    }
}