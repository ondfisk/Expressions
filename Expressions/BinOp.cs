using System;

namespace Expressions
{
    public class BinOp : Expression
    {
        private readonly Operator op;
        private readonly Expression e1, e2;

        public BinOp(Operator op, Expression e1, Expression e2)
        {
            this.op = op;
            this.e1 = e1;
            this.e2 = e2;
        }

        public override int Eval(REnv env, FEnv fEnv)
        {
            int v1 = e1.Eval(env, fEnv);
            int v2 = e2.Eval(env, fEnv);
            switch (op)
            {
                case Operator.Add:
                    return v1 + v2;
                case Operator.Div:
                    return v1 / v2;
                case Operator.Mul:
                    return v1 * v2;
                case Operator.Sub:
                    return v1 - v2;
                case Operator.Eq:
                    return v1 == v2 ? 1 : 0;
                case Operator.Ne:
                    return v1 != v2 ? 1 : 0;
                case Operator.Lt:
                    return v1 < v2 ? 1 : 0;
                case Operator.Le:
                    return v1 <= v2 ? 1 : 0;
                case Operator.Gt:
                    return v1 > v2 ? 1 : 0;
                case Operator.Ge:
                    return v1 >= v2 ? 1 : 0;
                case Operator.And:
                    return v1 == 0 ? 0 : v2;
                case Operator.Or:
                    return v1 == 0 ? v2 : 1;
                default:
                    throw new Exception("Unknown binary operator: " + op);
            }
        }

        public override Type Check(TEnv env, FEnv fEnv)
        {
            Type t1 = e1.Check(env, fEnv);
            Type t2 = e2.Check(env, fEnv);
            switch (op)
            {
                case Operator.Add:
                case Operator.Div:
                case Operator.Mul:
                case Operator.Sub:
                    if (t1 == Type.intType && t2 == Type.intType)
                        return Type.intType;
                    else
                        throw new TypeException("Arguments to + - * / must be int");
                case Operator.Eq:
                case Operator.Ge:
                case Operator.Gt:
                case Operator.Le:
                case Operator.Lt:
                case Operator.Ne:
                    if (t1 == Type.intType && t2 == Type.intType)
                        return Type.boolType;
                    else
                        throw new TypeException("Arguments to == >= > <= < != must be int");
                case Operator.Or:
                case Operator.And:
                    if (t1 == Type.boolType && t2 == Type.boolType)
                        return Type.boolType;
                    else
                        throw new TypeException("Arguments to & must be bool");
                default:
                    throw new Exception("Unknown binary operator: " + op);
            }
        }

        public override void Compile(CEnv env, Generator gen)
        {
            e1.Compile(env, gen);
            env.PushTemporary();
            e2.Compile(env, gen);
            switch (op)
            {
                case Operator.Add:
                    gen.Emit(Instruction.ADD);
                    break;
                case Operator.Div:
                    gen.Emit(Instruction.DIV);
                    break;
                case Operator.Mul:
                    gen.Emit(Instruction.MUL);
                    break;
                case Operator.Sub:
                    gen.Emit(Instruction.SUB);
                    break;
                case Operator.Eq:
                    gen.Emit(Instruction.EQ);
                    break;
                case Operator.Ne:
                    gen.Emit(Instruction.EQ);
                    gen.Emit(Instruction.NOT);
                    break;
                case Operator.Ge:
                    gen.Emit(Instruction.LT);
                    gen.Emit(Instruction.NOT);
                    break;
                case Operator.Gt:
                    gen.Emit(Instruction.SWAP);
                    gen.Emit(Instruction.LT);
                    break;
                case Operator.Le:
                    gen.Emit(Instruction.SWAP);
                    gen.Emit(Instruction.LT);
                    gen.Emit(Instruction.NOT);
                    break;
                case Operator.Lt:
                    gen.Emit(Instruction.LT);
                    break;
                case Operator.And:
                    gen.Emit(Instruction.MUL);
                    break;
                case Operator.Or:
                    gen.Emit(Instruction.ADD);
                    gen.Emit(new CSTI(0));
                    gen.Emit(Instruction.EQ);
                    gen.Emit(Instruction.NOT);
                    break;
                default:
                    throw new Exception("Unknown binary operator: " + op);
            }
            env.PopTemporary();
        }
    }
}