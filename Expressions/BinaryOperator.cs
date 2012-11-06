using System;

namespace Expressions
{
    public class BinaryOperation : Expression
    {
        private readonly Operator _op;
        private readonly Expression _e1, _e2;

        public BinaryOperation(Operator op, Expression e1, Expression e2)
        {
            _op = op;
            _e1 = e1;
            _e2 = e2;
        }

        public override int Eval(RuntimeEnvironment runtimeEnvironment, FunctionEnvironment functionEnvironment)
        {
            var v1 = _e1.Eval(runtimeEnvironment, functionEnvironment);
            var v2 = _e2.Eval(runtimeEnvironment, functionEnvironment);

            switch (_op)
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
                    throw new InvalidOperationException(string.Format("Unknown binary operator: {0}", _op));
            }
        }

        public override Type Check(TypeCheckingEnvironment typeCheckingEnvironment, FunctionEnvironment functionEnvironment)
        {
            var t1 = _e1.Check(typeCheckingEnvironment, functionEnvironment);
            var t2 = _e2.Check(typeCheckingEnvironment, functionEnvironment);

            switch (_op)
            {
                case Operator.Add:
                case Operator.Div:
                case Operator.Mul:
                case Operator.Sub:
                    if (t1 == Type.intType && t2 == Type.intType)
                    {
                        return Type.intType;
                    }
                    throw new InvalidOperationException("Arguments to + - * / must be int");
                case Operator.Eq:
                case Operator.Ge:
                case Operator.Gt:
                case Operator.Le:
                case Operator.Lt:
                case Operator.Ne:
                    if (t1 == Type.intType && t2 == Type.intType)
                    {
                        return Type.boolType;
                    }
                    throw new InvalidOperationException("Arguments to == >= > <= < != must be int");
                case Operator.Or:
                case Operator.And:
                    if (t1 == Type.boolType && t2 == Type.boolType)
                    {
                        return Type.boolType;
                    }
                    throw new InvalidOperationException("Arguments to & must be bool");
                default: 
                    throw new InvalidOperationException(string.Format("Unknown binary operator: {0}", _op));
            }
        }

        public override void Compile(CompilationEnvironment env, Generator gen)
        {
            _e1.Compile(env, gen);
            env.PushTemporary();
            _e2.Compile(env, gen);

            switch (_op)
            {
                case Operator.Add:
                    gen.Emit(Instruction.Add);
                    break;
                case Operator.Div:
                    gen.Emit(Instruction.Div);
                    break;
                case Operator.Mul:
                    gen.Emit(Instruction.Mul);
                    break;
                case Operator.Sub:
                    gen.Emit(Instruction.Sub);
                    break;
                case Operator.Eq:
                    gen.Emit(Instruction.Eq);
                    break;
                case Operator.Ne:
                    gen.Emit(Instruction.Eq);
                    gen.Emit(Instruction.Not);
                    break;
                case Operator.Ge:
                    gen.Emit(Instruction.LT);
                    gen.Emit(Instruction.Not);
                    break;
                case Operator.Gt:
                    gen.Emit(Instruction.Swap);
                    gen.Emit(Instruction.LT);
                    break;
                case Operator.Le:
                    gen.Emit(Instruction.Swap);
                    gen.Emit(Instruction.LT);
                    gen.Emit(Instruction.Not);
                    break;
                case Operator.Lt:
                    gen.Emit(Instruction.LT);
                    break;
                case Operator.And:
                    gen.Emit(Instruction.Mul);
                    break;
                case Operator.Or:
                    gen.Emit(Instruction.Add);
                    gen.Emit(new CstI(0));
                    gen.Emit(Instruction.Eq);
                    gen.Emit(Instruction.Not);
                    break;
                default:
                    throw new InvalidOperationException(string.Format("Unknown binary operator: {0}", _op));
            }
            env.PopTemporary();
        }
    }
}