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
										Transition.DefaultTransition("output",14),
										Transition.CallTransition("if",51,17),
										Transition.DefaultTransition("for",23)},
				"Invalid Operator");
			// SETTER //
			table.AddState(12,
				new List<Transition>() {Transition.CallTransition("=",41,13)},
				"Missed =");
			table.AddState(13,
				new List<Transition>() {Transition.ExitTransition(Transition.NoLexem)},
				"[APPLICATION'S BUG]");
			// INPUT / OUTPUT //
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
			// IF //
			table.AddState(17,
				new List<Transition>() {Transition.CallTransition("\n",31,18)},
				"Missed ENTER");
			table.AddState(18,
				new List<Transition>() {Transition.DefaultTransition("\n",19)},
				"Missed ENTER");
			table.AddState(19,
				new List<Transition>() {Transition.DefaultTransition("else",20)},
				"Missed else");
			table.AddState(20,
				new List<Transition>() {Transition.CallTransition("\n",31,21)},
				"Missed ENTER");
			table.AddState(21,
				new List<Transition>() {Transition.DefaultTransition("\n",22)},
				"Missed ENTER");
			table.AddState(22,
				new List<Transition>() {Transition.ExitTransition("endif")},
				"Missed endif");
			// FOR //
			table.AddState(23,
				new List<Transition>() {Transition.DefaultTransition(Transition.LexemID,24)},
				"Missed variable-iterator");
			table.AddState(24,
				new List<Transition>() {Transition.CallTransition("=",41,25)},
				"Missed =");
			table.AddState(25,
				new List<Transition>() {Transition.CallTransition("to",41,26)},
				"Missed to");
			table.AddState(26,
				new List<Transition>() {Transition.CallTransition("step",41,27)},
				"Missed step");
			table.AddState(27,
				new List<Transition>() {Transition.CallTransition("\n",31,28)},
				"Missed ENTER");
			table.AddState(28,
				new List<Transition>() {Transition.DefaultTransition("\n",29)},
				"Missed ENTER");
			table.AddState(29,
				new List<Transition>() {Transition.ExitTransition("next")},
				"Missed next");

			// Half-automat Operators block //
			table.AddState(31,
				new List<Transition>() {Transition.DefaultTransition("{",32),
									    Transition.CallTransition(Transition.NoLexem,11,34)},
				"Missed { or OPERATOR");
			table.AddState(32,
				new List<Transition>() {Transition.DefaultTransition("\n",33)},
				"Missed ENTER");
			table.AddState(33,
				new List<Transition>() {Transition.ExitTransition("}"),
										Transition.CallTransition(Transition.NoLexem,11,32)},
				"Missed } or OPERATOR");
			table.AddState(34,
				new List<Transition>() {Transition.ExitTransition(Transition.NoLexem)},
				"[APPLICATION'S BUG]");

			// Half-automat Arifmetic Expression
			table.AddState(41,
				new List<Transition>() {Transition.DefaultTransition(Transition.LexemID,42),
										Transition.DefaultTransition(Transition.LexemCONST,42),
										Transition.CallTransition("(",41,43)},
				"Missed ID, CONST or (");
			table.AddState(42,
				new List<Transition>() {Transition.DefaultTransition("+",41),
										Transition.DefaultTransition("-",41),
										Transition.DefaultTransition("*",41),
										Transition.DefaultTransition("/",41),
										Transition.DefaultTransition("^",41),
										Transition.ExitTransition(Transition.NoLexem)},
				"[APPLICATION'S BUG]");
			table.AddState(43,
				new List<Transition>() {Transition.DefaultTransition(")",42)},
				"Missed )");

			// Half-operator Logical Expression //
			table.AddState(51,
				new List<Transition>() {Transition.DefaultTransition("!",51),
										Transition.CallTransition("[",51,52),
										Transition.CallTransition(Transition.NoLexem,41,54)},
				"[APPLICATION'S BUG]");
			table.AddState(52,
				new List<Transition>() {Transition.DefaultTransition("]",53)},
				"Missed ]");
			table.AddState(53,
				new List<Transition>() {Transition.DefaultTransition("and",51),
										Transition.DefaultTransition("or",51),
										Transition.ExitTransition(Transition.NoLexem)},
				"[APPLICATION'S BUG]");
			table.AddState(54,
				new List<Transition>() {Transition.CallTransition(">",41,53),
										Transition.CallTransition("<",41,53),
										Transition.CallTransition(">=",41,53),
										Transition.CallTransition("<=",41,53),
										Transition.CallTransition("!=",41,53),
										Transition.CallTransition("equ",41,53)},
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
			List<Lexem> lexems = LexemAnalyzer.sharedAnalyzer.Lexems;
			int lexemsIterator = 0;
			int currentState = 1;
			stack.Push(int.MaxValue);
			while (lexemsIterator < lexems.Count)
			{
				Out.Log(Out.State.LogVerbose,"On state "+currentState+
				        ". Will Process lexem: "+lexems[lexemsIterator].command);
				ProcessLexemOnState(lexems[lexemsIterator],
				                    ref lexemsIterator,ref currentState);
				Out.Log(Out.State.LogInfo,"Did Process "+lexemsIterator+
				        " of "+lexems.Count+" lexems");
			}
			Out.Log(Out.State.LogInfo,"Finish analyze");
		}

	}
}

