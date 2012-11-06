using System;
using System.Collections.Generic;

namespace Expressions
{
    /// <summary>
    /// Code generation 
    /// </summary>
    public class Generator
    {
        private readonly List<Instruction> instructions;

        public Generator()
        {
            instructions = new List<Instruction>();
        }

        public void Emit(Instruction instr)
        {
            instructions.Add(instr);
        }

        public void Label(String label)
        {
            instructions.Add(new Label(label));
        }

        public int[] ToBytecode()
        {
            // Pass 1: Build mapping from labels to absolute addresses
            Dictionary<String, int> labelMap = new Dictionary<string, int>();
            int address = 0;
            foreach (Instruction instr in instructions)
            {
                if (instr is Label)
                    labelMap.Add(((Label)instr).name, address);
                else
                    address += instr.Size;
            }
            // Pass 2: Use mapping to convert symbolic code to bytes
            List<int> bytecode = new List<int>();
            foreach (Instruction instr in instructions)
                instr.ToBytecode(labelMap, bytecode);
            return bytecode.ToArray();
        }

        public void PrintCode()
        {
            int address = 0;
            foreach (Instruction instr in instructions)
            {
                Console.WriteLine("{0,5} {1}", address, instr);
                address += instr.Size;
            }
        }
    }
}