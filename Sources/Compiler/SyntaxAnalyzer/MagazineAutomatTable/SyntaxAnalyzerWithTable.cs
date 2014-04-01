using System;
using System.Collections.Generic;

namespace Translators
{
	public class SyntaxAnalyzerWithTable : ISyntaxAnalyzer
	{
		private static SyntaxAnalyzerWithTable _sharedAnalyzer = null;
		public static SyntaxAnalyzerWithTable sharedAnalyzer
		{
			get
			{
				if (_sharedAnalyzer == null) _sharedAnalyzer = new SyntaxAnalyzerWithTable();
				return _sharedAnalyzer;
			}
		}


		public Stack<int> stack = new Stack<int>();
		private AutomatTable table = new AutomatTable();

		private SyntaxAnalyzerWithTable() 
		{
			FIllTable();
		}

		private void FIllTable()
		{
			table.AddState(1,
				new List<Transition>() {Transition.DefaultTransition("@interface",2)},
				"Missed @interface");
			table.AddState(2,
				new List<Transition>() {Transition.DefaultTransition(Transition.LexemID,3)},
				"Missed ID Application's name");
			table.AddState(3,
				new List<Transition>() {Transition.DefaultTransition("\n",4)},
				"Missed ENTER");
			table.AddState(4,
				new List<Transition>() {Transition.DefaultTransition("int",5),
										Transition.DefaultTransition("@implementation",7)},
				"Missed int or @implementation");
			table.AddState(5,
				new List<Transition>() {Transition.DefaultTransition(Transition.LexemID,6)},
				"Missed variable's name");
			table.AddState(6,
				new List<Transition>() {Transition.DefaultTransition("\n",4),
										Transition.DefaultTransition(",",5)},
				"Missed ENTER or ,");
			table.AddState(7,
				new List<Transition>() {Transition.DefaultTransition("\n",8)},
				"Missed ENTER");
			table.AddState(8,
				new List<Transition>() {Transition.ExitTransition("@end"),
										Transition.CallTransition(Transition.NoLexem,11,7)},
				"Missed ENTER");

			// Half-automat Operator //
			table.AddState(11,
				new List<Transition>() {Transition.DefaultTransition(Transition.LexemID,12),
										Transition.DefaultTransition("input",14),
										Transition.DefaultTransition("output",17),
										Transition.CallTransition("if",61,20),
										Transition.DefaultTransition("for",27)},
				"Invalid Operator");
			// SETTER //
			table.AddState(12,
				new List<Transition>() {Transition.CallTransition("=",51,13)},
				"Missed =");
			table.AddState(13,
				new List<Transition>() {Transition.ExitTransition(Transition.NoLexem)},
				"[APPLICATION'S BUG]");

			// INPUT //
			table.AddState(14,
				new List<Transition>() {Transition.DefaultTransition("(",15)},
				"Missed (");
			table.AddState(15,
				new List<Transition>() {Transition.DefaultTransition(Transition.LexemID,16)},
				"Missed variable");
			table.AddState(16,
				new List<Transition>() {Transition.DefaultTransition(",",15),
										Transition.ExitTransition(")")},
				"Missed , or )");

			// OUTPUT //
			table.AddState(17,
			    new List<Transition>() {Transition.DefaultTransition("(",18)},
			"Missed (");
			table.AddState(18,
			    new List<Transition>() {Transition.DefaultTransition(Transition.LexemID,19),
										Transition.DefaultTransition(Transition.LexemCONST,19)},
			"Missed variable");
			table.AddState(19,
			    new List<Transition>() {Transition.DefaultTransition(",",18),
				  					    Transition.ExitTransition(")")},
			"Missed , or )");
			// IF //
			table.AddState(20,
			     new List<Transition>() {Transition.DefaultTransition("then",21)},
				"Missed then");
			table.AddState(21,
				new List<Transition>() {Transition.CallTransition("\n",41,22)},
				"Missed ENTER");
			table.AddState(22,
				new List<Transition>() {Transition.DefaultTransition("\n",23)},
				"Missed ENTER");
			table.AddState(23,
				new List<Transition>() {Transition.DefaultTransition("else",24),
										Transition.ExitTransition("endif")},
				"Missed else or endif");
			table.AddState(24,
				new List<Transition>() {Transition.CallTransition("\n",41,25)},
				"Missed ENTER");
			table.AddState(25,
				new List<Transition>() {Transition.DefaultTransition("\n",26)},
				"Missed ENTER");
			table.AddState(26,
				new List<Transition>() {Transition.ExitTransition("endif")},
				"Missed endif");
			// FOR //
			table.AddState(27,
				new List<Transition>() {Transition.DefaultTransition(Transition.LexemID,28)},
				"Missed variable-iterator");
			table.AddState(28,
				new List<Transition>() {Transition.CallTransition("=",51,29)},
				"Missed =");
			table.AddState(29,
				new List<Transition>() {Transition.CallTransition("step",51,30)},
				"Missed step");
			table.AddState(30,
				new List<Transition>() {Transition.CallTransition("to",51,31)},
				"Missed to");
			table.AddState(31,
			    new List<Transition>() {Transition.DefaultTransition("do",32)},
			"Missed ENTER");
			table.AddState(32,
				new List<Transition>() {Transition.CallTransition("\n",41,33)},
				"Missed ENTER");
			table.AddState(33,
				new List<Transition>() {Transition.DefaultTransition("\n",34)},
				"Missed ENTER");
			table.AddState(34,
				new List<Transition>() {Transition.ExitTransition("next")},
				"Missed next");

			// Half-automat Operators block //
			table.AddState(41,
				new List<Transition>() {Transition.DefaultTransition("{",42),
									    Transition.CallTransition(Transition.NoLexem,11,44)},
				"Missed { or OPERATOR");
			table.AddState(42,
				new List<Transition>() {Transition.DefaultTransition("\n",43)},
				"Missed ENTER");
			table.AddState(43,
				new List<Transition>() {Transition.ExitTransition("}"),
										Transition.CallTransition(Transition.NoLexem,11,42)},
				"Missed } or OPERATOR");
			table.AddState(44,
				new List<Transition>() {Transition.ExitTransition(Transition.NoLexem)},
				"[APPLICATION'S BUG]");

			// Half-automat Arifmetic Expression
			table.AddState(51,
				new List<Transition>() {Transition.DefaultTransition(Transition.LexemID,52),
										Transition.DefaultTransition(Transition.LexemCONST,52),
										Transition.CallTransition("(",51,53)},
				"Missed ID, CONST or (");
			table.AddState(52,
				new List<Transition>() {Transition.DefaultTransition("+",51),
										Transition.DefaultTransition("-",51),
										Transition.DefaultTransition("*",51),
										Transition.DefaultTransition("/",51),
										Transition.DefaultTransition("^",51),
										Transition.DefaultTransition("root",51),
										Transition.ExitTransition(Transition.NoLexem)},
				"[APPLICATION'S BUG]");
			table.AddState(53,
				new List<Transition>() {Transition.DefaultTransition(")",52)},
				"Missed )");

			// Half-operator Logical Expression //
			table.AddState(61,
				new List<Transition>() {Transition.DefaultTransition("!",61),
										Transition.CallTransition("[",61,62),
										Transition.CallTransition(Transition.NoLexem,51,64)},
				"[APPLICATION'S BUG]");
			table.AddState(62,
				new List<Transition>() {Transition.DefaultTransition("]",63)},
				"Missed ]");
			table.AddState(63,
				new List<Transition>() {Transition.DefaultTransition("and",61),
										Transition.DefaultTransition("or",61),
										Transition.ExitTransition(Transition.NoLexem)},
				"[APPLICATION'S BUG]");
			table.AddState(64,
				new List<Transition>() {Transition.CallTransition(">",51,63),
										Transition.CallTransition("<",51,63),
										Transition.CallTransition(">=",51,63),
										Transition.CallTransition("<=",51,63),
										Transition.CallTransition("!=",51,63),
										Transition.CallTransition("equ",51,63)},
				"Missed >, <, >=, <=, !=, or equ");
		}		

		public void ProcessLexemOnState(Lexem lexem, ref int lexemIterator, 
		                                ref int stateNumber)
		{
			State state = table.StateWithNumber(stateNumber);
			state.Run(lexem,ref stateNumber, ref lexemIterator);
		}

		public void AnalyzeLexems()
		{
			List<Lexem> lexems = LexemList.Instance.Lexems;
			int lexemsIterator = 0;
			int currentState = 1;
			stack.Push(int.MaxValue);
			while (lexemsIterator < lexems.Count)
			{
				Out.Log(Out.State.LogVerbose,"On state "+currentState+
				        ". Will Process lexem: "+lexems[lexemsIterator].Command);
				ProcessLexemOnState(lexems[lexemsIterator],
				                    ref lexemsIterator,ref currentState);
				Out.Log(Out.State.LogInfo,"Did Process "+lexemsIterator+
				        " of "+lexems.Count+" lexems");
			}
			Out.Log(Out.State.LogInfo,"Finish analyze");
		}

	}
}

