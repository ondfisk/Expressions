namespace Expressions
{
    public enum Opcode
    {
        LABEL = -1, // Unused
        CSTI, ADD, SUB, MUL, DIV, MOD, EQ, LT, NOT,
        DUP, SWAP, LDI, STI, GETBP, GETSP, INCSP,
        GOTO, IFZERO, IFNZRO, CALL, TCALL, RET,
        PRINTI, PRINTC, READ, LDARGS,
        STOP
    }
}