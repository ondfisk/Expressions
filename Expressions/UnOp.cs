using System;

namespace Expressions
{
    public class UnOp : Expression
    {
        private readonly Operator op;
        private readonly Expression e1;

        public UnOp(Operator op, Expression e1)
        {
            this.op = op;
            this.e1 = e1;
        }

        public override int Eval(REnv env, FEnv fEnv)
        {
            int v1 = e1.Eval(env, fEnv);
            switch (op)
            {
                case Operator.Not:
                    return v1 == 0 ? 1 : 0;
                case Operator.Neg:
                    return -v1;
                default:
                    throw new Exception("Unknown unary operator: " + op);
            }
        }

        public override Type Check(TEnv env, FEnv fEnv)
        {
            Type t1 = e1.Check(env, fEnv);
            switch (op)
            {
                case Operator.Neg:
                    if (t1 == Type.intType)
                        return Type.intType;
                    else
                        throw new TypeException("Argument to - must be int");
                case Operator.Not:
                    if (t1 == Type.boolType)
                        return Type.boolType;
                    else
                        throw new TypeException("Argument to ! must be bool");
                default:
                    throw new Exception("Unknown unary operator: " + op);
            }
        }

        public override void Compile(CEnv env, Generator gen)
        {
            e1.Compile(env, gen);
            switch (op)
            {
                case Operator.Not:
                    gen.Emit(Instruction.NOT);
                    break;
                case Operator.Neg:
                    gen.Emit(new CSTI(0));
                    gen.Emit(Instruction.SWAP);
                    gen.Emit(Instruction.SUB);
                    break;
                default:
                    throw new Exception("Unknown unary operator: " + op);
            }
        }
    }
}