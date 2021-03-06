using System.Collections.Generic;



using System;

namespace Expressions {



public class Parser {
	public const int _EOF = 0;
	public const int _ident = 1;
	public const int _number = 2;
	public const int maxT = 21;

	const bool T = true;
	const bool x = false;
	const int minErrDist = 2;
	
	public Scanner scanner;
	public Errors  errors;

	public Token t;    // last recognized token
	public Token la;   // lookahead token
	int errDist = minErrDist;

public Program program;

/*------------------------------------------------------------------------------------------------*/


	public Parser(Scanner scanner) {
		this.scanner = scanner;
		errors = new Errors();
	}

	void SynErr (int n) {
		if (errDist >= minErrDist) errors.SynErr(la.line, la.col, n);
		errDist = 0;
	}

	public void SemErr (string msg) {
		if (errDist >= minErrDist) errors.SemErr(t.line, t.col, msg);
		errDist = 0;
	}
	
	void Get () {
		for (;;) {
			t = la;
			la = scanner.Scan();
			if (la.kind <= maxT) { ++errDist; break; }

			la = t;
		}
	}
	
	void Expect (int n) {
		if (la.kind==n) Get(); else { SynErr(n); }
	}
	
	bool StartOf (int s) {
		return set[s, la.kind];
	}
	
	void ExpectWeak (int n, int follow) {
		if (la.kind == n) Get();
		else {
			SynErr(n);
			while (!StartOf(follow)) Get();
		}
	}


	bool WeakSeparator(int n, int syFol, int repFol) {
		int kind = la.kind;
		if (kind == n) {Get(); return true;}
		else if (StartOf(repFol)) {return false;}
		else {
			SynErr(n);
			while (!(set[syFol, kind] || set[repFol, kind] || set[0, kind])) {
				Get();
				kind = la.kind;
			}
			return StartOf(syFol);
		}
	}

	
	void Program(out Program p) {
		p = null; FunctionDefinition f = null; string name = null; Expression e = null;                                      
		Dictionary<string, FunctionDefinition> functions = new Dictionary<string, FunctionDefinition>(); 
		while (la.kind == 7 || la.kind == 8) {
			FunctionDefinition(out f, out name);
			functions.Add(name, f); 
		}
		Expr(out e);
		p = new Program(functions, e); 
	}

	void FunctionDefinition(out FunctionDefinition f, out string name) {
		f = null; string an = null; Expression e = null;
		             Type at = null; Type rt = null;
		TypeExpr(out rt);
		Ident(out name);
		Expect(3);
		TypeExpr(out at);
		Ident(out an);
		Expect(4);
		Expect(5);
		Expr(out e);
		Expect(6);
		f = new FunctionDefinition(rt, name, at, an, e); 
	}

	void Expr(out Expression e) {
		Expression e1, e2; Operator op; e = null; 
		BoolTerm(out e1);
		e = e1; 
		while (la.kind == 9) {
			AndOp(out op);
			BoolTerm(out e2);
			e = new BinaryOperation(op, e, e2); 
		}
	}

	void TypeExpr(out Type t) {
		t = null; 
		if (la.kind == 7) {
			Get();
			t = Type.IntegerType; 
		} else if (la.kind == 8) {
			Get();
			t = Type.BooleanType; 
		} else SynErr(22);
	}

	void Ident(out string name) {
		Expect(1);
		name = t.val; 
	}

	void BoolTerm(out Expression e) {
		Expression e1, e2; Operator op; e = null; 
		SimBoolExpr(out e1);
		e = e1; 
		while (la.kind == 10) {
			OrOp(out op);
			SimBoolExpr(out e2);
			e = new BinaryOperation(op, e, e2); 
		}
	}

	void AndOp(out Operator op) {
		op = Operator.Bad; 
		Expect(9);
		op = Operator.And; 
	}

	void SimBoolExpr(out Expression e) {
		Expression e1, e2; Operator op; e = null; 
		SimExpr(out e1);
		e = e1; 
		if (StartOf(1)) {
			RelOp(out op);
			SimExpr(out e2);
			e = new BinaryOperation(op, e, e2); 
		}
	}

	void OrOp(out Operator op) {
		op = Operator.Bad; 
		Expect(10);
		op = Operator.Or; 
	}

	void SimExpr(out Expression e) {
		Expression e1, e2; Operator op; 
		Term(out e1);
		e = e1; 
		while (la.kind == 17 || la.kind == 18) {
			AddOp(out op);
			Term(out e2);
			e = new BinaryOperation(op, e, e2); 
		}
	}

