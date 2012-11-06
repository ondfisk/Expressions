using System;
using System.Collections.Generic;

namespace Expressions
{
    public class Label : Instruction
    {  // Pseudo-instruction
        public readonly String name;
        private static int last = 0;  // For generating fresh labels

        public Label(String name)
            : base(Opcode.LABEL)
        {
            this.name = name;
        }

        public override int Size
        {
            get { return 0; }
        }

        public override void ToBytecode(IDictionary<string, int> labelMap, List<int> bytecode)
        {
            // No bytecode for a label
        }

        public static String Fresh()
        {
            last++;
            return "L" + last.ToString();
        }

        public override string ToString()
        {
            return name + ":";
        }
    }
}