	void RelOp(out Operator op) {
		op = Operator.Bad; 
		switch (la.kind) {
		case 11: {
			Get();
			op = Operator.Eq; 
			break;
		}
		case 12: {
			Get();
			op = Operator.Ne; 
			break;
		}
		case 13: {
			Get();
			op = Operator.Lt; 
			break;
		}
		case 14: {
			Get();
			op = Operator.Le; 
			break;
		}
		case 15: {
			Get();
			op = Operator.Gt; 
			break;
		}
		case 16: {
			Get();
			op = Operator.Ge; 
			break;
		}
		default: SynErr(23); break;
		}
	}

	void Term(out Expression e) {
		Operator op; Expression e1, e2; 
		Factor(out e1);
		e = e1; 
		while (la.kind == 19 || la.kind == 20) {
			MulOp(out op);
			Factor(out e2);
			e = new BinaryOperation(op, e, e2); 
		}
	}

	void AddOp(out Operator op) {
		op = Operator.Bad; 
		if (la.kind == 17) {
			Get();
			op = Operator.Add; 
		} else if (la.kind == 18) {
			Get();
			op = Operator.Sub; 
		} else SynErr(24);
	}

	void Factor(out Expression e) {
		string name; Expression e1; e = null; 
		if (la.kind == 1) {
			Ident(out name);
			e = new Variable(name); 
			if (la.kind == 3) {
				Get();
				Expr(out e1);
				Expect(4);
				e = new FunctionCall(name, e1); 
			}
		} else if (la.kind == 2) {
			Get();
			e = new Constant(int.Parse(t.val), Type.IntegerType); 
		} else if (la.kind == 18) {
			Get();
			Factor(out e1);
			e = new UnaryOperation(Operator.Neg, e1); 
		} else if (la.kind == 3) {
			Get();
			Expr(out e1);
			Expect(4);
			e = e1; 
		} else SynErr(25);
	}

	void MulOp(out Operator op) {
		op = Operator.Bad; 
		if (la.kind == 19) {
			Get();
			op = Operator.Mul; 
		} else if (la.kind == 20) {
			Get();
			op = Operator.Div; 
		} else SynErr(26);
	}

	void Expressions() {
		Program p; 
		Program(out p);
		program = p; 
	}



	public void Parse() {
		la = new Token();
		la.val = "";		
		Get();
		Expressions();
		Expect(0);

	}
	
	static readonly bool[,] set = {
		{T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x},
		{x,x,x,x, x,x,x,x, x,x,x,T, T,T,T,T, T,x,x,x, x,x,x}

	};
} // end Parser


public class Errors {
	public int count = 0;                                    // number of errors detected
	public System.IO.TextWriter errorStream = Console.Out;   // error messages go to this stream
	public string errMsgFormat = "-- line {0} col {1}: {2}"; // 0=line, 1=column, 2=text

	public virtual void SynErr (int line, int col, int n) {
		string s;
		switch (n) {
			case 0: s = "EOF expected"; break;
			case 1: s = "ident expected"; break;
			case 2: s = "number expected"; break;
			case 3: s = "\"(\" expected"; break;
			case 4: s = "\")\" expected"; break;
			case 5: s = "\"=\" expected"; break;
			case 6: s = "\";\" expected"; break;
			case 7: s = "\"int\" expected"; break;
			case 8: s = "\"bool\" expected"; break;
			case 9: s = "\"&\" expected"; break;
			case 10: s = "\"|\" expected"; break;
			case 11: s = "\"==\" expected"; break;
			case 12: s = "\"!=\" expected"; break;
			case 13: s = "\"<\" expected"; break;
			case 14: s = "\"<=\" expected"; break;
			case 15: s = "\">\" expected"; break;
			case 16: s = "\">=\" expected"; break;
			case 17: s = "\"+\" expected"; break;
			case 18: s = "\"-\" expected"; break;
			case 19: s = "\"*\" expected"; break;
			case 20: s = "\"/\" expected"; break;
			case 21: s = "??? expected"; break;
			case 22: s = "invalid TypeExpr"; break;
			case 23: s = "invalid RelOp"; break;
			case 24: s = "invalid AddOp"; break;
			case 25: s = "invalid Factor"; break;
			case 26: s = "invalid MulOp"; break;

			default: s = "error " + n; break;
		}
		errorStream.WriteLine(errMsgFormat, line, col, s);
		count++;
	}

	public virtual void SemErr (int line, int col, string s) {
		errorStream.WriteLine(errMsgFormat, line, col, s);
		count++;
	}
	
	public virtual void SemErr (string s) {
		errorStream.WriteLine(s);
		count++;
	}
	
	public virtual void Warning (int line, int col, string s) {
		errorStream.WriteLine(errMsgFormat, line, col, s);
	}
	
	public virtual void Warning(string s) {
		errorStream.WriteLine(s);
	}
} // Errors


public class FatalError: Exception {
	public FatalError(string m): base(m) {}
}
}